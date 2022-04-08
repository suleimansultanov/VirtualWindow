using NasladdinPlace.UI.Dtos.City;
using NasladdinPlace.UI.Dtos.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.UI.Dtos.Pos
{
    public class PosDto : ICommonHandbook
    {
        public CityDto City { get; set; }

        public int Id { get; set; }

        [Required]
        public int? CityId { get; set; }

        [Required]
        public double? Longitude { get; set; }

        [Required]
        public double? Latitude { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public PosStatus? Status { get; set; }

        [Required]
        public bool? IsNotDeactivated { get; set; }

        [Required]
        public PosActivityStatus? PosActivityStatus { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string AbbreviatedName { get; set; }

        [Required]
        public bool? AreNotificationsEnabled { get; set; }

        public IEnumerable<string> IpAddresses { get; set; }

        public string Version { get; set; }
    }
}
