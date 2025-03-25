[System.Serializable]
public class WeatherApiResponse
{
    public WeatherProperties properties;
}

[System.Serializable]
public class WeatherProperties
{
    public WeatherPeriod[] periods;
}

[System.Serializable]
public class WeatherPeriod
{
    public string name;         // Сегодня / Tonight
    public int temperature;     // 61
    public string icon;         // URL картинки
}