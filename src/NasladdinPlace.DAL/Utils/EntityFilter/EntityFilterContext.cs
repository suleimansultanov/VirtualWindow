using System;
using System.Reflection;
using NasladdinPlace.Core.Models.Reference;

namespace NasladdinPlace.DAL.Utils.EntityFilter
{
    public class EntityFilterContext
    {
        private PropertyInfo _propertyInfo;

        public FilterItemModel ItemModel { get; set; }
        public Type Type { get; set; }
        public string Property { get; set; }
        public bool IsPropertyAndItsValueExist => !string.IsNullOrEmpty(ItemModel.Value) && _propertyInfo != null;

        public EntityFilterContext(FilterItemModel itemModel, Type entityType)
        {
            ItemModel = itemModel;
            Type = entityType;

            DefinePropertyInfo();
        }

        private void DefinePropertyInfo()
        {
            Property = ItemModel.PropertyName;
            _propertyInfo = PropertyInfoGenerator.GetValidProperty(ItemModel.PropertyName, Type);
            if (_propertyInfo == null)
            {
                Property = ItemModel.Sort;
                _propertyInfo = PropertyInfoGenerator.GetValidProperty(ItemModel.Sort, Type);
            }
        }
    }
}