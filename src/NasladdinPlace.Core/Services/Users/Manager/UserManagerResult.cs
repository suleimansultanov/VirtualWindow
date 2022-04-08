using System.Collections.Generic;
using System.Linq;

namespace NasladdinPlace.Core.Services.Users.Manager
{
    public class UserManagerResult
    {
        public static UserManagerResult Success()
        {
            return new UserManagerResult(true, Enumerable.Empty<string>());
        }

        public static UserManagerResult Failure(IEnumerable<string> errors)
        {
            return new UserManagerResult(false, errors ?? Enumerable.Empty<string>());
        }

        public static UserManagerResult Failure()
        {
            return Failure(Enumerable.Empty<string>());
        }
        
        public bool Succeeded { get; }
        public IEnumerable<string> Errors { get; }

        private UserManagerResult(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors;
        }
    }
}