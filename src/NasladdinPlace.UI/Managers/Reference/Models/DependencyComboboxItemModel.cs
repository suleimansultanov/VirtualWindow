using NasladdinPlace.UI.Managers.Reference.Interfaces;

namespace NasladdinPlace.UI.Managers.Reference.Models
{
    public class DependencyComboboxItemModel : ComboboxItemModel, IDependencyComboboxViewModel
    {
        public object Dependency { get; set; }

        public object GetDependency()
        {
            return Dependency;
        }

        public DependencyComboboxItemModel()
        {
        }

        public DependencyComboboxItemModel(object value, string text, object dependency) : base(value, text)
        {
            Dependency = dependency;
        }
    }
}
