using AutoMapper;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Discounts;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Core.Services.Check.Detailed.Models;
using NasladdinPlace.Core.Services.Pos.Groups.Models;
using NasladdinPlace.Core.Services.Pos.State.Models;
using NasladdinPlace.Dtos;
using NasladdinPlace.UI.Dtos.Audit;
using NasladdinPlace.UI.Dtos.BankTransaction;
using NasladdinPlace.UI.Dtos.Check;
using NasladdinPlace.UI.Dtos.Currency;
using NasladdinPlace.UI.Dtos.Good;
using NasladdinPlace.UI.Dtos.GoodCategory;
using NasladdinPlace.UI.Dtos.LabeledGood;
using NasladdinPlace.UI.Dtos.Maker;
using NasladdinPlace.UI.Dtos.Pos;
using NasladdinPlace.UI.Dtos.PosOperation;
using NasladdinPlace.UI.Dtos.User;
using NasladdinPlace.UI.Managers.Reference.UniReferencesManagers.Models;
using NasladdinPlace.UI.Services.Mapper;
using NasladdinPlace.UI.ViewModels.Checks;
using NasladdinPlace.UI.ViewModels.Discounts;
using NasladdinPlace.UI.ViewModels.Documents;
using NasladdinPlace.UI.ViewModels.Fiscalization;
using NasladdinPlace.UI.ViewModels.GoodCategories;
using NasladdinPlace.UI.ViewModels.Goods;
using NasladdinPlace.UI.ViewModels.LabeledGoods;
using NasladdinPlace.UI.ViewModels.Logs;
using NasladdinPlace.UI.ViewModels.Media;
using NasladdinPlace.UI.ViewModels.PointsOfSale;
using NasladdinPlace.UI.ViewModels.PosOperationTransaction;
using NasladdinPlace.UI.ViewModels.PosScreenTemplates;
using NasladdinPlace.UI.ViewModels.PosSensor;
using NasladdinPlace.UI.ViewModels.Promotions;
using NasladdinPlace.UI.ViewModels.Reports;
using NasladdinPlace.UI.ViewModels.Roles;
using NasladdinPlace.UI.ViewModels.Schedules;
using NasladdinPlace.UI.ViewModels.Security;
using NasladdinPlace.UI.ViewModels.UserNotifications;
using NasladdinPlace.UI.ViewModels.Users;
using NasladdinPlace.Utilities.DateTimeConverter;
using NasladdinPlace.Utilities.EnumHelpers;
using System;
using System.Globalization;
using System.Linq;
using NasladdinPlace.UI.ViewModels.Makers;
using PosFormViewModel = NasladdinPlace.UI.ViewModels.PointsOfSale.PosFormViewModel;
using PosTemperatureDto = NasladdinPlace.Dtos.Pos.PosTemperatureDto;

namespace NasladdinPlace.UI.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateGoodMap();
            CreateLabeledGoodMap();
            CreatePosMap();
            CreateUserMap();
            CreateMakerMap();
            CreatePosOperationMap();
            CreateBankTransactionInfoMap();
            CreateUserNotificationMap();
            CreatePromotionsMap();
            CreateScheduleMap();
            CreateMediaContentMap();
            CreateReportUploadingInfoMap();
            CreatePosTemperatureDetailsMap();
            CreateFiscalizationInfosMap();
            CreateLabeledGoodTrackingRecordMap();
            CreatePosAbnormalSensorMeasurementMap();
            CreatePosTemperatureChartsMap();
            CreateDetailedCheckMap();
            CreateCheckItemAuditRecordMap();
            CreateDiscountsMap();
            CreatePosShortInfoMap();
            CreateLabeledGoodPosGroupMap();
            CreateGoodCategoriesMap();
            CreatePosOperationTransactionMap();
            CreatePosOperationTransactionDetailsMap();
            CreatePosScreenTemplatesMap();
            CreateCurrencyMap();
            CreatePosLogMap();
            CreateRoleMap();
            CreateAppFeatureMap();
            CreateDocumentGoodsMovingMap();
            CreateMakerMap();
            CreateBankTransacitonInfosVersionTwoMap();
            CreateFiscalizationInfosVersioinTwoMap();
        }

        private void CreateRoleMap()
        {
            CreateMap<Role, RoleViewModel>();
        }

        private void CreateAppFeatureMap()
        {
            CreateMap<AppFeatureItem, AppFeaturesViewModel>()
                .ForMember(src => src.Category, opts => opts.MapFrom(u => u.PermissionCategory.GetDescription()));
        }

        private void CreatePosScreenTemplatesMap()
        {
            CreateMap<PosScreenTemplate, PosScreenTemplateFormViewModel>();
        }

        private void CreateCurrencyMap()
        {
            CreateMap<Core.Models.Currency, CurrencyDto>();
        }

        private void CreateCheckItemAuditRecordMap()
        {
            CreateMap<CheckItemAuditRecord, CheckItemAuditRecordDto>()
                .ForMember(src => src.CreatedDate,
                    opts => opts.MapFrom(ar => GetMoscowDateTime(ar.CreatedDate)))
                .ForMember(src => src.UserName,
                    opts => opts.MapFrom(ar => ar.User.UserName))
                .ForMember(src => src.GoodName,
                    opts => opts.MapFrom(ar => ar.CheckItem.Good.Name))
                .ForMember(src => src.Label,
                    opts => opts.MapFrom(ar => ar.CheckItem.LabeledGood.Label));
        }

        private void CreateDetailedCheckMap()
        {
            CreateMap<DetailedCheck, DetailedCheckDto>()
                .ForMember(src => src.DateStatusUpdated,
                    opts => opts.MapFrom(d => GetMoscowDateTime(d.DateStatusUpdated)));
        }

        private void CreatePosMap()
        {
            CreateMap<Pos, PosFormViewModel>();
            CreateMap<PosDto, PosViewModel>()
                .ForMember(src => src.City,
                    opts => opts.MapFrom(p => p.City.Name))
                .ForMember(src => src.IpAddress,
                    opts => opts.MapFrom(p => p.IpAddresses.Any() ? string.Join(",", p.IpAddresses) : "—"));
            CreateMap<Pos, PosBasicInfoViewModel>()
                .ForMember(src => src.PosId,
                    opts => opts.MapFrom(p => p.Id));
        }

        private void CreateGoodMap()
        {
            CreateMap<Good, GoodDto>()
                .ForMember(src => src.Name,
                    opt => opt.MapFrom(g => $"{g.Id} - {g.Name}"));
            CreateMap<Good, GoodShortInfoDto>();
            CreateMap<Good, GoodViewModel>()
                .ForMember(src => src.GoodCategory,
                    opts => opts.MapFrom(g => g.GoodCategory.Name))
                .ForMember(src => src.Maker,
                    opts => opts.MapFrom(g => g.Maker.Name))
                .ForMember(src => src.PublishingStatus,
                    opts => opts.MapFrom(g => (int)g.PublishingStatus));
            CreateMap<Good, GoodsFormViewModel>()
                .ForMember(src => src.ProteinsInGrams,
                    opts => opts.MapFrom(g => g.ProteinsFatsCarbohydratesCalories != null
                        ? g.ProteinsFatsCarbohydratesCalories.ProteinsInGrams
                        : (double?)null)
                    )
                .ForMember(src => src.FatsInGrams,
                    opts => opts.MapFrom(g => g.ProteinsFatsCarbohydratesCalories != null
                        ? g.ProteinsFatsCarbohydratesCalories.FatsInGrams
                        : (double?)null)
                )
                .ForMember(src => src.CarbohydratesInGrams,
                    opts => opts.MapFrom(g => g.ProteinsFatsCarbohydratesCalories != null
                        ? g.ProteinsFatsCarbohydratesCalories.CarbohydratesInGrams
                        : (double?)null)
                )
                .ForMember(src => src.CaloriesInKcal,
                    opts => opts.MapFrom(g => g.ProteinsFatsCarbohydratesCalories != null
                        ? g.ProteinsFatsCarbohydratesCalories.CaloriesInKcal
                        : (double?)null)
                )
                .ForMember(opt => opt.ImagePath,
                    g => g.MapFrom(x=>x.GetGoodImagePath()));

            CreateMap<Good, BuyGoodViewModel>()
                .ForMember(src => src.Category, opt => opt.MapFrom(g => g.GoodCategory.Name))
                .ForMember(src => src.ProteinsInGrams,
                    opts => opts.MapFrom(g => g.ProteinsFatsCarbohydratesCalories != null
                        ? g.ProteinsFatsCarbohydratesCalories.ProteinsInGrams
                        : (double?)null)
                )
                .ForMember(src => src.FatsInGrams,
                    opts => opts.MapFrom(g => g.ProteinsFatsCarbohydratesCalories != null
                        ? g.ProteinsFatsCarbohydratesCalories.FatsInGrams
                        : (double?)null)
                )
                .ForMember(src => src.CarbohydratesInGrams,
                    opts => opts.MapFrom(g => g.ProteinsFatsCarbohydratesCalories != null
                        ? g.ProteinsFatsCarbohydratesCalories.CarbohydratesInGrams
                        : (double?)null)
                )
                .ForMember(src => src.CaloriesInKcal,
                    opts => opts.MapFrom(g => g.ProteinsFatsCarbohydratesCalories != null
                        ? g.ProteinsFatsCarbohydratesCalories.CaloriesInKcal
                        : (double?)null)
                )
                .ForMember(opt => opt.ImagePath, g => g.MapFrom(x => x.GetGoodImagePath()))
                .ForMember(src => src.Maker, opts => opts.MapFrom(g => g.Maker.Name))
                .ForMember(src => src.Price, opts => 
                    opts.MapFrom(g => g.LabeledGoods.OrderByDescending(lg => lg.Id).FirstOrDefault().Price));
        }

        private void CreateLabeledGoodMap()
        {
            CreateMap<LabeledGood, LabeledGoodDto>()
                .ForMember(src => src.ExpirationDate,
                    opts => opts.MapFrom(lg => GetMoscowDateHourMinutePartsOrNull(lg.ExpirationDate ?? DateTime.MinValue)))
                .ForMember(src => src.ManufactureDate,
                    opts => opts.MapFrom(lg => GetMoscowDateHourMinutePartsOrNull(lg.ManufactureDate ?? DateTime.MinValue)))
                .ForMember(src => src.Currency,
                    opts => opts.MapFrom(lg => lg.Currency.Name));

            CreateMap<LabeledGood, LabeledGoodFormViewModel>()
                .ForMember(src => src.ExpirationDate,
                    opts => opts.MapFrom(lg =>
                        GetMoscowDateHourMinutePartsOrNull(lg.ExpirationDate ?? DateTime.MinValue)))
                .ForMember(src => src.ManufactureDate,
                    opts => opts.MapFrom(lg =>
                        GetMoscowDateHourMinutePartsOrNull(lg.ManufactureDate ?? DateTime.MinValue)));

            CreateMap<LabeledGood, LabeledGoodBasicInfoViewModel>();

            CreateMap<LabeledGood, LabeledGoodsGroupByGoodViewModelForFilter>()
                .ForMember(src => src.Name,
                    opts => opts.MapFrom(lg => lg.Good.Name));
        }

        private void CreateUserMap()
        {
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(src => src.LastPosOperation,
                    opts => opts.MapFrom(ap =>
                        ap.PosOperations.Where(po => po.DatePaid != null)
                            .OrderByDescending(po => po.DatePaid).FirstOrDefault()))
                .ForMember(src => src.RegistrationInitiationDate,
                    opts => opts.MapFrom(u => GetMoscowDateHourMinutePartsOrNull(u.RegistrationInitiationDate)))
                .ForMember(src => src.RegistrationCompletionDate,
                    opts => opts.MapFrom(u => GetMoscowDateHourMinutePartsOrNull(u.RegistrationCompletionDate)))
                .ForMember(src => src.PaymentCardVerificationInitiationDate,
                    opts => opts.MapFrom(u =>
                        GetMoscowDateHourMinutePartsOrNull(u.PaymentCardVerificationInitiationDate)))
                .ForMember(src => src.PaymentCardVerificationCompletionDate,
                    opts => opts.MapFrom(u =>
                        GetMoscowDateHourMinutePartsOrNull(u.PaymentCardVerificationCompletionDate)))
                .ForMember(src => src.TotalBonus, opts => opts.MapFrom(u => u.TotalBonusPoints));

            CreateMap<ApplicationUser, UserEditViewModel>()
                .ForMember(src => src.Birthday,
                    opts => opts.MapFrom(u => GetMoscowDateHourMinutePartsOrNull(u.Birthdate)));

            CreateMap<ApplicationUser, UserViewModel>()
                .ForMember(src => src.Id, opts => opts.MapFrom(u => u.Id))
                .ForMember(src => src.RegistrationInitiationDate,
                    opts => opts.MapFrom(u => GetMoscowDateHourMinutePartsOrNull(u.RegistrationInitiationDate)))
                .ForMember(src => src.RegistrationCompletionDate,
                    opts => opts.MapFrom(u => GetMoscowDateHourMinutePartsOrNull(u.RegistrationCompletionDate)))
                .ForMember(src => src.BankingCardVerificationInitiationDate,
                    opts => opts.MapFrom(u =>
                        GetMoscowDateHourMinutePartsOrNull(u.PaymentCardVerificationInitiationDate)))
                .ForMember(src => src.BankingCardVerificationCompletionDate,
                    opts => opts.MapFrom(u =>
                        GetMoscowDateHourMinutePartsOrNull(u.PaymentCardVerificationCompletionDate)))
                .ForMember(src => src.LastPaidDate, opts => opts.MapFrom(u =>
                    u.PosOperations != null && u.PosOperations.Any()
                        ? GetMoscowDateHourMinutePartsOrNull(u.PosOperations.Where(po => po.DatePaid != null)
                            .OrderByDescending(po => po.DatePaid).FirstOrDefault().DatePaid)
                        : null))
                .ForMember(src => src.Gender, opts => opts.MapFrom(u => u.Gender))
                .ForMember(src => src.Birthdate, opts => opts.MapFrom(u => GetMoscowDateTime(u.Birthdate)))
                .ForMember(src => src.TotalBonus, opts => opts.MapFrom(u => u.TotalBonusPoints))
                .ForMember(src => src.SelectedRoles, opts => opts.MapFrom(u => u.UserRoles.Select(ur => ur.RoleId)));
        }

        private void CreateMakerMap()
        {
            CreateMap<Maker, MakerDto>();
            CreateMap<Maker, MakerViewModel>();
        }

        private void CreatePosOperationMap()
        {
            CreateMap<PosOperation, DetailedCheckViewModel>()
                .ForMember(src => src.User, opts => opts.MapFrom(po => po.User))
                .ForPath(src => src.AuditDateTime.AuditRequestDateTime, opts => opts.MapFrom(po => GetMoscowDateHourMinutePartsOrNull(po.AuditRequestDateTime)))
                .ForPath(src => src.AuditDateTime.AuditCompletionDateTime, opts => opts.MapFrom(po => GetMoscowDateHourMinutePartsOrNull(po.AuditCompletionDateTime)))
                .ForPath(src => src.User.TotalBonus, opts => opts.MapFrom(po => po.User.TotalBonusPoints))
                .ForMember(src => src.Transactions,
                    opts => opts.MapFrom(src => src.BankTransactionInfos.OrderByDescending(bti => bti.DateCreated)))
                .ForMember(src => src.FiscalizationChecks,
                    opts => opts.MapFrom(src => src.FiscalizationInfos.OrderByDescending(fi => fi.DateTimeRequest)))
                .ForMember(src => src.OperationTransactions,
                    opts => opts.MapFrom(src => src.PosOperationTransactions.OrderByDescending(t => t.CreatedDate)));

            CreateMap<PosOperation, ShortenPosOperationDto>()
                .ForMember(src => src.DatePaid, opts => opts.MapFrom(po => GetMoscowDateHourMinutePartsOrNull(po.DatePaid)));

            CreateMap<PosOperation, PosOperationViewModel>().ConvertUsing<PosOperationViewModelMapper>();
        }

        private void CreateBankTransactionInfoMap()
        {
            CreateMap<BankTransactionInfo, BankTransactionInfoDto>()
                .ForMember(src => src.Comment, opts => opts.MapFrom(bti => bti.Comment ?? string.Empty))
                .ForMember(src => src.DateCreated,
                    opts => opts.MapFrom(bti => GetMoscowDateHourMinutePartsOrNull(bti.DateCreated)));

            CreateMap<BankTransactionInfoVersionTwo, BankTransactionInfoDto>()
                .ForMember(src => src.Comment, opts => opts.MapFrom(bti => bti.Comment ?? string.Empty))
                .ForMember(src => src.DateCreated,
                    opts => opts.MapFrom(bti => GetMoscowDateHourMinutePartsOrNull(bti.DateCreated)));
        }

        private void CreateUserNotificationMap()
        {
            CreateMap<UserNotification, UserNotificationViewModel>()
                .ForMember(src => src.UserName, opts => opts.MapFrom(u => u.User != null ? u.User.UserName : ""))
                .ForMember(src => src.DateTimeSent, opts => opts.MapFrom(u => GetMoscowDateTime(u.DateTimeSent)));
        }

        private void CreatePromotionsMap()
        {
            CreateMap<PromotionSetting, PromotionSettingViewModel>()
                .ForMember(src => src.PromotionSettingId, opts => opts.MapFrom(ps => ps.Id));
        }

        private void CreateGoodCategoriesMap()
        {
            CreateMap<GoodCategory, GoodCategoryViewModel>();
            CreateMap<GoodCategory, GoodCategoryDto>();
        }

        private void CreateScheduleMap()
        {
            CreateMap<ScheduleDto, ScheduleViewModel>()
                .ForMember(src => src.Enabled, opts => opts.MapFrom(s => !s.Disabled));
        }

        private void CreateMediaContentMap()
        {
            CreateMap<MediaContentToPosPlatform, MediaContentToPosPlatformViewModel>()
                .ForMember(src => src.MediaContentToPosPlatformId, opts => opts.MapFrom(mc => mc.Id))
                .ForMember(src => src.FileName, opts => opts.MapFrom(mc => mc.MediaContent != null ? mc.MediaContent.FileName : ""))
                .ForMember(src => src.DateTimeCreated, opts => opts.MapFrom(mc => GetMoscowDateTime(mc.DateTimeCreated)));

            CreateMap<PosMediaContent, PosMediaContentViewModel>()
                .ForMember(src => src.FileName, opts => opts.MapFrom(pm => pm.MediaContent != null ? pm.MediaContent.FileName : ""))
                .ForMember(src => src.DateTimeCreated, opts => opts.MapFrom(pm => GetMoscowDateTime(pm.DateTimeCreated)));
        }

        private void CreateReportUploadingInfoMap()
        {
            CreateMap<ReportUploadingInfo, ReportUploadingInfoViewModel>();
        }

        private void CreatePosTemperatureDetailsMap()
        {
            CreateMap<PosTemperatureDto, PosTemperatureDetailsViewModel>()
                .ForMember(src => src.DateTimeTemperatureReceipt, opts => opts.MapFrom(pt => GetMoscowDateTime(pt.DateTimeTemperatureReceipt)));
        }

        private void CreatePosTemperatureChartsMap()
        {
            CreateMap<PosEquipmentState, PosEquipmentStateChartRenderingViewModel>()
                .ForMember(src => src.MeasurementDateTime, opts => opts.MapFrom(s => GetMoscowDateTime(s.MeasurementDateTime).ToString(CultureInfo.InvariantCulture)))
                .ForMember(src => src.Temperature, opts => opts.MapFrom(s => s.TemperatureValue.ToString(CultureInfo.InvariantCulture)))
                .ForMember(src => src.AreDoorsOpened, opts => opts.MapFrom(s => s.DoorsState == DoorsState.RightDoorOpened || s.DoorsState == DoorsState.LeftDoorOpened));
        }

        private void CreateFiscalizationInfosMap()
        {
            CreateMap<FiscalizationInfo, FiscalizationInfoViewModel>()
                .ForMember(src => src.FiscalizationInfoId, opts => opts.MapFrom(f => f.Id))
                .ForMember(src => src.RequestDateTime, opts => opts.MapFrom(f => GetMoscowDateTime(f.DateTimeRequest)))
                .ForMember(src => src.ResponseDateTime, opts => opts.MapFrom(f => f.DateTimeResponse.HasValue ? GetMoscowDateTime(f.DateTimeResponse.Value) : (DateTime?)null));

            CreateMap<FiscalizationInfoVersionTwo, FiscalizationInfoViewModel>()
                .ForMember(src => src.FiscalizationInfoId, opts => opts.MapFrom(f => f.Id))
                .ForMember(src => src.RequestDateTime, opts => opts.MapFrom(f => GetMoscowDateTime(f.RequestDateTime)))
                .ForMember(src => src.ResponseDateTime, opts => opts.MapFrom(f => f.ResponseDateTime.HasValue ? GetMoscowDateTime(f.ResponseDateTime.Value) : (DateTime?)null));
        }

        private void CreateLabeledGoodTrackingRecordMap()
        {
            CreateMap<LabeledGoodTrackingRecord, LabeledGoodTrackingRecordViewModel>()
                .ForMember(src => src.Label, options => options.MapFrom(ltr => ltr.LabeledGood.Label))
                .ForMember(src => src.PosName, options => options.MapFrom(ltr => ltr.Pos.Name))
                .ForMember(src => src.GoodId, options => options.MapFrom(ltr => ltr.LabeledGood.GoodId))
                .ForMember(src => src.GoodName, options => options.MapFrom(ltr => ltr.LabeledGood.Good.Name))
                .ForMember(src => src.Timestamp, opts => opts.MapFrom(ltr => GetMoscowDateTime(ltr.Timestamp)));
        }

        private void CreatePosAbnormalSensorMeasurementMap()
        {
            CreateMap<PosAbnormalSensorMeasurement, PosAbnormalSensorMeasurementViewModel>()
                .ForMember(src => src.DateMeasured, options => options.MapFrom(sm => GetMoscowDateTime(sm.DateMeasured)))
                .ForMember(src => src.PosName, options => options.MapFrom(sm => sm.Pos.Name))
                .ForMember(src => src.Value, options => options.MapFrom(sm => $"{sm.MeasurementValue:0.00} {sm.MeasurementUnit.GetDescription()}"));
        }

        private void CreateDiscountsMap()
        {
            CreateMap<Discount, DiscountViewModel>()
                .ForMember(src => src.DateTimeCreated, options => options.MapFrom(d => GetMoscowDateTime(d.DateTimeCreated)))
                .ForMember(src => src.DiscountId, opt => opt.MapFrom(d => d.Id))
                .ForMember(src => src.AppliedTo, opt => opt.MapFrom(d => d.PosDiscounts != null && d.PosDiscounts.Any()
                                                                            ? string.Join(", ", d.PosDiscounts.Select(pd => pd.Pos.AbbreviatedName).ToList())
                                                                            : ""));
        }

        private void CreatePosOperationTransactionMap()
        {
            CreateMap<PosOperationTransaction, PosOperationTransactionViewModel>()
                .ForMember(src => src.PosOperationTransactionId, opts => opts.MapFrom(pot => pot.Id))
                .ForMember(src => src.CreatedDate, opts => opts.MapFrom(pot => GetMoscowDateTime(pot.CreatedDate)))
                .ForMember(src => src.FiscalizationPaidDate, opts => opts.MapFrom(
                    pot => pot.FiscalizationDate.HasValue
                        ? GetMoscowDateTime(pot.FiscalizationDate.Value)
                        : (DateTime?)null))
                .ForMember(src => src.BankTransactionPaidDate, opts => opts.MapFrom(
                    pot => pot.BankTransactionPaidDate.HasValue
                        ? GetMoscowDateTime(pot.BankTransactionPaidDate.Value)
                        : (DateTime?)null));
        }

        private void CreateBankTransacitonInfosVersionTwoMap()
        {
            CreateMap<BankTransactionInfoVersionTwo, BankTransactionInfoVersionTwo>()
                .ForMember(src => src.DateCreated, opts => opts.MapFrom(bti => GetMoscowDateTime(bti.DateCreated)));
        }

        private void CreateFiscalizationInfosVersioinTwoMap()
        {
            CreateMap<FiscalizationInfoVersionTwo, FiscalizationInfoVersionTwo>()
                .ForMember(src => src.RequestDateTime, opts => opts.MapFrom(f => GetMoscowDateTime(f.RequestDateTime)))
                .ForMember(src => src.ResponseDateTime, opts => opts.MapFrom(f => f.ResponseDateTime.HasValue ? GetMoscowDateTime(f.ResponseDateTime.Value) : (DateTime?)null));
        }

        private void CreatePosOperationTransactionDetailsMap()
        {
            CreateMap<PosOperationTransaction, PosOperationTransactionDetailsViewModel>()
                .IncludeBase<PosOperationTransaction, PosOperationTransactionViewModel>()
                .ForMember(src => src.BankTransactionInfos, opts => opts.MapFrom(pot => pot.BankTransactionInfos))
                .ForMember(src => src.FiscalizationInfos, opts => opts.MapFrom(pot => pot.FiscalizationInfos));
        }

        private void CreatePosShortInfoMap()
        {
            CreateMap<PosShortInfo, PosDto>();
        }

        private void CreateLabeledGoodPosGroupMap()
        {
            CreateMap<PosGroup<LabeledGood>, PosGroupDto<LabeledGoodDto>>();
        }

        private void CreatePosLogMap()
        {
            CreateMap<PosLog, PosLogsViewModelForFilter>()
                .ForMember(src => src.AbbreviatedName, opts => opts.MapFrom(pl => pl.Pos.AbbreviatedName));

            CreateMap<PosLogsViewModelForFilter, PosLogViewModel>()
                .ForMember(src => src.PosLogId, opts => opts.MapFrom(pl => pl.Id))
                .ForMember(src => src.PosName, opts => opts.MapFrom(pl => pl.AbbreviatedName))
                .ForMember(src => src.DateTimeCreated,
                    opts => opts.MapFrom(pl => GetMoscowDateTime(pl.DateTimeCreated)));
        }

        private void CreateDocumentGoodsMovingMap()
        {
            CreateMap<DocumentGoodsMoving, DocumentGoodsMovingViewModel>()
                .ForMember(src => src.User, opts => opts.MapFrom(doc => doc.PosOperation.User))
                .ForMember(src => src.PosName, opts => opts.MapFrom(doc => doc.PointOfSale.Name))
                .ForMember(src => src.TablePart,
                    opts => opts.MapFrom(doc => doc.TablePart.OrderBy(item => item.LineNum)))
                .ForMember(src => src.CreatedDate, opts => opts.MapFrom(item => GetMoscowDateTime(item.CreatedDate)))
                .ForMember(src => src.ModifiedDate, opts => opts.MapFrom(item => item.ModifiedDate.HasValue
                    ? GetMoscowDateTime(item.ModifiedDate.Value)
                    : (DateTime?)null));
        }

        private static string GetMoscowDateHourMinutePartsOrNull(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return null;

            var moscowDateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(dateTime.Value);
            return SharedDateTimeConverter.ConvertDateHourMinutePartsToString(moscowDateTime);
        }

        private static DateTime GetMoscowDateTime(DateTime dateTime)
        {
            var moscowDateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(dateTime);
            return moscowDateTime;
        }
    }
}