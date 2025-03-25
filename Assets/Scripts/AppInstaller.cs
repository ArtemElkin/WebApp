using UnityEngine;
using Zenject;

public class AppInstaller : MonoInstaller
{
    [SerializeField] private WeatherApiService _weatherApiService;
    [SerializeField] private DogApiService _dogApiService;
    public override void InstallBindings()
    {
        Container.Bind<WeatherApiService>().FromInstance(_weatherApiService);
        Container.Bind<DogApiService>().FromInstance(_dogApiService);
        Container.Bind<RequestQueue>().FromNew();
        
    }
}