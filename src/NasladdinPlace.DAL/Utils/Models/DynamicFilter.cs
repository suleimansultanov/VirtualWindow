namespace NasladdinPlace.DAL.Utils.Models
{
    /// <summary>
    /// Модель для динамической фильтрации
    /// </summary>
    public class DynamicFilter
    {
        public string Predicate { get; set; }
        public object[] Params { get; set; }
        public bool HasFilter => Params.Length > 0;
        public string Sort { get; set; }
        public bool HasSort => !string.IsNullOrEmpty(Sort);
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
