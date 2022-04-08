using System;

namespace NasladdinPlace.Utilities.EnumHelpers
{
    /// <summary>
    /// Атрибут, который используется для описания отдельного enum
    /// </summary>
    public class EnumDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Описание объекта
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Создание нового атрибута
        /// </summary>
        /// <param name="text">Описание объекта</param>
        public EnumDescriptionAttribute(string text)
        {
            Text = text;
        }
    }


    public static class EnumDescriptionAttributeExt
    {
        public static string GetDisplayName<T>(this T source)
        {
            var type = source.GetType();
            if (!type.IsEnum) throw new ArgumentException("Тип параметра не является типом перечесления");
            var fi = type.GetField(source.ToString());
            var attributes = (EnumDescriptionAttribute[])fi?.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
            return attributes != null && attributes.Length > 0 ? attributes[0].Text : source.ToString();
        }
    }
}
