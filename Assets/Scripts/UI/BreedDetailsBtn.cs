using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Класс кнопки для отображения информации о породе собак
public class BreedDetailsBtn : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _num, _name; // Текстовые поля для номера и названия породы
    [SerializeField] private GameObject _loadingImg;      // Индикатор загрузки данных о породе
    [SerializeField] private Button _button;             // Кнопка для вызова деталей породы
    private IPopup _popup;                               // Интерфейс попапа для отображения деталей
    private DogApiService _api;                          // Сервис для загрузки данных о породах
    public string _breedId;                              // Идентификатор породы

    // Подписываемся на событие клика при активации объекта
    private void OnEnable()
    {
        _button.onClick.AddListener(ButtonClickHandler);
    }

    // Отписываемся от события клика при деактивации объекта
    private void OnDisable()
    {
        _button.onClick.RemoveListener(ButtonClickHandler);
    }

    // Инициализирует кнопку данными породы и зависимостями
    public void Init(string id, string name, int num, DogApiService api, IPopup popup)
    {
        _breedId = id;
        _num.text = num.ToString();
        _name.text = name;
        _api = api;
        _popup = popup;
    }

    // Отменяет отображение индикатора загрузки
    public void CancelLoading()
    {
        _loadingImg.SetActive(false);
    }
    
    // Обрабатывает клик по кнопке, загружает и показывает детали породы
    private void ButtonClickHandler()
    {
        _loadingImg.SetActive(true); // Показываем индикатор загрузки
        
        // Запрашиваем данные о породе и показываем их в попапе после загрузки
        _api.LoadBreedDetails(_breedId, (title, description) =>
        {
            _loadingImg.SetActive(false);
            _popup.Show(title, description);
        });
    }
}