using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models.Goods
{
    public class Good : Entity
    {
        public static readonly Good Unknown = new Good
        {
            Name = "Неизвестный товар",
            Description = "Test",
            Maker = Maker.Default,
            GoodCategory = GoodCategory.Default,
            Volume = 0,
            NetWeight = 0
        };

        private string _name;
        private string _description;

        private ProteinsFatsCarbohydratesCalories _proteinsFatsCarbohydratesCalories;

        public ICollection<LabeledGood> LabeledGoods { get; private set; }
        public ICollection<GoodImage> GoodImages { get; private set; }
        public ICollection<CheckItem> CheckItems { get; private set; }
        
        public Maker Maker { get; private set; }
        public GoodCategory GoodCategory { get; private set; }

        public int MakerId { get; private set; }
        public int GoodCategoryId { get; private set; }

        public double? Volume  { get; private set; }
        public double? NetWeight { get; private set; }

        public string Composition { get; private set; }

        public GoodPublishingStatus PublishingStatus { get; private set; }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException($"{nameof(Name)} value must not be null.");

                _name = value;
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException($"{nameof(Description)} value must not be null.");

                _description = value;
            }
        }

        public ProteinsFatsCarbohydratesCalories ProteinsFatsCarbohydratesCalories
        {
            get => _proteinsFatsCarbohydratesCalories;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (_proteinsFatsCarbohydratesCalories != null)
                {
                    _proteinsFatsCarbohydratesCalories.Update(value);
                }
                else
                {
                    _proteinsFatsCarbohydratesCalories = value;
                }
            }
        }

        protected Good()
        {
            LabeledGoods = new Collection<LabeledGood>();
            GoodImages = new Collection<GoodImage>();
            CheckItems = new Collection<CheckItem>();
        }
        
        public Good(
            string name, 
            string description,
            GoodParameters goodParameters)
            : this()
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentNullException(nameof(description));
            if (goodParameters == null)
                throw new ArgumentNullException(nameof(goodParameters));

            Name = name;
            Description = description;
            UpdateGoodParameters(goodParameters);
            PublishingStatus = GoodPublishingStatus.NotPublished;
        }

        public Good(
            int id,
            string name,
            string description,
            GoodParameters goodParameters) : this(name, description, goodParameters)
        {
            Id = id;
        }

        public Good Clone()
        {
            return new Good
            {
                Id = Id,
                MakerId = MakerId,
                GoodCategoryId = GoodCategoryId,
                Name = Name,
                NetWeight = NetWeight,
                Volume = Volume,
                Description = Description
            };
        }
    
        public void AddLabeledGood(LabeledGood labeledGood)
        {
            LabeledGoods.Add(labeledGood);
        }

        public void UpdateGoodParameters(GoodParameters goodParameters)
        {
            MakerId = goodParameters.MakerId;
            GoodCategoryId = goodParameters.GoodCategoryId;
            Volume = goodParameters.Volume;
            NetWeight = goodParameters.NetWeight;
        }

        public double GetCalories()
        {
            return ProteinsFatsCarbohydratesCalories == null 
                ? 0
                : ProteinsFatsCarbohydratesCalories.CaloriesInKcal;
        }

        public double GetCarbohydrates()
        {
            return ProteinsFatsCarbohydratesCalories == null
                ? 0
                : ProteinsFatsCarbohydratesCalories.CarbohydratesInGrams;
        }

        public double GetProteins()
        {
            return ProteinsFatsCarbohydratesCalories == null
                ? 0
                : ProteinsFatsCarbohydratesCalories.ProteinsInGrams;
        }

        public double GetFats()
        {
            return ProteinsFatsCarbohydratesCalories == null
                ? 0
                : ProteinsFatsCarbohydratesCalories.FatsInGrams;
        }

        public void SetComposition(string composition)
        {
            Composition = composition;
        }

        public string GetGoodImagePath()
        {
            return GoodImages.FirstOrDefault()?.ImagePath;
        }

        public string GetGoodImagePathOrDefault()
        {
            return GoodImages.FirstOrDefault()?.ImagePath ?? GoodCategory.GetImagePath();
        }

        public int GetLabeledGoodsCountInsidePosAndHasEqualPrices(LabeledGood labeledGood)
        {
            return LabeledGoods
                .Count(lg => lg.IsNotDisabledAndInsidePos && lg.Price.Equals(labeledGood.Price));
        }

        public void SetGoodStatus(GoodPublishingStatus status)
        {
            PublishingStatus = status;
        }

        public bool IsPublished => PublishingStatus == GoodPublishingStatus.Published;
    }
}
