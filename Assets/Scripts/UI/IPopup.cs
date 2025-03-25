// Интерфейс для управления отображением попапов с информацией
public interface IPopup
{
    // Показывает попап с названием и описанием породы
    public void Show(string breedname, string breedinfo);
    
    // Скрывает попап
    public void Hide();
}