using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.CommandDelivery
{
    [DataContract]
    public class ConfirmCommandDeliveryDto
    {
        [Required]
        [DataMember(Name = "commandId")]
        public Guid CommandId { get; set; }
    }
}
