using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

// Попап с анимацией появления/исчезновения через DoTween
public class DoTweenPopup : MonoBehaviour, IPopup
{
    [SerializeField] private CanvasGroup _canvasGroup;         // Для fade-анимации
    [SerializeField] private Transform _popupRoot;             // Объект, который масштабируем
    [SerializeField] private TextMeshProUGUI _title;           // Заголовок
    [SerializeField] private TextMeshProUGUI _description;     // Описание
    private Tween _currentTween; // Текущая анимация

    private void Awake()
    {
        // Скрыть при старте
        _canvasGroup.alpha = 0;
        _popupRoot.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void Show(string breedname, string breedinfo)
    {
        gameObject.SetActive(true);

        _title.text = breedname;
        _description.text = breedinfo;

        // Отменяем предыдущее, если шло
        _currentTween?.Kill();

        // Анимация появления: масштаб + прозрачность
        _popupRoot.localScale = Vector3.zero;
        _canvasGroup.alpha = 0;

        _currentTween = DOTween.Sequence()
            .Append(_popupRoot.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack))
            .Join(_canvasGroup.DOFade(1f, 0.2f));
    }

    public void Hide()
    {
        // Отменяем предыдущее, если шло
        _currentTween?.Kill();

        // Анимация скрытия
        _currentTween = DOTween.Sequence()
            .Append(_canvasGroup.DOFade(0f, 0.2f))
            .Join(_popupRoot.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack))
            .SetUpdate(true)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
}
