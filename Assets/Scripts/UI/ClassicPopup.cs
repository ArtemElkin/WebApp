using TMPro;
using UnityEngine;

public class ClassicPopup : MonoBehaviour, IPopup
{
    [SerializeField] private TextMeshProUGUI _title, _description;

    
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(string breedname, string breedinfo)
    {
        _title.text = breedname;
        _description.text = breedinfo;
        gameObject.SetActive(true);
    }
}
