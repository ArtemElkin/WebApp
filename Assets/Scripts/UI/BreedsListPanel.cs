using System.Collections.Generic;
using UnityEngine;

public class BreedsListPanel : MonoBehaviour
{
    [SerializeField] private List<BreedDetailsBtn> _breedBtnViewList;
    [SerializeField] private BreedDetailsBtn _breedDetailsBtnPrefab;
    [SerializeField] private DogApiService _api;
    [SerializeField] private ClassicPopup _popup;


    public void CreateButton(string breedId, string breedName)
    {
        BreedDetailsBtn btn = Instantiate(_breedDetailsBtnPrefab, transform);
        _breedBtnViewList.Add(btn);
        btn.Init(breedId, breedName, _breedBtnViewList.Count, _api, _popup);
    }

    public void ClearList()
    {
        while (_breedBtnViewList.Count > 0)
        {
            _breedBtnViewList[0].gameObject.SetActive(false);
            _breedBtnViewList.RemoveAt(0);
        }
    }

    public void CancelAllLoaders(string exceptionBreedId)
    {
        foreach (var btn in _breedBtnViewList)
        {
            if (btn._breedId != exceptionBreedId)
                btn.CancelLoading();
        }
    }
    
    public void CancelAllLoaders()
    {
        foreach (var btn in _breedBtnViewList)
        {
            btn.CancelLoading();
        }
    }
}
