using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
public class WeatherApiService : MonoBehaviour
{
    [SerializeField] private string _url;
    [SerializeField] private RequestQueue _requestQueue;
    [SerializeField] private WeatherPanel _weatherPanel;
    private CancellationTokenSource _loopCts;
    private bool _loopIsActive = false;
    private const string WEATHER_TASK_ID = "Weather";


    public void StartUpdating(System.Action onSuccess)
    {
        if (!_loopIsActive)
        {
            _loopCts = new CancellationTokenSource();
            LoopRequestsAsync(onSuccess, _loopCts.Token).Forget();
            _loopIsActive = true;
        }
    }

    public void StopUpdating()
    {
        _loopIsActive = false;
        _loopCts?.Cancel();
        _loopCts?.Dispose();
    }

    private async UniTaskVoid LoopRequestsAsync(System.Action onSuccess, CancellationToken loopToken)
    {
        while (!loopToken.IsCancellationRequested)
        {
            _requestQueue.Enqueue(token => GetWeatherAsync(onSuccess, token), WEATHER_TASK_ID);
            await UniTask.Delay(5000, cancellationToken: loopToken);
        }
    }

    private async UniTask GetWeatherAsync(System.Action onSuccess, CancellationToken token)
    {
        using UnityWebRequest request = UnityWebRequest.Get(_url);
        await request.SendWebRequest().ToUniTask(cancellationToken: token);
        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonConvert.DeserializeObject<WeatherApiResponse>(request.downloadHandler.text);
            var today = response?.properties?.periods?[0];
            if (today != null)
            {
                Texture2D icon = await GetIconAsync(today.icon, token);
                if (icon != null)
                {
                    _weatherPanel.ShowWeather(icon, today.name, today.temperature);
                }
                else
                {
                    Debug.LogWarning("Иконка не загружена, но покажем текст");
                    _weatherPanel.ShowWeather(null, today.name, today.temperature);
                }
                onSuccess?.Invoke();
            }
        }
        else
            Debug.LogError("Ошибка запроса погоды: " + request.error);
    }
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
            return DownloadHandlerTexture.GetContent(request);
        return null;
    }
}
