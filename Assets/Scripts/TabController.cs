using UnityEngine;
using Zenject;

// Контроллер вкладок главного интерфейса
public class TabController : MonoBehaviour
{
    [Inject] private WeatherApiService _weatherApiService;    // Сервис погоды
    [Inject] private DogApiService _dogApiService;            // Сервис пород
    [Inject] private WeatherPanel _weatherPanel;              // Панель погоды
    [Inject] private BreedsListPanel _breedsListPanel;        // Панель пород
    [SerializeField] private GameObject _loadingImg;          // Индикатор загрузки

    private enum Tab { None, Weather, Breeds }                // Типы вкладок
    private Tab _currentTab = Tab.None;                       // Активная вкладка

    // Запуск — сразу переходим на вкладку погоды
    private void Start()
    {
        SwitchTab(Tab.Weather);
    }

    // Основная логика переключения между вкладками
    private void SwitchTab(Tab newTab)
    {
        if (_currentTab == newTab) return; // Если уже на нужной вкладке — ничего не делаем

        // Отключаем старую вкладку
        switch (_currentTab)
        {
            case Tab.Weather:
                _weatherApiService.StopUpdating();                     // Остановить запросы погоды
                _weatherPanel.gameObject.SetActive(false);            // Скрыть панель погоды
                break;
            case Tab.Breeds:
                _dogApiService.StopAllLoadings();                     // Остановить запросы пород
                _breedsListPanel.gameObject.SetActive(false);         // Скрыть панель пород
                break;
        }

        _loadingImg.SetActive(true); // Показываем спиннер загрузки
        _currentTab = newTab;        // Обновляем текущую вкладку

        // Включаем новую вкладку
        switch (_currentTab)
        {
            case Tab.Weather:
                // Запускаем сервис погоды, по завершении включаем панель
                _weatherApiService.StartUpdating(() =>
                {
                    _loadingImg.SetActive(false);
                    _weatherPanel.gameObject.SetActive(true);
                });
                break;
            case Tab.Breeds:
                _breedsListPanel.ClearList(); // Очищаем старые кнопки
                _dogApiService.LoadBreeds(() =>
                {
                    _loadingImg.SetActive(false);
                    _breedsListPanel.gameObject.SetActive(true);
                });
                break;
        }
    }

    // Методы, вызываемые UI-кнопками
    public void OnWeatherTabClicked() => SwitchTab(Tab.Weather);
    public void OnBreedsTabClicked() => SwitchTab(Tab.Breeds);
}
