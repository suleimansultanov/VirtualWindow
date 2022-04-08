using Google.Apis.Auth.OAuth2;

namespace NasladdinPlace.Api.Services.Spreadsheet.Models
{
    public class GoogleServiceAccountParameters
    {
        public JsonCredentialParameters CredentialParameters { get; }
        public string ApplicationName { get; }
        public string[] Scopes { get; }

        public GoogleServiceAccountParameters(JsonCredentialParameters credentialParameters, string applicationName,
            params string[] scopes)
        {
            CredentialParameters = credentialParameters;
            ApplicationName = applicationName;
            Scopes = scopes;
        }
    }
}