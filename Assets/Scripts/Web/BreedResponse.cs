[System.Serializable]
public class BreedResponse
{
    public BreedData[] data;
}

[System.Serializable]
public class BreedData
{
    public string id;
    public BreedAttributes attributes;
}

[System.Serializable]
public class BreedAttributes
{
    public string name;
    public string description;
}

[System.Serializable]
public class BreedSingleResponse
{
    public BreedData data;
}