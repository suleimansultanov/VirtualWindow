using NasladdinPlace.UI.Dtos.Permission;
using NasladdinPlace.UI.ViewModels.Roles;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.UI.ViewModels.Security
{
    public class EditAppFeaturesViewModel
    {
        public ICollection<AppFeatureItemDto> AvailableAppFeatures { get; set; }

        public ICollection<RoleViewModel> AvailableRoles { get; set; }

        public IDictionary<string, IDictionary<int, bool>> Allowed { get; set; }

        public EditAppFeaturesViewModel()
        {
            AvailableAppFeatures = new Collection<AppFeatureItemDto>();
            AvailableRoles = new Collection<RoleViewModel>();
            Allowed = new Dictionary<string, IDictionary<int, bool>>();
        }
    }
}
