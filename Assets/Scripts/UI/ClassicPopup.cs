using TMPro;
using UnityEngine;

// Простой попап для отображения информации, реализующий интерфейс IPopup
public class ClassicPopup : MonoBehaviour, IPopup
{
    [SerializeField] private TextMeshProUGUI _title;      // Текстовое поле для заголовка попапа
    [SerializeField] private TextMeshProUGUI _description; // Текстовое поле для описания

    // Скрывает попап, деактивируя объект
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // Показывает попап с заданным заголовком и описанием
    public void Show(string breedname, string breedinfo)
    {
        _title.text = breedname;      // Устанавливаем заголовок
        _description.text = breedinfo; // Устанавливаем описание
        gameObject.SetActive(true);   // Активируем попап
    }
}