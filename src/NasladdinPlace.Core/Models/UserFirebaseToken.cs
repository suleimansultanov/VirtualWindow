using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models
{
    public class UserFirebaseToken : Entity
    {
        public Brand Brand { get; private set;  }
        public string Token { get; private set;  }
        public int UserId { get; private set;  }

        protected UserFirebaseToken()
        {
            // required for EF
        }

        public UserFirebaseToken(Brand brand, string token, int userId) 
            : this()
        {
            Brand = brand;
            Token = token;
            UserId = userId;
        }

        public void UpdateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return;
            
            Token = token;
        }
    }
}