using System.Runtime.Serialization;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Dtos.PosLogs
{
    [DataContract]
    public class RequestPosLogDto
    {
        [DataMember(Name = "logType")]
        public PosLogType LogType { get; }

        public RequestPosLogDto(PosLogType logType)
        {
            LogType = logType;
        }
    }
}
