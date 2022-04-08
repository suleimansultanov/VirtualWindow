using System;

namespace NasladdinPlace.Core.Models
{
    public class GoodParameters
    {
        private double? _volume;
        private double? _netWeight;

        public int MakerId { get; private set; }
        public int GoodCategoryId { get; private set; }

        public double? Volume {
            get => _volume;
            set
            {
                if (value.HasValue && value < 0)
                    throw new ArgumentOutOfRangeException(
                        nameof(value), value, $"{nameof(Volume)} should be greater than zero."
                    );

                _volume = value;
            }
        }

        public double? NetWeight {
            get => _netWeight;
            set
            {
                if (value.HasValue && value < 0)
                    throw new ArgumentOutOfRangeException(
                        nameof(value), value, $"{nameof(NetWeight)} should be greater than zero."
                    );

                _netWeight = value;
            }
        }
        
        public GoodParameters(int makerId, int goodCategoryId)
        {
            MakerId = makerId;
            GoodCategoryId = goodCategoryId;
        }
    }
}