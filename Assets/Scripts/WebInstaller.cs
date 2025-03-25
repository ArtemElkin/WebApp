using Zenject;

public class WebInstaller : MonoInstaller
{
    public override void InstallBindings()
{
    Container.Bind<WeatherApiService>().AsSingle();
    Container.Bind<DogApiService>().AsSingle();
    Container.Bind<RequestQueue>().AsSingle();
}
}