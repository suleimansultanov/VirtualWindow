using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Api.Dtos.Pos;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Dtos.PosDoorsState
{
    public class PosDoorsStateDto : BasePosWsMessageDto
    {
        [Required]
        public DoorsState DoorsState { get; set; }
    }
}