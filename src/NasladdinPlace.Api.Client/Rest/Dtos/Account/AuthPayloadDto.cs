using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Client.Rest.Dtos.Account
{
    [DataContract]
    public class AuthPayloadDto
    {
        [DataMember(Name = "access_token")]
        public string Token { get; set; }

        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }
    }
}
