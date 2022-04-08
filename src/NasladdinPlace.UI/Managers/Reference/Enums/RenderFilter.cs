namespace NasladdinPlace.UI.Managers.Reference.Enums
{
    public enum RenderFilter
    {
        /// <summary>
        /// Отрисовать фильтры в заголовке таблицы
        /// </summary>
        InGridHeader,

        /// <summary>
        /// Отрисовать фильтры в панеле фильтров (вместе с дополнительными фильтрами)
        /// </summary>
        WithAdditionFilters,

        /// <summary>
        /// Не отрисовывать фильтры (не влияет на дополнительные фильтры)
        /// </summary>
        NotRender,
    }
}
