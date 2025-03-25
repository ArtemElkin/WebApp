using System.Collections.Generic;
using UnityEngine;
using Zenject;

// Панель для отображения списка кнопок с породами собак
public class BreedsListPanel : MonoBehaviour
{
    [SerializeField] private List<BreedDetailsBtn> _breedBtnViewList; // Список активных кнопок пород
    [SerializeField] private BreedDetailsBtn _breedDetailsBtnPrefab;  // Префаб кнопки для создания новых элементов
    [Inject] private DogApiService _api;                              // Сервис для работы с API пород
    [Inject] private IPopup _popup;                                   // Интерфейс попапа для отображения деталей

    // Создаёт новую кнопку для породы и добавляет её в список
    public void CreateButton(string breedId, string breedName)
    {
        // Инстанцируем кнопку из префаба и добавляем в список
        BreedDetailsBtn btn = Instantiate(_breedDetailsBtnPrefab, transform);
        _breedBtnViewList.Add(btn);
        
        // Инициализируем кнопку с данными породы
        btn.Init(breedId, breedName, _breedBtnViewList.Count, _api, _popup);
    }

    // Очищает список кнопок, скрывая их из UI
    public void ClearList()
    {
        // Постепенно убираем все кнопки из списка
        while (_breedBtnViewList.Count > 0)
        {
            _breedBtnViewList[0].gameObject.SetActive(false);
            _breedBtnViewList.RemoveAt(0);
        }
    }

    // Отменяет загрузку для всех кнопок, кроме указанной породы
    public void CancelAllLoaders(string exceptionBreedId)
    {
        foreach (var btn in _breedBtnViewList)
        {
            if (btn._breedId != exceptionBreedId)
                btn.CancelLoading();
        }
    }
    
    // Отменяет загрузку для всех кнопок в списке
    public void CancelAllLoaders()
    {
        foreach (var btn in _breedBtnViewList)
        {
            btn.CancelLoading();
        }
    }
}