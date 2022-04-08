using System;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using NasladdinPlace.Spreadsheets.Services.Credential.Contracts;

namespace NasladdinPlace.Spreadsheets.Services.Credential
{
    public class GoogleServiceAccountCredential : IGoogleCredential
    {
        public JsonCredentialParameters CredentialParameters { get; set; }
        public IEnumerable<string> Scopes { get; set; }

        public GoogleServiceAccountCredential(JsonCredentialParameters credentialParameters, IEnumerable<string> scopes)
        {
            CredentialParameters = credentialParameters;
            Scopes = scopes;
        }

        public ServiceAccountCredential CreateServiceAccountCredential()
        {
            if (CredentialParameters.Type != JsonCredentialParameters.ServiceAccountCredentialType ||
                string.IsNullOrEmpty(CredentialParameters.ClientEmail) ||
                string.IsNullOrEmpty(CredentialParameters.PrivateKey))
            {
                throw new InvalidOperationException("JSON data does not represent a valid service account credential.");
            }

            var initializer = new ServiceAccountCredential.Initializer(CredentialParameters.ClientEmail)
            {
                Scopes = Scopes
            };

            return new ServiceAccountCredential(initializer.FromPrivateKey(CredentialParameters.PrivateKey));
        }
    }
}
