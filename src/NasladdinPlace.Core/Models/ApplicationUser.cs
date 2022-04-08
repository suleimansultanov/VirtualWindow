using Microsoft.AspNetCore.Identity;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Payment.Card;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

namespace NasladdinPlace.Core.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public ICollection<PosOperation> PosOperations { get; private set; }
        public ICollection<PaymentCard> PaymentCards { get; private set; }
        public ICollection<UserBonusPoint> BonusPoints { get; private set; }
        public ICollection<UserFirebaseToken> FirebaseTokens { get; private set; }
        public ICollection<PromotionLog> PromotionLogs { get; private set; }
        public ICollection<CheckItemAuditRecord> CheckItemsAuditHistory { get; private set; }
        public ICollection<UserRole> UserRoles { get; private set; }

        public PaymentCard ActivePaymentCard { get; private set; }

        [Obsolete("This field is deprecated and will be removed in the future versions.")]
        public Gender Gender { get; set; }

        public DateTime Birthdate { get; set; }
        public string FullName { get; set; }
        public string ChangePhoneNumberTokenRemainder { get; private set; }
        public int? ActivePaymentCardId { get; private set; }
        public DateTime RegistrationInitiationDate { get; private set; }
        public DateTime? PaymentCardVerificationInitiationDate { get; private set; }
        public DateTime? PaymentCardVerificationCompletionDate { get; private set; }
        public DateTime? RegistrationCompletionDate { get; private set; }
        public bool IsActive { get; private set; }
        public GoalType? Goal { get; set; }
        public int? Age { get; private set; }
        public int? Height { get; private set; }
        public int? Weight { get; private set; }
        public ActivityType? Activity { get; set; }
        public PregnancyType? Pregnancy { get; set; }

        public ApplicationUser()
        {
            PosOperations = new Collection<PosOperation>();
            PaymentCards = new Collection<PaymentCard>();
            BonusPoints = new Collection<UserBonusPoint>();
            FirebaseTokens = new Collection<UserFirebaseToken>();
            PromotionLogs = new Collection<PromotionLog>();
            CheckItemsAuditHistory = new Collection<CheckItemAuditRecord>();
            RegistrationInitiationDate = DateTime.UtcNow;
            UserRoles = new Collection<UserRole>();
        }

        public decimal TotalBonusPoints { get; private set; }

        public string ShortenChangePhoneTokenAndSaveRemainder(string phoneNumberToken)
        {
            if (string.IsNullOrEmpty(phoneNumberToken) || phoneNumberToken.Length <= 4)
            {
                ChangePhoneNumberTokenRemainder = string.Empty;
                return string.Empty;
            }

            ChangePhoneNumberTokenRemainder = phoneNumberToken.Substring(4);

            return phoneNumberToken.Substring(0, 4);
        }

        public string GetFullChangePhoneNumberToken(string shortPhoneNumberToken)
        {
            return string.Concat(shortPhoneNumberToken, ChangePhoneNumberTokenRemainder);
        }

        public void CreateOrUpdateFirebaseToken(Brand brand, string token)
        {
            var firebaseToken = FindFirebaseTokenByBrand(brand);

            if (firebaseToken == null)
            {
                firebaseToken = new UserFirebaseToken(brand, token, Id);
                FirebaseTokens.Add(firebaseToken);
            }
            else
            {
                firebaseToken.UpdateToken(token);
            }
        }

        public void NotifyBankingCardVerificationInitiation()
        {
            PaymentCardVerificationInitiationDate = DateTime.UtcNow;
        }

        public void NotifyRegistrationCompletion()
        {
            RegistrationCompletionDate = DateTime.UtcNow;
        }

        public UserFirebaseToken FindFirebaseTokenByBrand(Brand brand)
        {
            return FirebaseTokens.SingleOrDefault(ft => ft.Brand == brand);
        }

        public void ResetActivePaymentCard()
        {
            ActivePaymentCardId = null;
        }

        public void SetActivePaymentCard(ExtendedPaymentCardInfo extendedPaymentCardInfo)
        {
            if (extendedPaymentCardInfo == null)
                throw new ArgumentNullException(nameof(extendedPaymentCardInfo));

            var paymentSystem = extendedPaymentCardInfo.PaymentSystem;
            var cryptogramSource = extendedPaymentCardInfo.CryptogramSource;
            var cardToken = extendedPaymentCardInfo.Token;

            var paymentCards = PaymentCards.Where(pc =>
                pc.PaymentSystem == paymentSystem &&
                pc.CryptogramSource == cryptogramSource &&
                pc.Status == PaymentCardStatus.AbleToMakePayment
            ).ToImmutableList();

            MarkPreviousPaymentCardsNotAbleToMakePayment(paymentCards, cardToken, extendedPaymentCardInfo);

            if (!paymentCards.Any() ||
                 cryptogramSource == PaymentCardCryptogramSource.Common && paymentCards.All(pc => pc.Token != cardToken))
            {
                var paymentCard = new PaymentCard(
                    userId: Id,
                    extendedPaymentCardInfo: extendedPaymentCardInfo
                );
                PaymentCards.Add(paymentCard);

                ActivePaymentCard = paymentCard;

                ActivePaymentCard.MarkAsAbleToMakePayment();
            }
            else if (cryptogramSource == PaymentCardCryptogramSource.Common)
            {
                ActivePaymentCard = paymentCards.First(pc => pc.Token == cardToken);
                ActivePaymentCard.UpdateInfo(extendedPaymentCardInfo);
            }
            else
            {
                ActivePaymentCard = paymentCards.First();
                ActivePaymentCard.UpdateInfo(extendedPaymentCardInfo);
            }

            NotifyPaymentCardConfirmationCompletion();
        }

        public void SetActivePaymentCard(int paymentCardId)
        {
            var paymentCard = PaymentCards.SingleOrDefault(pc => pc.Id == paymentCardId);

            if (paymentCard == null)
                throw new InvalidOperationException($"Payment card with id = {paymentCardId} does not exists.");

            if (paymentCard.Status != PaymentCardStatus.AbleToMakePayment)
                throw new InvalidOperationException($"Payment card with id = {paymentCardId} has incorrect status.");

            ActivePaymentCardId = paymentCardId;
            ActivePaymentCard = paymentCard;

            NotifyPaymentCardConfirmationCompletion();
        }

        public void NotifyPaymentCardConfirmationCompletion()
        {
            PaymentCardVerificationCompletionDate = DateTime.UtcNow;
        }

        public void AddBonusPoints(decimal value, BonusType type)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(value), value, "The value to be added must be greater than zero."
                );

            if (value == 0) return;

            AddBonusPointsHistoryRecord(value, type);

            TotalBonusPoints += value;
        }

        public void SubtractBonusPoints(decimal value, BonusType type)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(value), value, "Subtrahend value must be greater zero."
                );

            var subtrahendBonusValue = TotalBonusPoints - value;
            if (subtrahendBonusValue < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "Total bonus must be greater or equal than subtrahend value." +
                    $"However, found total value is {TotalBonusPoints}, subtrahend value is {value}."
                );

            if (value == 0) return;

            AddBonusPointsHistoryRecord(-value, type);

            TotalBonusPoints = TotalBonusPoints - value > 0 ? TotalBonusPoints - value : 0;
        }

        public decimal CalculateBonusPointsAmountThatCanBeWrittenOff(decimal amountViaMoney)
        {
            return TotalBonusPoints > amountViaMoney ? amountViaMoney : TotalBonusPoints;
        }

        public bool HasBirthDate()
        {
            return Birthdate != DateTime.MinValue;
        }

        public void MarkAsCompletedRegistration()
        {
            RegistrationCompletionDate = DateTime.UtcNow;
        }

        public void SetUserActivity(bool isActive)
        {
            IsActive = isActive;
        }

        private void AddBonusPointsHistoryRecord(decimal value, BonusType type)
        {
            var bonusHistoryRecord = new UserBonusPoint(Id, value, type);

            BonusPoints.Add(bonusHistoryRecord);
        }

        private void MarkPreviousPaymentCardsNotAbleToMakePayment(ImmutableList<PaymentCard> paymentCards, 
            string cardToken, 
            ExtendedPaymentCardInfo extendedPaymentCardInfo)
        {
            paymentCards.Where(pc => pc.Token != cardToken && pc.HasNumber &&
                                     pc.FirstSixDigits == extendedPaymentCardInfo.Number.FirstSixDigits &&
                                     pc.LastFourDigits == extendedPaymentCardInfo.Number.LastFourDigits &&
                                     pc.ExpirationDate == extendedPaymentCardInfo.ExpirationDate)
                .ToList()
                .ForEach(paymentCard =>
                {
                    if (paymentCard.CryptogramSource == PaymentCardCryptogramSource.Common)
                        paymentCard.MarkAsNotAbleToMakePayment();
                });
        }

        #region Nutrients

        public void SetAge(int? age)
        {
            if (age == null || age == 0) return;

            if (age < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(age), age, "The age must be greater than zero."
                );

            Age = age;
        }

        public void SetWeight(int? weight)
        {
            if (weight == null || weight == 0) return;

            if (weight < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(weight), weight, "The weight must be greater than zero."
                );

            Weight = weight;
        }

        public void SetHeight(int? height)
        {
            if (height == null || height == 0) return;

            if (height < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(height), height, "The height must be greater than zero."
                );

            Height = height;
        }

        public bool IsGoalDecreasingWeightWithoutActivity =>
            Goal.HasValue && Activity.HasValue && Activity.Value == ActivityType.Zero && Goal.Value == GoalType.DecreaseWeight;

        public bool IsGoalDecreasingWeightWithActivity =>
            Goal.HasValue && Activity.HasValue && Activity.Value != ActivityType.Zero && Goal.Value == GoalType.DecreaseWeight;

        public bool CanCalculateNutrientsByUserParams => Goal.HasValue && Activity.HasValue &&
                                                         Weight.HasValue && Height.HasValue &&
                                                         Age.HasValue && Pregnancy.HasValue &&
                                                         Gender != Gender.Undefined &&
                                                         (Gender == Gender.Female || Gender == Gender.Male);

        #endregion
    }
}
