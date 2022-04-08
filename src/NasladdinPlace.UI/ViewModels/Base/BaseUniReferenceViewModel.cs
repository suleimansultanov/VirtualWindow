using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.UI.Helpers;
using NasladdinPlace.UI.Managers.Reference.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NasladdinPlace.UI.ViewModels.Base
{
    public interface IBaseUniReferenceViewModel
    {
        List<FormRendererItemInfo> FormRendererInfos { get; set; }
    }

    /// <summary>
    /// ViewModel содержит в себе данные о ключевых полях и их валидации
    /// </summary>
    public abstract class BaseUniReferenceViewModel<T> : IBaseUniReferenceViewModel where T : BaseViewModel
    {
        protected BaseUniReferenceViewModel()
        {
        }

        protected BaseUniReferenceViewModel(T viewModel, List<T> items)
        {
            var formRendererHelper = new FormRendererHelper();
            var infos = formRendererHelper.GetFields(viewModel.GetType()).ToList();

            AllFormRendererInfos = infos;
            FormRendererInfos = infos.GetRendererFields().ToList();
            ReferenceType = viewModel.GetType();

            ViewModel = viewModel;

            Filter = infos.GetFilterItems();

            var type = viewModel.GetType();
            foreach (var item in infos.Select(x => x.RenderInfo.GetTextReference()).Where(x => x != null && x.Contains(".")))
            {
                var propertyInfo = type.GetProperty(item.Split('.')[0]);
                propertyInfo?.SetValue(ViewModel, Activator.CreateInstance(propertyInfo.PropertyType));
            }
        }

        public IEnumerable<FormRendererItemInfo> GetFilterInfo()
        {
            return AllFormRendererInfos.GetFilterFields();
        }

        public void ClearInfo()
        {
            FormRendererInfos = null;
            AllFormRendererInfos = null;
        }

        public void RemoveInfo(string name)
        {
            var infos = AllFormRendererInfos.ToList();
            var first = infos.FirstOrDefault(x => x.Info.Name == name);
            if (first != null)
            {
                infos.Remove(first);
                AllFormRendererInfos = infos;
            }

            var info = FormRendererInfos.FirstOrDefault(x => x.Info.Name == name);
            if (info != null)
            {
                FormRendererInfos.Remove(info);
            }
        }

        [JsonIgnore]
        public List<FormRendererItemInfo> FormRendererInfos { get; set; }

        [JsonIgnore]
        public IEnumerable<FormRendererItemInfo> AllFormRendererInfos { get; set; }

        [JsonIgnore]
        public BaseViewModel ViewModel { get; set; }

        [JsonIgnore]
        public string Name { get; set; }

        public Type ReferenceType { get; set; }

        public Dictionary<string, FilterItemModel> Filter { get; set; }

        public PaginationInfo Pagination { get; set; }
    }
}
