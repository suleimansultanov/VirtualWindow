using System;

namespace NasladdinPlace.Core.Models
{
    public class Currency : Entity, ICommonHandbook
    {
        public static readonly Currency Unknown = new Currency(0, "Unknown", "Unknown");
        
        public static readonly Currency Ruble = new Currency(1, "руб", "RUB");
        
        public string Name { get; private set; }
        public string IsoCode { get; private set; }

        protected Currency()
        {
            // required for EF
        }
        
        public Currency(int id, string name, string isoCode)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(isoCode))
                throw new ArgumentNullException(nameof(isoCode));
            
            Id = id;
            Name = name;
            IsoCode = isoCode;
        }
    }
}
