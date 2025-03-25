using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Zenject;

// Сервис для периодической загрузки и обновления данных о погоде через API
public class WeatherApiService
{
    private string _url = "https://api.weather.gov/gridpoints/TOP/32,81/forecast"; // Базовый URL API погоды
    [Inject] private RequestQueue _requestQueue;        // Очередь для управления запросами
    [Inject] private WeatherPanel _weatherPanel;        // Панель для отображения данных погоды
    private CancellationTokenSource _loopCts;           // Токен для управления циклом обновлений
    private bool _loopIsActive = false;                 // Флаг активности цикла обновления
    private const string WEATHER_TASK_ID = "Weather";   // Идентификатор задачи погоды

    // Запускает периодическое обновление данных о погоде
    public void StartUpdating(System.Action onSuccess)
    {
        if (!_loopIsActive)
        {
            _loopCts = new CancellationTokenSource(); // Создаём токен для цикла
            LoopRequestsAsync(onSuccess, _loopCts.Token).Forget(); // Запускаем цикл асинхронно
            _loopIsActive = true;
        }
    }

    // Останавливает цикл обновления погоды
    public void StopUpdating()
    {
        _loopIsActive = false;
        _loopCts?.Cancel();  // Отменяем цикл
        _loopCts?.Dispose(); // Очищаем токен
    }

    // Выполняет периодические запросы погоды с интервалом
    private async UniTaskVoid LoopRequestsAsync(System.Action onSuccess, CancellationToken loopToken)
    {
        while (!loopToken.IsCancellationRequested)
        {
            // Добавляем задачу получения погоды в очередь
            _requestQueue.Enqueue(token => GetWeatherAsync(onSuccess, token), WEATHER_TASK_ID);
            await UniTask.Delay(5000, cancellationToken: loopToken); // Ждём 5 секунд перед следующим запросом
        }
    }

    // Асинхронно загружает данные о погоде из API
    private async UniTask GetWeatherAsync(System.Action onSuccess, CancellationToken token)
    {
        using UnityWebRequest request = UnityWebRequest.Get(_url);
        await request.SendWebRequest().ToUniTask(cancellationToken: token);
        if (request.result == UnityWebRequest.Result.Success)
        {
            // Десериализуем ответ и берём данные за текущий период
            var response = JsonConvert.DeserializeObject<WeatherApiResponse>(request.downloadHandler.text);
            var today = response?.properties?.periods?[0];
            if (today != null)
            {
                Texture2D icon = await GetIconAsync(today.icon, token); // Загружаем иконку погоды
                if (icon != null)
                {
                    _weatherPanel.ShowWeather(icon, today.name, today.temperature);
                }
                else
                {
                    Debug.LogWarning("Иконка не загружена, но покажем текст");
                    _weatherPanel.ShowWeather(null, today.name, today.temperature);
                }
                onSuccess?.Invoke(); // Уведомляем об успешной загрузке
            }
        }
        else
            Debug.LogError("Ошибка запроса погоды: " + request.error);
    }

    // Асинхронно загружает иконку погоды по URL
    private async UniTask<Texture2D> GetIconAsync(string iconUrl, CancellationToken token)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(iconUrl);
        try
        {
            await request.SendWebRequest().ToUniTask(cancellationToken: token);
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Запрос иконки отменён");
            return null;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ошибка в запросе иконки: {ex.Message}");
            return null;
        }
        if (request.result == UnityWebRequest.Result.Success)
            return DownloadHandlerTexture.GetContent(request); // Возвращаем загруженную текстуру
        return null;
    }
}