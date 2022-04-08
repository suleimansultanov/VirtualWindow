using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Core.Services.Check.CommonModels;
using NasladdinPlace.Core.Services.Check.Detailed.Makers.Contracts;
using NasladdinPlace.Core.Services.Check.Detailed.Mappers.Contracts;
using NasladdinPlace.Core.Services.Check.Detailed.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.Check.Detailed.Makers
{
    public class DetailedCheckMaker : IDetailedCheckMaker
    {
        private readonly IDetailedCheckGoodInstanceCreator _detailedCheckGoodInstanceCreator;
        private readonly string _fiscalizationQrCodeUrlTemplate;
        private readonly string _fiscalCheckUrlTemplate;

        public DetailedCheckMaker(
            IDetailedCheckGoodInstanceCreator detailedCheckGoodInstanceCreator,
            string fiscalizationQrCodeUrlTemplate,
            string fiscalCheckUrlTemplate)
        {
            _detailedCheckGoodInstanceCreator = detailedCheckGoodInstanceCreator;
            _fiscalizationQrCodeUrlTemplate = fiscalizationQrCodeUrlTemplate;
            _fiscalCheckUrlTemplate = fiscalCheckUrlTemplate;
        }

        public DetailedCheck MakeCheck(PosOperation operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            var checkItems = operation.CheckItems.ToImmutableList();

            var checkItemsGroupByGood = checkItems.GroupBy(ck => new { ck.GoodId, ck.PriceWithDiscount });

            var detailedCheckFiscalizationInfo = operation.TryGetCheckFiscalizationInfo(
                _fiscalizationQrCodeUrlTemplate,
                _fiscalCheckUrlTemplate,
                out var checkFiscalizationInfo
            )
                ? checkFiscalizationInfo
                : CheckFiscalizationInfo.Empty;

            var detailedCheckGoods = checkItemsGroupByGood.Select(g =>
            {
                var detailedCheckGoodInstances =
                    g.Select(cki => _detailedCheckGoodInstanceCreator.Create(
                            cki,
                            detailedCheckFiscalizationInfo,
                            operation,
                            _fiscalizationQrCodeUrlTemplate,
                            _fiscalCheckUrlTemplate
                        ))
                        .ToImmutableList();

                var good = g.FirstOrDefault()?.Good ?? Good.Unknown;

                var checkGood = new DetailedCheckGood(g.Key.GoodId,
                    good.Name, good.GoodCategoryId, good.GoodCategory.Name, g.FirstOrDefault()?.DatePaid, detailedCheckGoodInstances);
                return checkGood;
            }).ToImmutableList();

            var userInfo = new DetailedCheckUserInfo(operation.User);
            var posInfo = new DetailedCheckPosInfo(operation.Pos);
            var checkGoodsStatistics = new DetailedCheckGoodsStatistics(detailedCheckGoods);

            var errorInfo = CheckPaymentErrorInfo.Empty;
            var lastBankTransaction = operation.GetLastBankTransactionInfo();
            if ( lastBankTransaction != null && lastBankTransaction.Type == BankTransactionInfoType.Error )
            {
                errorInfo = new CheckPaymentErrorInfo( lastBankTransaction.Comment, lastBankTransaction.DateCreated,
                    lastBankTransaction.PaymentCardId);
            }

            return new DetailedCheck(
                operation,
                detailedCheckGoods,
                userInfo,
                posInfo,
                checkGoodsStatistics,
                detailedCheckFiscalizationInfo,
                errorInfo )
            {
                CryptogramSources = operation.CryptogramSources,
                DatePaid = operation.DatePaid,
                DateCreated = operation.DateCompleted
            };
        }

        public ICollection<DetailedCheck> MakeChecks(ICollection<PosOperation> operations)
        {
            return operations.Select(MakeCheck).AsParallel().ToImmutableList();
        }
    }
}