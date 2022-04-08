using System;

namespace NasladdinPlace.Core.Models
{
    public class ExpirationPeriod
    {     
        public static ExpirationPeriod FromNowTill(DateTime expirationDate)
        {
            return new ExpirationPeriod(DateTime.UtcNow, expirationDate);
        }

        public DateTime ManufactureDate { get; }
        public DateTime ExpirationDate { get; }

        public ExpirationPeriod()
        {
            ManufactureDate = DateTime.UtcNow;
            ExpirationDate = DateTime.UtcNow.AddDays(1);
        }

        public ExpirationPeriod(DateTime manufactureDate, DateTime expirationDate)
        {
            if (manufactureDate > expirationDate)
                throw new ArgumentException("Manufacture date must be less or equal to expiration date");
            
            ManufactureDate = manufactureDate;
            ExpirationDate = expirationDate;
        }

        public bool IsExpired => ExpirationDate < DateTime.UtcNow;
    }
}