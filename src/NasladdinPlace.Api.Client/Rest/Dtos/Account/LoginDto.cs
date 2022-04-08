using Refit;

namespace NasladdinPlace.Api.Client.Rest.Dtos.Account
{
    public class LoginDto
    {
        [AliasAs("username")]
        public string UserName { get; set; }

        [AliasAs("password")]
        public string Password { get; set; }

        [AliasAs("grant_type")]
        public string GrantType => "password";

        [AliasAs("scope")]
        public string Scope => "openid+email+name+profile+roles";
    }
}
