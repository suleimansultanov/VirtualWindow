using System;

namespace NasladdinPlace.DAL.Utils
{
    /// <summary>
    /// Расширение для получения имени partial представления фильтра который отображается в контролах TextReference и TextReferenceBigData 
    /// </summary>

    public static class RenderExt
    {
        public const string DynamicFilterShortDateFormat = "yyyy-MM-dd";
        public const string DynamicFilterDateFormat = "yyyy-MM-ddTHH:mm:ss";

        public static string ToDynamicFilterDateFormat(this DateTime date)
        {
            return date.ToString(DynamicFilterDateFormat);
        }

        public static DateTime? ToDynamicFilterDateFormat(this string str)
        {
            try
            {
                return DateTime.ParseExact(str, RenderExt.DynamicFilterDateFormat, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
