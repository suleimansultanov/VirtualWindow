using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models
{
    public class Pos : Entity, ICommonHandbook
    {
        public static readonly Pos Default = new Pos(
            id: 0,
            name: "Default",
            abbreviatedName: "Def",
            address: Address.FromCityStreetAtCoordinates(0, "Street", new Location())
        );

        private string _name;
        private string _abbreviatedName;
        private string _qrCode;
        private DateTime _screenTemplateAppliedDate;
        private DateTime _posActivityStatusChangeDate;
        /// <summary>
        /// This property is for internal use only in order to support many to many relationship.
        /// </summary>
        public ICollection<AllowedPosMode> InternalAllowedModes { get; private set; }
        
        public ICollection<PosImage> Images { get; private set; }
        public ICollection<LabeledGood> LabeledGoods { get; private set; }
        public ICollection<PosOperation> Operations { get; private set; }
        public ICollection<PointsOfSaleToRole> AssignedRoles { get; private set; }
        public ICollection<PosAbnormalSensorMeasurement> PosAbnormalSensorMeasurements { get; private set; }

        public ScreenResolution? ScreenResolutionOrNull
        {
            get
            {
                if (RequiredScreenResolutionWidth.HasValue || RequiredScreenResolutionHeight.HasValue)
                    return new ScreenResolution(RequiredScreenResolutionWidth.Value,
                        RequiredScreenResolutionHeight.Value);

                return null;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                RequiredScreenResolutionWidth = value.Value.Width;
                RequiredScreenResolutionHeight = value.Value.Height;
            }
        }

        public City City { get; private set; }

        public int CityId { get; private set; }
        public double Longitude { get; private set; }
        public double Latitude { get; private set; }
        public string Street { get; private set; }
        public int? RequiredScreenResolutionHeight { get; private set; }
        public int? RequiredScreenResolutionWidth { get; private set; }
        public PosQrCodeGenerationType QrCodeGenerationType { get; set; }
        public SensorControllerType SensorControllerType { get; set; }
        public PosActivityStatus PosActivityStatus { get; set; }
        public bool UseNewPaymentSystem { get; set; }
        public bool IsRestrictedAccess { get; private set; }
        public string AccurateLocation { get; private set; }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException($"{nameof(Name)} value must not be null.");

                _name = value;
            }
        }

        public string AbbreviatedName
        {
            get => _abbreviatedName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException($"{nameof(AbbreviatedName)} value must not be null.");

                _abbreviatedName = value;
            } 
        }
        
        public string QrCode
        {
            get => _qrCode;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException($"{nameof(QrCode)} value must not be null.");

                _qrCode = value;
            } 
        }
        
        public bool AreNotificationsEnabled { get; set; }

        public PosScreenTemplate PosScreenTemplate { get; private set; }
        public int PosScreenTemplateId { get; private set; }

        public DateTime ScreenTemplateAppliedDate
        {
            get => _screenTemplateAppliedDate;
            set
            {
                if (value > DateTime.UtcNow)
                    throw new ArgumentException($"Incorrect datetime {value} of the {nameof(ScreenTemplateAppliedDate)} property.");
                _screenTemplateAppliedDate = value;
            }
        }

        public DateTime PosActivityStatusChangeDate
        {
            get => _posActivityStatusChangeDate;
            private set
            {
                if (value > DateTime.UtcNow)
                    throw new ArgumentException($"Incorrect datetime {value} of the {nameof(PosActivityStatusChangeDate)} property.");
                _posActivityStatusChangeDate = value;
            }
        }

        protected Pos()
        {
            Images = new Collection<PosImage>();
            LabeledGoods = new Collection<LabeledGood>();
            Operations = new Collection<PosOperation>();
            QrCode = Guid.NewGuid().ToString();
            InternalAllowedModes = new Collection<AllowedPosMode>();
            PosAbnormalSensorMeasurements = new Collection<PosAbnormalSensorMeasurement>();
            QrCodeGenerationType = PosQrCodeGenerationType.Static;
            AssignedRoles = new Collection<PointsOfSaleToRole>();
        }

        public Pos(
            int id,
            string name,
            string abbreviatedName,
            Address address)
            : this()
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(abbreviatedName))
                throw new ArgumentNullException(nameof(abbreviatedName));
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            Id = id;
            Name = name;
            AbbreviatedName = abbreviatedName;
            UpdateAddress(address);
        }

        public Pos(
            int id,
            string name,
            string abbreviatedName,
            Address address,
            bool useNewPaymentSystem)
            : this(id, name, abbreviatedName, address)
        {
            UseNewPaymentSystem = useNewPaymentSystem;
        }

        public void UpdateAllowedModes(IEnumerable<PosMode> modes)
        {
            if (modes == null)
                throw new ArgumentNullException(nameof(modes));
            
            var newAllowedOperationModes = modes.ToImmutableHashSet();
            
            var operationModesToRemove = InternalAllowedModes
                .Where(iaom => !newAllowedOperationModes.Contains(iaom.Mode))
                .ToImmutableList();
            
            foreach (var operationModeToRemove in operationModesToRemove)
            {
                InternalAllowedModes.Remove(operationModeToRemove);
            }

            var modesToAdd =
                newAllowedOperationModes.Where(m => InternalAllowedModes.All(iaom => iaom.Mode != m))
                .ToImmutableList();

            foreach (var mode in modesToAdd)
            {
                InternalAllowedModes.Add(new AllowedPosMode(Id, mode));
            }
        }
        
        public ICollection<PosMode> AllowedModes
        {
            get { return InternalAllowedModes.Select(m => m.Mode).ToImmutableHashSet(); }
        }

        public bool IsModeAllowed(PosMode mode)
        {
            return AllowedModes.Contains(mode);
        }

        public void DisableNotifications()
        {
            AreNotificationsEnabled = false;
        }

        public void EnableNotifications()
        {
            AreNotificationsEnabled = true;
        }

        public void UpdateAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            
            CityId = address.CityId;
            Street = address.Street;
            Latitude = address.Location.Latitude;
            Longitude = address.Location.Longitude;
            AccurateLocation = address.AccurateLocation;
        }

        public void SetRestrictedAccess(bool isRestrictedAccess)
        {
            IsRestrictedAccess = isRestrictedAccess;
        }

        public void UpdatePosScreenTemplate(int templateId)
        {
            if (templateId < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(templateId), templateId, "TemplateId must be greater or equals zero."
                );

            PosScreenTemplateId = templateId;
        }

        public void ChangeActivityStatus(PosActivityStatus posActivityStatus)
        {
            if (PosActivityStatus != posActivityStatus)
            {
                PosActivityStatus = posActivityStatus;
                PosActivityStatusChangeDate = DateTime.UtcNow;
            }
        }

        public bool IsInServiceOrInTestMode => PosActivityStatus == PosActivityStatus.Active || 
                                   PosActivityStatus == PosActivityStatus.Test;

        public bool IsInServiceMode => PosActivityStatus == PosActivityStatus.Active;
    }
}
