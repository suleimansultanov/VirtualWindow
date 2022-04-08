using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.UI.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NasladdinPlace.UI.Controllers.Base.Models
{
    /// <summary>
    /// Параметры для получения данных UniReference
    /// </summary>
    public class ReferenceGetDataParametersModel
    {
        public ReferenceGetDataParametersModel()
        {
        }

        public ReferenceGetDataParametersModel(Type viewModelType, IEnumerable<BaseFilterItemModel> filter, PaginationInfo pagination, ICollection<Type> additionReferenceSource, bool isAutoLoadData)
            : this(viewModelType, filter, pagination, additionReferenceSource)
        {
            LoadData = isAutoLoadData;
        }

        public ReferenceGetDataParametersModel(Type viewModelType, IEnumerable<BaseFilterItemModel> filter, PaginationInfo pagination, ICollection<Type> additionReferenceSource) : this()
        {
            if (!typeof(BaseViewModel).IsAssignableFrom(viewModelType))
            {
                throw new ArgumentException("Не верный параметр тип модели отображения");
            }

            Filter = filter;
            ViewModelType = viewModelType;
            ViewModel = (BaseViewModel)Activator.CreateInstance(viewModelType);
            Pagination = pagination;
            AdditionReferenceSource = additionReferenceSource?.Select(x => x.AssemblyQualifiedName).ToList();

            EntityType = ViewModel.EntityType();
        }

        public BaseViewModel ViewModel { get; set; }

        public Type ViewModelType { get; set; }

        public Type EntityType { get; set; }

        public bool LoadData { get; set; } = true;

        public IEnumerable<BaseFilterItemModel> Filter { get; set; }

        public PaginationInfo Pagination { get; set; }

        public List<string> AdditionReferenceSource { get; set; }
    }
}
