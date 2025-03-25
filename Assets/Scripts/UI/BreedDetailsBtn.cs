using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BreedDetailsBtn : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _num, _name;
    [SerializeField] private GameObject _loadingImg;
    [SerializeField] private Button _button;
    private IPopup _popup;
    private DogApiService _api;
    public string _breedId;


    private void OnEnable()
    {
        _button.onClick.AddListener(ButtonClickHandler);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(ButtonClickHandler);
    }

    public void Init(string id, string name, int num, DogApiService api, IPopup popup)
    {
        _breedId = id;
        _num.text = num.ToString();
        _name.text = name;
        _api = api;
        _popup = popup;
    }

    public void CancelLoading()
    {
        _loadingImg.SetActive(false);
    }
    
    private void ButtonClickHandler()
    {
        _loadingImg.SetActive(true);

        _api.LoadBreedDetails(_breedId, (title, description) =>
        {
            _loadingImg.SetActive(false);
            _popup.Show(title, description);
        });
    }
}
