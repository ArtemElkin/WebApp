using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] private Button _weatherTabBtn, _breedsTabBtn;
    [SerializeField] private TabController _tabController;


    private void OnEnable()
    {
        _weatherTabBtn.onClick.AddListener(_tabController.OnWeatherTabClicked);
        _breedsTabBtn.onClick.AddListener(_tabController.OnBreedsTabClicked);
    }
    
    private void OnDisable()
    {
        _weatherTabBtn.onClick.RemoveListener(_tabController.OnWeatherTabClicked);
        _breedsTabBtn.onClick.RemoveListener(_tabController.OnBreedsTabClicked);
    }
}
