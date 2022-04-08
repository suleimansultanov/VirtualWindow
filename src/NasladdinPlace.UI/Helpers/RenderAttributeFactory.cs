using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NasladdinPlace.UI.Helpers
{
    /// <summary>
    /// Фабрика по созданию RenderAttribute по умолчанию, используется для свойств у которых нету RenderAttribute
    /// </summary>
    public class RenderAttributeFactory
    {
        private readonly Dictionary<Type, Func<RenderAttribute>> _defaultRenderAttribute = new Dictionary<Type, Func<RenderAttribute>>
        {
            { typeof(DateTime), () => new RenderAttribute {Control = RenderControl.Date} },
            { typeof(decimal), () => new RenderAttribute {Control = RenderControl.Decimal} },
            { typeof(int), () => new RenderAttribute {Control = RenderControl.Integer} },
            { typeof(long), () => new RenderAttribute {Control = RenderControl.Integer} },
            { typeof(bool), () => new RenderAttribute {Control = RenderControl.YesNo} },
            { typeof(bool?), () => new RenderAttribute {Control = RenderControl.YesNoEmpty} },
        };

        public RenderAttribute Create(PropertyInfo propertyInfo, RenderAttribute defRenderParams = null)
        {
            Func<RenderAttribute> getAttr;
            var attr = _defaultRenderAttribute.TryGetValue(propertyInfo.PropertyType, out getAttr)
                ? getAttr()
                : new RenderAttribute();

            if (defRenderParams != null)
            {
                attr.FilterState = defRenderParams.FilterState;
                attr.DisplayType = defRenderParams.DisplayType;
            }

            return attr;
        }
    }
}
