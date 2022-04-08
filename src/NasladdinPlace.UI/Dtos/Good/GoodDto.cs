using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.UI.Dtos.LabeledGood;
using NasladdinPlace.UI.Dtos.Maker;
using NasladdinPlace.UI.Dtos.Shared;

namespace NasladdinPlace.UI.Dtos.Good
{
    public sealed class GoodDto : ICommonHandbook, IEquatable<GoodDto>
    {
        public MakerDto Maker { get; set; }

        public int Id { get; set; }

        [Required]
        public int? MakerId { get; set; }

        [Required]
        public int? GoodCategoryId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(2000)]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public double? Volume { get; set; }

        [Range(0, double.MaxValue)]
        public double? NetWeight { get; set; }

        public decimal Price { get; set; }
        public string Currency { get; set; }

        public ICollection<LabeledGoodDto> LabeledGoods { get; set; }

        public GoodDto()
        {
            LabeledGoods = new Collection<LabeledGoodDto>();
        }

        public bool Equals(GoodDto other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GoodDto) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
