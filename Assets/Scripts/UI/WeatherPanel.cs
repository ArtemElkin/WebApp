using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Панель для отображения текущей погоды
public class WeatherPanel : MonoBehaviour
{
    [SerializeField] private Image _iconImg;         // Иконка погоды
    [SerializeField] private TextMeshProUGUI _text;  // Текстовое поле для описания погоды и температуры

    // Отображает данные о погоде: иконку и текст с температурой
    public void ShowWeather(Texture2D texture, string text, int temperature)
    {
        // Устанавливаем текст с описанием и температурой в формате "описание – температура°F"
        _text.text = $"{text} – {temperature}°F";

        // Если есть текстура, создаём спрайт и показываем иконку
        if (texture != null)
        {
            if (!_iconImg.gameObject.activeInHierarchy)
                _iconImg.gameObject.SetActive(true);
            _iconImg.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}