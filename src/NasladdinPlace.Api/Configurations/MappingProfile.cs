using AutoMapper;
using NasladdinPlace.Api.Dtos;
using NasladdinPlace.Api.Dtos.BankTransaction;
using NasladdinPlace.Api.Dtos.Check;
using NasladdinPlace.Api.Dtos.City;
using NasladdinPlace.Api.Dtos.Country;
using NasladdinPlace.Api.Dtos.GoodImage;
using NasladdinPlace.Api.Dtos.LabeledGood;
using NasladdinPlace.Api.Dtos.Log;
using NasladdinPlace.Api.Dtos.Maker;
using NasladdinPlace.Api.Dtos.Payment;
using NasladdinPlace.Api.Dtos.PaymentCard;
using NasladdinPlace.Api.Dtos.Pos;
using NasladdinPlace.Api.Dtos.PosImage;
using NasladdinPlace.Api.Dtos.PurchaseCompletionResult;
using NasladdinPlace.Api.Dtos.SimpleCheck;
using NasladdinPlace.Api.Dtos.User;
using NasladdinPlace.Application.Dtos.Feedback;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Feedback;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Models;
using NasladdinPlace.Core.Services.Check.CommonModels;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Core.Services.Pos.Sensors.Models;
using NasladdinPlace.Core.Services.Purchase.Completion.Models;
using NasladdinPlace.Core.Services.Purchase.Initiation.Models;
using NasladdinPlace.Core.Services.PurchasesHistoryMaker.Models;
using NasladdinPlace.Dtos;
using NasladdinPlace.Dtos.Pos;
using NasladdinPlace.Dtos.Purchase;
using NasladdinPlace.Logging.Models;
using NasladdinPlace.Payment.Models;
using NasladdinPlace.Utilities.DateTimeConverter;
using NasladdinPlace.Utilities.DateTimeConverter.Extensions;
using System;
using System.Globalization;
using System.Linq;
using NasladdinPlace.Api.Dtos.Approach;
using NasladdinPlace.Api.Dtos.Good;
using NasladdinPlace.Api.Dtos.GoodCategory;
using NasladdinPlace.Api.Dtos.ProteinsFatsCarbohydratesCalories;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Core.Models.LabeledGoods;
using Currency = NasladdinPlace.Core.Models.Currency;
using GoodDto = NasladdinPlace.Api.Dtos.Good.GoodDto;
using NasladdinPlace.Api.Dtos.MessengerContact;
using NasladdinPlace.Api.Dtos.OneCSync.Base;
using NasladdinPlace.Api.Dtos.OneCSync.Purchases;

namespace NasladdinPlace.Api.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateGoodMap();
            CreateMakerMap();
            CreateLabeledGoodMap();
            CreateSensorMeasurementsMap();
            CreatePosMap();
            CreateCountryMap();
            CreateCityMap();
            CreatePaymentCardMap();
            CreateUserMap();
            CreateGoodImageMap();
            CreateLocationMap();
            CreateLogMap();
            CreateCheckMap();
            CreateSimpleCheckMap();
            CreatePosImageMap();
            CreateBankTransactionInfoMap();
            CreateFeedbackMap();
            CreateHardToDetectLabelMap();
            CreateInfo3DsMap();
            CreateBankingCardConfirmationResultMap();
            CreatePurchaseCompletionResultMap();
            CreatePosTemperatureDetailsMap();
            CreatePurchaseInitiationResultMap();
            CreatePosRealTimeInfoMap();
            CreateGoodsMovingReportItemFromDocumentTablePartMap();
            CreateLabeledGoodPartnerInfoMap();
            CreateApproachDataMap();
            CreatePosOperationDtoMap();
            CreatePfccMap();
            CreateBankTransactionInfoOneCSyncDtoMap();
            CreateFiscalizationInfoOneCSyncDtoMap();
            CreateMessengerContactMap();
            CreateGoodCategoryMap();
            CreatePosOperationVersioniTwoDtoMap();
            CreatePosOperationTransactonDtoMap();
            CreatePosOperationTransactionCheckItemDtoDtoMap();
        }

        private void CreateGoodMap()
        {
            CreateMap<Good, GoodDto>();
            CreateMap<Good, GoodSyncDto>();
            CreateMap<CheckItem, GoodInCheckDto>()
                .ForMember(src => src.Id, opt => opt.MapFrom(chi => chi.GoodId))
                .ForMember(src => src.CheckItemId, opt => opt.MapFrom(chi => chi.Id))
                .ForMember(src => src.CheckId, opt => opt.MapFrom(chi => chi.PosOperationId))
                .ForMember(src => src.ShopId, opt => opt.MapFrom(chi => chi.PosId))
                .ForMember(src => src.Status, opt => opt.MapFrom(chi => chi.Status))
                .ForMember(src => src.Sum, opt => opt.MapFrom(chi => chi.PriceWithDiscount))
                .ForMember(src => src.SumWithoutDiscount,
                    opt => opt.MapFrom(chi => chi.Price));
            CreateMap<Good, GoodWithPfccDto>()
                .ForMember(src => src.Nutrients,
                    opts => opts.MapFrom(g => g.ProteinsFatsCarbohydratesCalories));
            CreateMap<Good, GoodWithImageAndNutrients>()
                .ForMember(src => src.Weight,
                    opts => opts.MapFrom(g => g.NetWeight))
                .ForMember(src => src.ImagePath,
                    opts => opts.MapFrom(g => g.GetGoodImagePathOrDefault()))
                .ForMember(src => src.Maker,
                    opts => opts.MapFrom(g => g.Maker.Name))
                .ForMember(src => src.Nutrients,
                    opts => opts.MapFrom(g => g.ProteinsFatsCarbohydratesCalories))
                .ForMember(src => src.PublishingStatus,
                    opts => opts.MapFrom(g => (int) g.PublishingStatus));
        }

        private void CreatePfccMap()
        {
            CreateMap<ProteinsFatsCarbohydratesCalories, ProteinsFatsCarbohydratesCaloriesDto>()
                .ForMember(src => src.Calories,
                    opts => opts.MapFrom(lg => lg.CaloriesInKcal))
                .ForMember(src => src.Fats,
                    opts => opts.MapFrom(lg => lg.FatsInGrams))
                .ForMember(src => src.Proteins,
                    opts => opts.MapFrom(lg => lg.ProteinsInGrams))
                .ForMember(src => src.Carbohydrates,
                    opts => opts.MapFrom(lg => lg.CarbohydratesInGrams));
        }

        private void CreateMakerMap()
        {
            CreateMap<Maker, MakerDto>();
        }

        private void CreateLabeledGoodMap()
        {
            CreateMap<LabeledGood, LabeledGoodDto>()
                .ForMember(src => src.ExpirationDate,
                    opts => opts.MapFrom(lg =>
                        SharedDateTimeConverter.ConvertDatePartToString(lg.ExpirationDate ?? DateTime.MinValue)))
                .ForMember(src => src.ManufactureDate,
                    opts => opts.MapFrom(lg =>
                        SharedDateTimeConverter.ConvertDatePartToString(lg.ManufactureDate ?? DateTime.MinValue)))
                .ForMember(src => src.Currency,
                    opts => opts.MapFrom(lg => lg.Currency.Name));
            CreateMap<LabeledGood, LabeledGoodPartnerDto>()
                .ForMember(src => src.ExpirationDate,
                    opts => opts.MapFrom(lg => lg.ExpirationDate.HasValue
                        ? lg.ExpirationDate.Value.ToMillisecondsSince1970()
                        : (long?) null))
                .ForMember(src => src.ManufactureDate,
                    opts => opts.MapFrom(lg => lg.ManufactureDate.HasValue
                        ? lg.ManufactureDate.Value.ToMillisecondsSince1970()
                        : (long?) null))
                .ForMember(src => src.Currency,
                    opts => opts.MapFrom(lg => lg.Currency.Name));
            CreateMap<LabeledGood, LabeledGoodWithImageDto>()
                .ForMember(src => src.Id,
                    opts => opts.MapFrom(lg => lg.GetGoodId()))
                .ForMember(src => src.ImagePath,
                    opts => opts.MapFrom(lg => lg.GetImagePath()))
                .ForMember(src => src.Currency,
                    opts => opts.MapFrom(lg => lg.GetCurrencyName()))
                .ForMember(src => src.Name,
                    opts => opts.MapFrom(lg => lg.GetGoodName()))
                .ForMember(src => src.Nutrients,
                    opts => opts.MapFrom(lg => lg.GetGoodNitrients()))
                .ForMember(src => src.Count,
                    opts => opts.MapFrom(lg => lg.GetLabeledGoodsCountInsidePosDistinctByPrice()))
                .ForMember(src => src.Weight,
                    opts => opts.MapFrom(lg => lg.GetGoodNetWeight()))
                .ForMember(src => src.Composition,
                    opts => opts.MapFrom(lg => lg.GetGoodComposition()))
                .ForMember(src => src.Description,
                    opts => opts.MapFrom(lg => lg.GetGoodDescription()))
                .ForMember(src => src.Maker,
                    opts => opts.MapFrom(lg => lg.GetMakerName()))
                .ForMember(src => src.PublishingStatus,
                    opts => opts.MapFrom(lg => (int)lg.GetGoodPublishingStatus()));
        }

        private void CreateHardToDetectLabelMap()
        {
            CreateMap<LabeledGood, HardToDetectLabelDto>().ForMember(src => src.GoodName,
                opts => opts.MapFrom(lg => lg.Good.Name));
        }

        private void CreateSensorMeasurementsMap()
        {
            CreateMap<SensorMeasurements, SensorMeasurementsDto>()
                .ForMember(src => src.SensorId, opts => opts.MapFrom(sm => sm.SensorPosition));
        }

        private void CreatePosMap()
        {
            CreateMap<Pos, PosDto>()
                .ForMember(src => src.IsNotDeactivated,
                    opts => opts.MapFrom(p => p.PosActivityStatus != PosActivityStatus.Inactive));
            CreateMap<Pos, PosSyncDto>();
            CreateMap<Pos, PointOfSaleDto>()
                .ForMember(src => src.RestrictedAccess,
                    opts => opts.MapFrom(p => p.IsRestrictedAccess))
                .ForMember(src => src.Location,
                    opts => opts.MapFrom(p => p.AccurateLocation));
        }

        private void CreatePosRealTimeInfoMap()
        {
            CreateMap<PosRealTimeInfo, PosRealTimeInfoDto>()
                .ForMember(src => src.HardToDetectLabels, opt => opt.Ignore())
                .ForMember(src => src.Temperature, opt => opt.MapFrom(pti => pti.TemperatureInsidePos))
                .ForMember(src => src.RfidTemperature, opt => opt.MapFrom(pti => pti.RfidTemperature))
                .ForMember(src => src.ScreenResolution,
                    opt => opt.MapFrom(pti => pti.UpdatableScreenResolution.ScreenResolution));
        }

        private void CreateCountryMap()
        {
            CreateMap<Country, CountryDto>();
        }

        private void CreateCityMap()
        {
            CreateMap<City, CityDto>();
        }

        private void CreatePaymentCardMap()
        {
            CreateMap<PaymentCardNumber, PaymentCardNumberDto>();
            CreateMap<PaymentCard, PaymentCardDto>();
        }

        private void CreateUserMap()
        {
            CreateMap<ApplicationUser, UserShortInfoDto>();
            CreateMap<ApplicationUser, UserFullInfoDto>()
                .ForMember(
                    src => src.AvailableBonusPoints,
                    opts => opts.MapFrom(u => u.TotalBonusPoints)
                )
                .ForMember(
                    src => src.ActivePaymentCard,
                    opts => opts.MapFrom(
                        u => u.ActivePaymentCard.HasNumber ? u.ActivePaymentCard : null)
                )
                .ForMember(
                    src => src.BirthDate,
                    opts => opts.MapFrom(
                        u => u.HasBirthDate() ? u.Birthdate : (DateTime?) null
                    )
                );
        }

        private void CreateGoodImageMap()
        {
            CreateMap<GoodImage, GoodImageDto>();
        }

        private void CreateLocationMap()
        {
            CreateMap<LocationDto, Location>();
        }

        private void CreateLogMap()
        {
            CreateMap<Log, LogDto>()
                .ForMember(
                    src => src.Timestamp,
                    opts => opts.MapFrom(
                        l => SharedDateTimeConverter.ConvertDateTimeToString(l.Timestamp.UtcDateTime))
                );
        }

        private void CreateCheckMap()
        {
            CreateMap<Currency, CurrencyDto>();
            CreateMap<SimpleCheckCostSummary, PriceInfoDto>()
                .ForMember(
                    src => src.Currency,
                    opts => opts.MapFrom(sc => sc.Currency)
                )
                .ForMember(
                    src => src.TotalPrice,
                    opts => opts.MapFrom(sc => sc.CostWithDiscount)
                )
                .ForMember(
                    pi => pi.TotalDiscount,
                    opts => opts.MapFrom(sc => sc.Discount)
                )
                .ForMember(
                    src => src.PricePerItem,
                    opts => opts.MapFrom(sc => sc.ItemsQuantity > 0
                        ? sc.CostWithDiscount / sc.ItemsQuantity
                        : 0
                    )
                )
                .ForMember(
                    src => src.TotalPriceWithDiscount,
                    opts => opts.MapFrom(sc => sc.CostWithDiscount)
                )
                .ForMember(
                    src => src.PriceWithoutDiscount,
                    opts => opts.MapFrom(sc => sc.CostWithoutDiscount)
                )
                .ForMember(
                    src => src.Quantity,
                    opts => opts.MapFrom(sc => 0)
                );

            CreateMap<SimpleCheckItem, CheckGoodDto>()
                .ForMember(
                    src => src.Id,
                    opts => opts.MapFrom(sci => sci.GoodInfo.GoodId)
                )
                .ForMember(
                    src => src.Name,
                    opts => opts.MapFrom(sci => sci.GoodInfo.GoodName)
                )
                .ForMember(
                    src => src.Maker,
                    opts => opts.MapFrom(sci => Maker.Default)
                )
                .ForMember(
                    src => src.PriceInfo,
                    opts => opts.MapFrom(sci => sci.CostSummary)
                )
                .ForMember(
                    src => src.Description,
                    opts => opts.MapFrom(sci => sci.GoodInfo.GoodDescription))
                .ForPath(
                    src => src.PriceInfo.Quantity,
                    opts => opts.MapFrom(sci => sci.CostSummary.ItemsQuantity)
                );

            CreateMap<SimpleCheck, CheckDto>()
                .ForMember(
                    src => src.PriceInfo,
                    opts => opts.MapFrom(sc => sc.Summary.CostSummary)
                )
                .ForPath(
                    src => src.PriceInfo.Quantity,
                    opts => opts.MapFrom(sc => sc.Summary.CostSummary.ItemsQuantity)
                )
                .ForMember(
                    src => src.IsZero,
                    opts => opts.MapFrom(sc => sc.IsFree)
                )
                .ForMember(
                    src => src.PurchaseDateTime,
                    opts => opts.MapFrom(sc => ToMoscowFullDateOrEmptyString(sc.DateCreated))
                )
                .ForMember(
                    src => src.Goods,
                    opts => opts.MapFrom(sc => sc.Items)
                );

            CreateMap<PurchaseHistory, PurchasesHistoryDto>();
        }

        private void CreateSimpleCheckMap()
        {
	        CreateMap<CheckPaymentErrorInfo, SimpleCheckPaymentErrorInfoDto>()
		        .ForMember(
			        src => src.NextPaymentAttemptDate,
			        opt => opt.MapFrom( sc => sc.PaymentDate )
			        );
            CreateMap<CheckFiscalizationInfo, SimpleCheckFiscalizationInfoDto>();
            CreateMap<CheckStatusInfo, SimpleCheckStatusInfoDto>();
            CreateMap<SimpleCheckBonusInfo, SimpleCheckBonusInfoDto>();
            CreateMap<SimpleCheckCostSummary, SimpleCheckCostSummaryDto>();
            CreateMap<SimpleCheckGoodInfo, SimpleCheckGoodInfoDto>();
            CreateMap<SimpleCheckItem, SimpleCheckItemDto>();
            CreateMap<SimpleCheckOriginInfo, SimpleCheckOriginInfoDto>();
            CreateMap<SimpleCheckSummary, SimpleCheckSummaryDto>();
            CreateMap<SimpleCheck, SimpleCheckDto>()
	            .ForMember( 
		            src => src.PaymentError,
		            opt => opt.MapFrom( sc => sc.PaymentErrorInfo )
		            );
        }

        private void CreatePosImageMap()
        {
            CreateMap<PosImage, PosImageDto>();
        }

        private void CreateBankTransactionInfoMap()
        {
            CreateMap<BankTransactionInfo, BankTransactionInfoDto>();
        }

        private void CreateFeedbackMap()
        {
            CreateMap<Feedback, FeedbackDto>();
            CreateMap<FeedbackDto, Feedback>();
        }

        private void CreateInfo3DsMap()
        {
            CreateMap<Info3Ds, Info3DsDto>();
        }

        private void CreatePurchaseCompletionResultMap()
        {
            CreateMap<PurchaseCompletionResult, PurchaseCompletionResultDto>()
                .ForMember(src => src.Check, opts => opts.MapFrom(pr => pr.Check))
                .ForMember(src => src.PurchaseCheck, opts => opts.MapFrom(pr => pr.Check))
                .ForMember(src => src.LocalizedError,
                    opts => opts.MapFrom(pr => pr.Error.LocalizedDescription)
                );
        }

        private void CreateBankingCardConfirmationResultMap()
        {
            CreateMap<PaymentCardConfirmationResult, PaymentCardConfirmationResultDto>()
                .ForMember(src => src.PaymentStatus,
                    opts => opts.MapFrom(pcr => pcr.ConfirmationStatus)
                );
        }

        private void CreatePosTemperatureDetailsMap()
        {
            CreateMap<PosTemperature, PosTemperatureDto>();
        }

        private void CreatePurchaseInitiationResultMap()
        {
            CreateMap<PurchaseInitiationResult, PurchaseInitiationResultDto>();
        }

        private void CreateGoodsMovingReportItemFromDocumentTablePartMap()
        {
            CreateMap<DocumentGoodsMovingTableItem, GoodMovingReportRecord>()
                .ForMember(src => src.PosId,
                    opts => opts.MapFrom(tp => tp.Document.PosId))
                .ForMember(src => src.PosName,
                    opts => opts.MapFrom(tp => tp.Document.PointOfSale.Name))
                .ForMember(src => src.GoodCategory,
                    opts => opts.MapFrom(tp => tp.Good.GoodCategory.Name))
                .ForMember(src => src.GoodCategoryId,
                    opts => opts.MapFrom(tp => tp.Good.GoodCategoryId))
                .ForMember(src => src.GoodName,
                    opts => opts.MapFrom(tp => tp.Good.Name))
                .ForMember(src => src.DocumentNumber,
                    opts => opts.MapFrom(tp => tp.Document.DocumentNumber))
                .ForMember(src => src.CreatedDate,
                    opts => opts.MapFrom(tp => ToMoscowFullDateTimeOrEmptyString(tp.Document.CreatedDate)));
        }

        private void CreateLabeledGoodPartnerInfoMap()
        {
            CreateMap<LabeledGoodPartnerDto, LabeledGoodPartnerInfo>();
        }

        private void CreateApproachDataMap()
        {
            CreateMap<ApproachInfoDto, ApproachRecord>()
                .ForMember(src => src.Id,
                    opts => opts.MapFrom(tp => tp.DeviceId))
                .ForMember(src => src.Url,
                    opts => opts.MapFrom(tp => tp.DeviceUrl))
                .ForMember(src => src.Pswd,
                    opts => opts.MapFrom(tp => tp.Password));
        }

        private static string ToMoscowFullDateOrEmptyString(DateTime dateTime)
        {
            var moscowDateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(dateTime);
            return moscowDateTime.ToString("d MMMM yyyy", new CultureInfo("ru"));
        }

        private static string ToMoscowFullDateTimeOrEmptyString(DateTime dateTime)
        {
            var moscowDateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(dateTime);
            return SharedDateTimeConverter.ConvertDateTimeToString(moscowDateTime);
        }

        private void CreatePosOperationDtoMap()
        {
            CreateMap<IGrouping<PosOperation, CheckItem>, PosOperationDto>()
                .ForMember(src => src.Id, opt => opt.MapFrom(x => x.Key.Id))
                .ForMember(src => src.ShopId, opt => opt.MapFrom(x => x.Key.Pos.Id))
                .ForMember(src => src.DatePaid, opt => opt.MapFrom(x => x.Key.DatePaid))
                .ForMember(src => src.DateCompleted, opt => opt.MapFrom(x => x.Key.DateCompleted))
                .ForMember(src => src.DateAuditCompleted, opt => opt.MapFrom(x => x.Key.AuditCompletionDateTime))
                .ForMember(src => src.OperationStatus, opt => opt.MapFrom(x => x.Key.Status))
                .ForMember(src => src.BonusAmount, opt => opt.MapFrom(x => x.Key.BonusAmount))
                .ForMember(src => src.TotalCost, opt => opt.MapFrom(x => x.Key.CheckItems
                    .Where(cki => cki.PosOperationId == x.Key.Id &&
                                  cki.Status == CheckItemStatus.Unpaid || cki.Status == CheckItemStatus.Paid ||
                                  cki.Status == CheckItemStatus.PaidUnverified)
                    .Sum(cki => cki.PriceWithDiscount)))
                .ForMember(src => src.TotalCostWithoutDiscount, opt => opt.MapFrom(x => x.Key.CheckItems
                    .Where(cki => cki.PosOperationId == x.Key.Id &&
                                  cki.Status == CheckItemStatus.Unpaid || cki.Status == CheckItemStatus.Paid ||
                                  cki.Status == CheckItemStatus.PaidUnverified)
                    .Sum(cki => cki.Price)));
        }

        private void CreatePosOperationVersioniTwoDtoMap()
        {
            CreateMap<IGrouping<PosOperation, CheckItem>, PosOperationVersionTwoDto>()
                .ForMember(src => src.Id, opt => opt.MapFrom(x => x.Key.Id))
                .ForMember(src => src.ShopId, opt => opt.MapFrom(x => x.Key.Pos.Id))
                .ForMember(src => src.DatePaid, opt => opt.MapFrom(x => x.Key.DatePaid))
                .ForMember(src => src.DateCompleted, opt => opt.MapFrom(x => x.Key.DateCompleted))
                .ForMember(src => src.DateAuditCompleted, opt => opt.MapFrom(x => x.Key.AuditCompletionDateTime))
                .ForMember(src => src.OperationStatus, opt => opt.MapFrom(x => x.Key.Status))
                .ForMember(src => src.TotalCost, opt => opt.MapFrom(x => x.Key.CheckItems
                    .Where(cki => cki.PosOperationId == x.Key.Id &&
                                  cki.Status == CheckItemStatus.Unpaid || cki.Status == CheckItemStatus.Paid ||
                                  cki.Status == CheckItemStatus.PaidUnverified)
                    .Sum(cki => cki.PriceWithDiscount)))
                .ForMember(src => src.TotalCostWithoutDiscount, opt => opt.MapFrom(x => x.Key.CheckItems
                    .Where(cki => cki.PosOperationId == x.Key.Id &&
                                  cki.Status == CheckItemStatus.Unpaid || cki.Status == CheckItemStatus.Paid ||
                                  cki.Status == CheckItemStatus.PaidUnverified)
                    .Sum(cki => cki.Price)))
                .ForMember(src => src.PosOperationTransactions, opt => opt.MapFrom(x => x.Key.PosOperationTransactions));
        }

        private void CreateFiscalizationInfoOneCSyncDtoMap()
        {
            CreateMap<FiscalizationInfo, FiscalizationInfoOneCSyncDto>();
            CreateMap<FiscalizationInfoVersionTwo, FiscalizationInfoOneCSyncDto>();
        }

        private void CreateMessengerContactMap()
        {
            CreateMap<MessengerContact, MessengerContactDto>();
        }

        private void CreateBankTransactionInfoOneCSyncDtoMap()
        {
            CreateMap<BankTransactionInfo, BankTransactionInfoOneCSyncDto>();
            CreateMap<BankTransactionInfoVersionTwo, BankTransactionInfoOneCSyncDto>();
        }

        private void CreateGoodCategoryMap()
        {
            CreateMap<GoodCategory, GoodCategoryDto>();
        }

        private void CreatePosOperationTransactonDtoMap()
        {
            CreateMap<PosOperationTransaction, PosOperationTransactionDto>()
                .ForMember(src => src.TransactionCheckItems, opt => opt.MapFrom(x => x.PosOperationTransactionCheckItems));
        }

        private void CreatePosOperationTransactionCheckItemDtoDtoMap()
        {
            CreateMap<PosOperationTransactionCheckItem, PosOperationTransactionCheckItemDto>();
        }
    }
}