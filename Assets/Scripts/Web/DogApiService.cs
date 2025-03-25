using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

// Сервис для загрузки данных о породах собак через API
public class DogApiService
{
    private string _url = "https://dogapi.dog/api/v2/breeds"; // Базовый URL API пород
    [Inject] private BreedsListPanel _breedsListPanel;        // Панель для отображения списка пород
    [Inject] private RequestQueue _requestQueue;              // Очередь для управления запросами
    private string _currentBreedId;                           // ID текущей загружаемой породы
    private const string BREEDS_TASK_ID = "Breeds";           // Идентификатор задачи загрузки списка пород
    private const string BREED_DETAILS_TASK_ID = "BreedDetails"; // Идентификатор задачи загрузки деталей породы

    // Загружает список пород и отображает их в UI
    public void LoadBreeds(System.Action onSuccess)
    {
        _breedsListPanel.CancelAllLoaders();       // Отменяем все текущие загрузки
        _requestQueue.CancelAndRemove(BREEDS_TASK_ID); // Удаляем предыдущую задачу загрузки списка
        // Добавляем новую задачу в очередь
        _requestQueue.Enqueue(token => GetBreedsAsync(onSuccess, token), BREEDS_TASK_ID);
    }

    // Загружает детали конкретной породы и передаёт их в коллбэк
    public void LoadBreedDetails(string id, System.Action<string, string> onSuccess)
    {
        if (id == _currentBreedId) return;         // Пропускаем, если порода уже загружается
        _breedsListPanel.CancelAllLoaders(id);     // Отменяем загрузки других кнопок, кроме текущей
        _requestQueue.CancelAndRemove(BREED_DETAILS_TASK_ID); // Удаляем предыдущую задачу деталей
        _currentBreedId = id;                      // Сохраняем текущий ID породы

        // Добавляем задачу загрузки деталей в очередь
        _requestQueue.Enqueue(token => GetBreedDetailsAsync(id, onSuccess, token), BREED_DETAILS_TASK_ID);
    }

    // Останавливает все активные загрузки
    public void StopAllLoadings()
    {
        _requestQueue.CancelAndRemove(BREEDS_TASK_ID);
        _requestQueue.CancelAndRemove(BREED_DETAILS_TASK_ID);
    }

    // Асинхронно загружает список пород из API
    private async UniTask GetBreedsAsync(System.Action onSuccess, CancellationToken token)
    {
        using UnityWebRequest request = UnityWebRequest.Get(_url);
        await request.SendWebRequest().ToUniTask(cancellationToken: token);
        if (request.result == UnityWebRequest.Result.Success)
        {
            // Десериализуем ответ и создаём кнопки для первых 10 пород
            var response = JsonConvert.DeserializeObject<BreedResponse>(request.downloadHandler.text);
            for (int i = 0; i < Mathf.Min(10, response.data.Length); i++)
            {
                var breed = response.data[i];
                _breedsListPanel.CreateButton(breed.id, breed.attributes.name);
            }
            onSuccess?.Invoke(); // Вызываем коллбэк после успешной загрузки
        }
        else
            Debug.LogError("Ошибка запроса пород: " + request.error);
    }

    // Асинхронно загружает детали породы по ID
    private async UniTask GetBreedDetailsAsync(string id, System.Action<string, string> onSuccess, CancellationToken token)
    {
        using UnityWebRequest request = UnityWebRequest.Get(_url + "/" + id);
        await request.SendWebRequest().ToUniTask(cancellationToken: token);
        if (request.result == UnityWebRequest.Result.Success)
        {
            // Десериализуем ответ и передаём данные в коллбэк
            var response = JsonConvert.DeserializeObject<BreedSingleResponse>(request.downloadHandler.text);
            var breed = response.data;
            string title = breed.attributes.name;
            string description = breed.attributes.description;
            onSuccess?.Invoke(title, description);
            ResetCurrentBreedId(); // Сбрасываем текущий ID после загрузки
        }
        else
            Debug.LogError("Ошибка запроса деталей породы: " + request.error);
    }

    // Сбрасывает ID текущей породы после завершения загрузки
    private void ResetCurrentBreedId()
    {
        _currentBreedId = "";
    }
}