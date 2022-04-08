using System.Reflection;
using NasladdinPlace.UI.Managers.Reference.Attributes;

namespace NasladdinPlace.UI.Managers.Reference.Models
{
    /// <summary>
    /// Модель для информации по рендерингу
    /// </summary>
    public class FormRendererItemInfo
    {
        public PropertyInfo Info { get; set; }

        public RenderAttribute RenderInfo { get; set; }

        public string ColumnTitle => RenderInfo.DisplayName;
    }
}
