using Google.Apis.Auth.OAuth2;

namespace NasladdinPlace.Spreadsheets.Services.Credential.Contracts
{
    public interface IGoogleCredential
    {
        ServiceAccountCredential CreateServiceAccountCredential();
    }
}
