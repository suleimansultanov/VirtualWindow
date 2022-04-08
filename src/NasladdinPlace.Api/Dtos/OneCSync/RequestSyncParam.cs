using System;

namespace NasladdinPlace.Api.Dtos.OneCSync
{
    public class RequestSyncParam
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}