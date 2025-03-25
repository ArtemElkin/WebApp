using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WeatherPanel : MonoBehaviour
{
    [SerializeField] private Image _iconImg;
    [SerializeField] private TextMeshProUGUI _text;


    public void ShowWeather(Texture2D texture, string text, int temperature)
    {
        _text.text = $"{text} – {temperature}°F";
        if (texture != null)
        {
            if (!_iconImg.gameObject.activeInHierarchy)
                _iconImg.gameObject.SetActive(true);
            _iconImg.sprite = Sprite.Create(texture, new Rect(0,0,texture.width,texture.height), new Vector2(0.5f,0.5f));
        }
    }

}
