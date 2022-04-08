using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Managers.Reference.Models;
using Newtonsoft.Json;

namespace NasladdinPlace.UI.ViewModels.Base
{
    /// <summary>
    /// ViewModel для работы универсального справочника
    /// </summary>
    public class UniReferenceViewModel<T> : GridViewModel<T> where T : BaseViewModel
    {
        public UniReferenceViewModel()
        {
        }

        public UniReferenceViewModel(T viewModel, List<T> items,  List<ReferencesModel> references) : base(viewModel, items)
        {
            References = references.ToDictionary(x => x.ReferenceType, x => x);
        }

        public Dictionary<string, ReferencesModel> References { get; set; }

        public ICollection<FilterItemModel> Context { get; set; }
    }



    public class GridViewModel<T> : BaseUniReferenceViewModel<T> where T : BaseViewModel
    {
        public GridViewModel() 
        {
        }

        public GridViewModel(T viewModel, List<T> items) : base(viewModel, items)
        {
            Items = items.ToList();
        }

        public List<T> Items { get; set; }

        public string SaveUrl { get; set; }

        public string LoadUrl { get; set; }
        
        public string Url { get; set; }

        public string ExportUrl { get; set; }

        public bool IsRenderFilter { get; set; } = true;

        public bool RenderControls { get; set; } = true;

        public List<string> ContextFields { get; set; }

        [JsonIgnore]
        public ConfigReference Configuration { get; set; }

        public object CustomData { get; set; }
    }
}
