using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.LabeledGood
{
    [DataContract]
    public class LabeledGoodPartnerDto : BaseLabeledGoodDto
    {
        [DataMember(Name = "manufactureDate")]
        public long? ManufactureDate { get; set; }

        [DataMember(Name = "expirationDate")]
        public long? ExpirationDate { get; set; }

        [DataMember(Name = "canBeDeleted")]
        public bool? CanBeDeleted { get; set; }

        [DataMember(Name = "cannotBeDeletedReason")]
        public string CannotBeDeletedReason { get; set; }
    }
}
