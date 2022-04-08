using System.Collections.Generic;

namespace NasladdinPlace.UI.Managers.Reference.Models
{
    /// <summary>
    /// Модель для универсального выбора из модального окна
    /// </summary>
    public class TextReferenceModel : PageInfoModel
    {
        public List<string> Headers { get; set; }
        public List<List<object>> Table { get; set; }
        public List<int> Keys { get; set; }
    }
}