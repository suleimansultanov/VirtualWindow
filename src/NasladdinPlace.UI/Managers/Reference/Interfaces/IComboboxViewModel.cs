namespace NasladdinPlace.UI.Managers.Reference.Interfaces
{
    /// <summary>
    /// Интерфейс для модели которая может быть представлена в виде выпадающего списка
    /// </summary>
    public interface IComboboxViewModel
    {

        /// <summary>
        /// Получить текстовое представление
        /// </summary>
        /// <returns></returns>
        string GetText();

        /// <summary>
        /// Получить значение
        /// </summary>
        /// <returns></returns>
        object GetValue();
    }
}
