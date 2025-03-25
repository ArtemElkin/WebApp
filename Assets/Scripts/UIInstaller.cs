using UnityEngine;
using Zenject;

public class UIInstaller : MonoInstaller
{
    [SerializeField] TabController _tabController;
    [SerializeField] private BreedsListPanel _breedListPanel;
    [SerializeField] private WeatherPanel _weatherPanel;
    [SerializeField] private DoTweenPopup _popup;
    public override void InstallBindings()
    {
        if (_tabController == null || _breedListPanel == null || _weatherPanel == null)
        {
            Debug.LogError("Не привязаны один или несколько UI элементов!");
            return;
        }
        Container.Bind<TabController>().FromInstance(_tabController).AsSingle();
        Container.Bind<BreedsListPanel>().FromInstance(_breedListPanel).AsSingle();
        Container.Bind<WeatherPanel>().FromInstance(_weatherPanel).AsSingle();
        Container.Bind<IPopup>().To<DoTweenPopup>().FromInstance(_popup).AsSingle();
    }
}