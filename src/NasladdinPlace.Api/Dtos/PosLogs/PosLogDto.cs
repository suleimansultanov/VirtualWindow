using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Api.Dtos.Pos;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Dtos.PosLogs
{
    public class PosLogDto : BasePosWsMessageDto
    {
        [Required]
        public string FileName { get; set; }

        [Required]
        public string FileContent { get; set; }

        [Required]
        public PosLogType LogType { get; set; }
    }
}
