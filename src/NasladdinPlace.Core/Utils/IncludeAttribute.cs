using System;

namespace NasladdinPlace.Core.Utils
{
    /// <summary>
    /// Атрибут для дополнительной подгрузки свойств (Entity-Include)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class IncludeAttribute : Attribute
    {
    }
}
