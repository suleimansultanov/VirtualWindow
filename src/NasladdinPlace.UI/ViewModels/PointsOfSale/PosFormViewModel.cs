using Microsoft.AspNetCore.Mvc.Rendering;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.UI.Dtos.Pos;
using NasladdinPlace.Utilities.ValidationAttributes.Basic;
using NasladdinPlace.Utilities.ValidationAttributes.Number;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Localization = NasladdinPlace.UI.Resources.ViewModels.Pos;

namespace NasladdinPlace.UI.ViewModels.PointsOfSale
{
    public class PosFormViewModel
    {
        private const string AddPosAction = "AddPos";
        private const string EditPosAction = "EditPos";

        public SelectList CitySelectList { get; set; }
        public SelectList ScreenResolutionSelectList { get; set; }
        public SelectList SensorControllerTypeSelectList { get; set; }
        public SelectList PosActivityStatusSelectList { get; set; }
        public MultiSelectList RolesMultiSelectList{ get; set; }

        public int Id { get; set; }

        [LocalizedRequired]
        [Display(Name = "City", ResourceType = typeof(Localization.PosFormViewModel))]
        public int? CityId { get; set; }

        [LocalizedRequired]
        [DoubleValue]
        [Display(Name = "Longitude", ResourceType = typeof(Localization.PosFormViewModel))]
        public string Longitude { get; set; }

        [LocalizedRequired]
        [DoubleValue]
        [Display(Name = "Latitude", ResourceType = typeof(Localization.PosFormViewModel))]
        public string Latitude { get; set; }

        [LocalizedRequired]
        [LocalizedRange(0, int.MaxValue)]
        [Display(Name = "Resolution", ResourceType = typeof(Localization.PosFormViewModel))]
        public int? RequiredScreenResolutionType { get; set; }

        [LocalizedRequired]
        [LocalizedRange(0, int.MaxValue)]
        [Display(Name = "SensorControllerType", ResourceType = typeof(Localization.PosFormViewModel))]
        public int? SensorControllerType { get; set; }

        [LocalizedRequired]
        [LocalizedRange(0, int.MaxValue)]
        [Display(Name = "PosActivityStatus", ResourceType = typeof(Localization.PosFormViewModel))]
        public int? PosActivityStatus { get; set; }

        [LocalizedRequired]
        [Display(Name = "Street", ResourceType = typeof(Localization.PosFormViewModel))]
        public string Street { get; set; }

        [Display(Name = "AccurateLocation", ResourceType = typeof(Localization.PosFormViewModel))]
        public string AccurateLocation { get; set; }

        [LocalizedRequired]
        [LocalizedStringLength(255)]
        [Display(Name = "Name", ResourceType = typeof(Localization.PosFormViewModel))]
        public string Name { get; set; }

        [LocalizedRequired]
        [LocalizedStringLength(50)]
        [Display(Name ="AbbreviatedName" , ResourceType = typeof(Localization.PosFormViewModel))]
        public string AbbreviatedName { get; set; }

        public PosStatus? PosStatus { get; set; }
        
        public List<SelectableOperationModeViewModel> SelectablePosModes { get; set; }

        public ICollection<int> SelectedRoles { get; set; }
        
        public ICollection<PosMode> AllowedModes { get; set; }
        
        public bool IsViewingPosModesAllowed { get; set; }

        public bool AreNotificationsEnabled { get; set; }

        public bool UseLegacySensorsPosition { get; set; }

        public bool IsRestrictedAccess { get; set; }

        [LocalizedRequired]
        public PosQrCodeGenerationType? QrCodeGenerationType { get; set; }

        public PosFormViewModel()
        {
            SelectablePosModes = new List<SelectableOperationModeViewModel>();
            AllowedModes = new Collection<PosMode>();
            SelectedRoles = new Collection<int>();
        }

        public string Header => Id == 0
            ? "Create Plant"
            : "Edit Plant";

        public string Action => Id == 0
            ? AddPosAction
            : EditPosAction;

        public bool IsEditAction => Action == EditPosAction;
    }
}
