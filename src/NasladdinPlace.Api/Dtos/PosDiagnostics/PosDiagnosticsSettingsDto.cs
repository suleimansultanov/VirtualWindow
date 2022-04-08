using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using NasladdinPlace.Core.Services.PosDiagnostics.Models;

namespace NasladdinPlace.Api.Dtos.PosDiagnostics
{
    [DataContract]
    public class PosDiagnosticsSettingsDto
    {
        [Required]
        [DataMember(Name = "posDiagnosticsType")]
        public PosDiagnosticsType? PosDiagnosticsType { get; set; }
    }
}