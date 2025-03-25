using UnityEngine;
using UnityEngine.UI;
using Zenject;

// Панель меню для переключения между вкладками погоды и пород
public class MenuPanel : MonoBehaviour
{
    [SerializeField] private Button _weatherTabBtn; // Кнопка для вкладки погоды
    [SerializeField] private Button _breedsTabBtn;  // Кнопка для вкладки пород
    [Inject] private TabController _tabController;  // Контроллер для управления вкладками

    // Подписываемся на события кликов по кнопкам при активации
    private void OnEnable()
    {
        _weatherTabBtn.onClick.AddListener(_tabController.OnWeatherTabClicked);
        _breedsTabBtn.onClick.AddListener(_tabController.OnBreedsTabClicked);
    }
    
    // Отписываемся от событий кликов  при деактивации для предотвращения утечек памяти
    private void OnDisable()
    {
        _weatherTabBtn.onClick.RemoveListener(_tabController.OnWeatherTabClicked);
        _breedsTabBtn.onClick.RemoveListener(_tabController.OnBreedsTabClicked);
    }
}