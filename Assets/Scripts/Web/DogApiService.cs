using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;


public class DogApiService : MonoBehaviour
{
    [SerializeField] private string _url;
    [SerializeField] private BreedsListPanel _breedsListPanel;
    [SerializeField] private RequestQueue _requestQueue;
    private string _currentBreedId;
    private const string BREEDS_TASK_ID = "Breeds";
    private const string BREED_DETAILS_TASK_ID = "BreedDetails";

    public void ResetCurrentBreedId()
    {
        _currentBreedId = "";
    }

    public void LoadBreeds(System.Action onSuccess)
    {
        _breedsListPanel.CancelAllLoaders();
        _requestQueue.CancelAndRemove(BREEDS_TASK_ID);
        _requestQueue.Enqueue(token => GetBreedsAsync(onSuccess, token), BREEDS_TASK_ID);
    }

    public void LoadBreedDetails(string id, System.Action<string, string> onSuccess)
    {
        if (id == _currentBreedId)
            return;
        _breedsListPanel.CancelAllLoaders(id);
        _requestQueue.CancelAndRemove(BREED_DETAILS_TASK_ID);
        _currentBreedId = id;

        _requestQueue.Enqueue(token => GetBreedDetailsAsync(id, onSuccess, token), BREED_DETAILS_TASK_ID);
    }

    public void StopAllLoadings()
    {
        _requestQueue.CancelAndRemove(BREEDS_TASK_ID);
        _requestQueue.CancelAndRemove(BREED_DETAILS_TASK_ID);
    }


    private async UniTask GetBreedsAsync(System.Action onSuccess, CancellationToken token)
    {
        using UnityWebRequest request = UnityWebRequest.Get(_url);
        await request.SendWebRequest().ToUniTask(cancellationToken: token);
        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonConvert.DeserializeObject<BreedResponse>(request.downloadHandler.text);
            for (int i = 0; i < Mathf.Min(10, response.data.Length); i++)
            {
                var breed = response.data[i];
                _breedsListPanel.CreateButton(breed.id, breed.attributes.name);
            }
            onSuccess?.Invoke();
        }
        else
            Debug.LogError("Ошибка запроса пород: " + request.error);
    }

    private async UniTask GetBreedDetailsAsync(string id, System.Action<string, string> onSuccess, CancellationToken token)
    {
        using UnityWebRequest request = UnityWebRequest.Get(_url + "/" + id);
        await request.SendWebRequest().ToUniTask(cancellationToken: token);
        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonConvert.DeserializeObject<BreedSingleResponse>(request.downloadHandler.text);
            var breed = response.data;
            string title = breed.attributes.name;
            string description = breed.attributes.description;
            onSuccess?.Invoke(title, description);
        }
        else
            Debug.LogError("Ошибка запроса деталей породы: " + request.error);
    }

}
