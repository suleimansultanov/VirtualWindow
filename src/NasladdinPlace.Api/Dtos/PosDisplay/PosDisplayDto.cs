using System.Runtime.Serialization;
using NasladdinPlace.Api.Dtos.Check;
using NasladdinPlace.Api.Dtos.CommandDelivery;
using NasladdinPlace.Api.Dtos.QrCode;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Dtos.PosDisplay
{
    [DataContract]
    public class PosDisplayDto
    {
        [DataMember(Name = "contentType")]
        public PosDisplayContentType ContentType { get; }
        
        [DataMember(Name = "content")]
        public object Content { get; }

        public PosDisplayDto(CheckDto checkDto)
        {
            ContentType = PosDisplayContentType.Check;
            Content = checkDto;
        }

        public PosDisplayDto(QrCodeDto qrCodeDto)
        {
            ContentType = PosDisplayContentType.QrCode;
            Content = qrCodeDto;
        }

        public PosDisplayDto(PosDisplayContentType contentType, object content)
        {
            ContentType = contentType;
            Content = content;
        }

        public PosDisplayDto(PosDisplayContentType contentType, PosDisplayCommandDto command)
        {
            ContentType = contentType;
            Content = command;
        }
    }
}