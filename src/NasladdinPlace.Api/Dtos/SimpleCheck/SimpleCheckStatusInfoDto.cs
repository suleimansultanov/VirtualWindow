using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Dtos.SimpleCheck
{
    public class SimpleCheckStatusInfoDto
    {
        public SimpleCheckStatus Status { get; set; }
        public DateTime DateModified { get; set; }
    }
}
