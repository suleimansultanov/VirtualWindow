using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Api.Dtos.City;
using NasladdinPlace.Api.Dtos.PosImage;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Api.Dtos.Pos
{
    public class PosDto : ICommonHandbook
    {
        public ICollection<PosImageDto> Images { get; set; }
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

        public PosConnectionStatus? Status { get; set; }

        public PosActivityStatus? PosActivityStatus { get; set; }

        public bool? IsNotDeactivated { get; set; }

        public IEnumerable<string> IpAddresses { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string AbbreviatedName { get; set; }

        [Required]
        public bool? AreNotificationsEnabled { get; set; }

        public double? DistanceInKmRelativeToUser { get; set; }

        public DoorsState DoorsState { get; set; }
        
        public string Version { get; set; }

        public PosDto()
        {
            Images = new Collection<PosImageDto>();
            IpAddresses = new Collection<string>();
        }
    }
}
