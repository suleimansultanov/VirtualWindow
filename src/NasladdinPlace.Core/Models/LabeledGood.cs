using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models.Goods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Core.Models
{
    public class LabeledGood : Entity
    {
        public static LabeledGoodOfPosBuilder NewOfPosBuilder(int posId, string label)
        {
            return new LabeledGoodOfPosBuilder(posId, label);
        }

        public static LabeledGood OfPos(int posId, string label)
        {
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException(nameof(label), label);

            return new LabeledGood
            {
                PosId = posId,
                Label = label
            };
        }

        public static LabeledGood FromLabel(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException(nameof(label), label);

            return new LabeledGood
            {
                Label = label
            };
        }

        public Pos Pos { get; private set; }
        public Good Good { get; private set; }
        public PosOperation PosOperation { get; private set; }
        public Currency Currency { get; private set; }
        public ICollection<LabeledGoodTrackingRecord> TrackingRecords { get; private set; }

        public string Label { get; private set; }
        public int? GoodId { get; private set; }
        public int? PosId { get; private set; }
        public int? PosOperationId { get; private set; }
        public DateTime? ManufactureDate { get; private set; }
        public DateTime? ExpirationDate { get; private set; }
        public bool IsDisabled { get; private set; }
        public decimal? Price { get; private set; }
        public int? CurrencyId { get; private set; }
        public DateTime? FoundDateTime { get; private set; }
        public DateTime? LostDateTime { get; private set; }

        protected LabeledGood()
        {
            TrackingRecords = new Collection<LabeledGoodTrackingRecord>();
        }

        public void Disable()
        {
            IsDisabled = true;
        }

        public void Enable()
        {
            IsDisabled = false;
        }

        public bool IsInsidePos => PosOperationId == null && PosId != null;

        public void MarkAsInsidePos(int posId)
        {
            PosId = posId;
            PosOperationId = null;
        }

        public void MarkAsFoundInPos(int posId)
        {
            FoundDateTime = DateTime.UtcNow;
            TrackingRecords.Add(LabeledGoodTrackingRecord.ForFoundLabeledGood(Id, posId));
        }

        public void MarkAsLostInPos(int posId)
        {
            LostDateTime = DateTime.UtcNow;
            TrackingRecords.Add(LabeledGoodTrackingRecord.ForLostLabeledGood(Id, posId));
        }

        public void UpdatePrice(LabeledGoodPrice labeledGoodPrice)
        {
            if (labeledGoodPrice == null)
                throw new ArgumentNullException(nameof(labeledGoodPrice));

            Price = labeledGoodPrice.Price;
            CurrencyId = labeledGoodPrice.CurrencyId;
        }

        public void UpdateExpirationPeriod(ExpirationPeriod expirationPeriod)
        {
            if (expirationPeriod == null)
                throw new ArgumentNullException(nameof(expirationPeriod));
            if (expirationPeriod.IsExpired)
                throw new ArgumentNullException(
                    nameof(expirationPeriod),
                    "Expiration period must not be expired."
                );

            ManufactureDate = expirationPeriod.ManufactureDate;
            ExpirationDate = expirationPeriod.ExpirationDate;
        }

        public void TieToGood(Good good, LabeledGoodPrice price, ExpirationPeriod expirationPeriod)
        {
            if (good == null)
                throw new ArgumentNullException(nameof(good));

            Good = good;
            TieToGood(good.Id, price, expirationPeriod);
        }

        public void TieToGood(int goodId, LabeledGoodPrice price, ExpirationPeriod expirationPeriod)
        {
            GoodId = goodId;
            UpdatePrice(price);
            if (!expirationPeriod.IsExpired)
                UpdateExpirationPeriod(expirationPeriod);
        }

        public void UntieFromGood()
        {
            GoodId = null;
            Price = null;
            CurrencyId = null;
            ManufactureDate = null;
            ExpirationDate = null;
        }

        public void MarkAsUsedInPosOperation(int posOperationId)
        {
            if (PosId == null)
                throw new InvalidOperationException($"{nameof(PosId)} must be initialized before this method execution.");

            PosOperationId = posOperationId;
        }

        public void MarkAsUsedInPosOperation(PosOperation posOperation)
        {
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));

            if (posOperation.PosId != PosId)
                throw new ArgumentException("Pos operation pos id must be the same as labeled good's pos id.");

            MarkAsUsedInPosOperation(posOperation.Id);
        }

        public void MarkAsNotBelongingToUserOrPos()
        {
            PosId = null;
            PosOperationId = null;
        }

        public void SetId(int id)
        {
            Id = id;
        }

        public string GetImagePath()
        {
            return Good.GetGoodImagePathOrDefault();
        }

        public int GetGoodId()
        {
            return Good.Id;
        }

        public string GetMakerName()
        {
            return Good.Maker.Name;
        }

        public string GetCurrencyName()
        {
            return Currency.Name;
        }

        public string GetGoodComposition()
        {
            return Good.Composition;
        }

        public string GetGoodDescription()
        {
            return Good.Description;
        }

        public ProteinsFatsCarbohydratesCalories GetGoodNitrients()
        {
            return Good.ProteinsFatsCarbohydratesCalories;
        }

        public double? GetGoodNetWeight()
        {
            return Good.NetWeight;
        }

        public string GetGoodName()
        {
            return Good.Name;
        }

        public GoodPublishingStatus GetGoodPublishingStatus()
        {
            return Good.PublishingStatus;
        }

        public int GetLabeledGoodsCountInsidePosDistinctByPrice()
        {
            return Good.GetLabeledGoodsCountInsidePosAndHasEqualPrices(this);
        }

        public bool IsNotDisabledAndInsidePos => !IsDisabled && IsInsidePos;
    }
}