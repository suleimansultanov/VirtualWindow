using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Api.Dtos.Check
{
    [Obsolete("Used in order to support older versions of mobile apps. " +
              "iOS version 2.0 or lower. " +
              "Android version 1.9 or lower.")]
    public class PurchasesHistoryDto
    {
        public ICollection<CheckDto> Checks { get; set; }

        public PurchasesHistoryDto()
        {
            Checks = new Collection<CheckDto>();
        }
    }
}