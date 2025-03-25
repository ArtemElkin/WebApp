using UnityEngine;

public class TabController : MonoBehaviour
{
    [SerializeField] private WeatherApiService _weatherApiService;
    [SerializeField] private DogApiService _dogApiService;
    [SerializeField] private WeatherPanel _weatherPanel;
    [SerializeField] private BreedsListPanel _breedsListPanel;
    [SerializeField] private GameObject _loadingImg;

    private enum Tab { Weather, Breeds }
    private Tab _currentTab = (Tab)(-1); // Некорректное значение для срабатывания SwitchTab на старте

    private void Start()
    {
        SwitchTab(Tab.Weather); // По умолчанию открываем погоду
    }

    private void SwitchTab(Tab newTab)
    {
        if (_currentTab == newTab) return;

        // Останавливаем текущую вкладку
        switch (_currentTab)
        {
            case Tab.Weather:
                _weatherApiService.StopUpdating();
                _weatherPanel.gameObject.SetActive(false);
                _loadingImg.SetActive(false);
                break;
            case Tab.Breeds:
                _dogApiService.StopAllLoadings();
                _breedsListPanel.gameObject.SetActive(false);
                _loadingImg.SetActive(false);
                break;
        }

        // Запускаем новую вкладку
        _currentTab = newTab;
        switch (_currentTab)
        {
            case Tab.Weather:
                _loadingImg.SetActive(true);
                _weatherApiService.StartUpdating(() =>
                {
                    _loadingImg.SetActive(false);
                    _weatherPanel.gameObject.SetActive(true);
                });
                break;
            case Tab.Breeds:
                _breedsListPanel.ClearList();
                _loadingImg.SetActive(true);
                _dogApiService.LoadBreeds(() =>
                {
                    _loadingImg.SetActive(false);
                    _breedsListPanel.gameObject.SetActive(true);
                });
                break;
        }
    }

    // Публичные методы для UI
    public void OnWeatherTabClicked() => SwitchTab(Tab.Weather);
    public void OnBreedsTabClicked() => SwitchTab(Tab.Breeds);
}