using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.Approach
{
    [DataContract]
    public class ApproachEventDto
    {
        [DataMember(Name = "time_beg")]
        public int TimeBeginningInMilliseconds { get; set; }

        [DataMember(Name = "time_end")]
        public int TimeEndInMilliseconds { get; set; }

        [DataMember(Name = "dists")]
        public int[] Distances { get; set; }
    }
}