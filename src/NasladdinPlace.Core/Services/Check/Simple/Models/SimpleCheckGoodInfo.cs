using System;

namespace NasladdinPlace.Core.Services.Check.Simple.Models
{
    public class SimpleCheckGoodInfo
    {
        private string _goodDescription;
        
        public int GoodId { get; }
        public string GoodName { get; }

        public string GoodDescription
        {
            get => _goodDescription;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) return;

                _goodDescription = value;
            }
        }

        public SimpleCheckGoodInfo(int goodId, string goodName)
        {
            if (string.IsNullOrWhiteSpace(goodName))
                throw new ArgumentNullException(nameof(goodId));
            
            GoodId = goodId;
            GoodName = goodName;
            _goodDescription = string.Empty;
        }
    }
}