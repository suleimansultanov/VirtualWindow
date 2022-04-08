using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.QrCode
{
    [DataContract]
    public class QrCodeDto
    {
        [DataMember(Name = "qrCode")]
        [Required]
        public string QrCode { get; set; }

        [DataMember(Name = "commandId")]
        public Guid CommandId { get; set; }

        public QrCodeDto(string qrCode, Guid commandId)
        {
            QrCode = qrCode;
            CommandId = commandId;
        }
    }
}