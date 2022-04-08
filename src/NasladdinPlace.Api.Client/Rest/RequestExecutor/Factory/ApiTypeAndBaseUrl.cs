using System;

namespace NasladdinPlace.Api.Client.Rest.RequestExecutor.Factory
{
    public sealed class ApiTypeAndBaseUrl : IEquatable<ApiTypeAndBaseUrl>
    {
        public Type Type { get; }
        public string BaseApiUrl { get; }

        public ApiTypeAndBaseUrl(Type type, string baseApiUrl)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (baseApiUrl == null)
                throw new ArgumentNullException(nameof(baseApiUrl));

            Type = type;
            BaseApiUrl = baseApiUrl;
        }
        
        public bool Equals(ApiTypeAndBaseUrl other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && string.Equals(BaseApiUrl, other.BaseApiUrl);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ApiTypeAndBaseUrl other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (BaseApiUrl != null ? BaseApiUrl.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ApiTypeAndBaseUrl left, ApiTypeAndBaseUrl right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ApiTypeAndBaseUrl left, ApiTypeAndBaseUrl right)
        {
            return !Equals(left, right);
        }
    }
}