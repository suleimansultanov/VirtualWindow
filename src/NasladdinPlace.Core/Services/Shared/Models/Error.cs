using System;

namespace NasladdinPlace.Core.Services.Shared.Models
{
    public class Error
    {
        public static readonly Error Empty = new Error("");
        
        public string Description { get; }
        public string LocalizedDescription { get; }

        private Error(string description, string localizedDescription = "")
        {
            if (description == null)
                throw new ArgumentNullException(nameof(description));

            Description = description;
            LocalizedDescription = localizedDescription;
        }

        public static Error FromDescription(string description)
        {
            return new Error(description);
        }

        public static Error FromDescriptionWithLocalization(string description, string localizedDescription)
        {
            if (localizedDescription == null)
                throw new ArgumentNullException(nameof(localizedDescription));
            
            return new Error(description, localizedDescription);
        }
    }
}