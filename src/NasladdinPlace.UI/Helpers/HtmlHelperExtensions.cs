using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace NasladdinPlace.UI.Helpers
{
    /// <summary>
    /// Вспомогательный класс с расширениями для <see cref="HtmlHelper"/>
    /// </summary>
    public static class HtmlHelperExtensions
    {
        #region Сериализация в JSON
        public static IHtmlContent Serialize(this IHtmlHelper self, object obj)
        {
            return new HtmlString(JsonConvert.SerializeObject(obj));
        }

        public static IHtmlContent SerializeToCamel(this IHtmlHelper self, object obj)
        {
            return new HtmlString(CamelCaseJsonSerializer.SerializeObject(obj));
        }
        #endregion
    }
}
