using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.ViewModels.Checks;
using NasladdinPlace.Utilities.DateTimeConverter;
using System;
using System.Linq;

namespace NasladdinPlace.UI.Managers.Reference.UniReferencesManagers.Builders
{
    public class PosOperationsViewModelQueryBuilder
    {
        private readonly UniReferenceDataProviderGetParameters _parameters;
        private IQueryable<PosOperation> _query;

        public PosOperationsViewModelQueryBuilder(UniReferenceDataProviderGetParameters parameters, IUnitOfWork unitOfWork)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            _parameters = parameters;
            _query = unitOfWork.PosOperations.GetDetailedIncludingOnlyRequiredFields();
        }

        public PosOperationsViewModelQueryBuilder ApplyTotalPriceContextFilter()
        {
            var filterContextTotalPrice = _parameters.Filter.GetItem(x => x.FilterName == nameof(PosOperationViewModel.FilterContextTotalPrice));
            var filterContextTotalPriceValue = filterContextTotalPrice?.GetValueDecimalNullable();

            var fromDate = _parameters.Filter.GetItem(x => x.FilterName == "DateTimeFrom").GetDateTimeValueNullable();
            var toDate = _parameters.Filter.GetItem(x => x.FilterName == "DateTimeTo").GetDateTimeValueNullable();

            if (filterContextTotalPrice != null)
            {
                if (fromDate == null && toDate != null)
                    _query = _query.Where(q => q.CheckItems.Any(ci => ci.Status == CheckItemStatus.Unpaid) &&
                                             q.CheckItems.Sum(c => c.Price) > filterContextTotalPriceValue.Value && q.DateStarted <= toDate);
                else
                    _query = _query.Where(q => q.CheckItems.Any() &&
                                             q.CheckItems.Sum(c => c.Price) > filterContextTotalPriceValue.Value);
            }

            return this;
        }

        public PosOperationsViewModelQueryBuilder ApplyStatusContextFilter()
        {
            var filterContextStatus = _parameters.Filter.GetItem(x => x.FilterName == nameof(PosOperationViewModel.FilterContextStatus));

            if (filterContextStatus == null)
                return this;

            var filterContextStatusValue = (PosOperationStatus?)filterContextStatus.GetValueIntNullable();

            if (filterContextStatusValue == null)
                return this;

            switch (filterContextStatus.FilterType)
            {
                case FilterTypes.Equals:
                    _query = _query.Where(q => q.Status == filterContextStatusValue.Value);
                    break;
                case FilterTypes.Less:
                    _query = _query.Where(q => q.Status < filterContextStatusValue.Value);
                    break;
                case FilterTypes.Contains:
                case FilterTypes.Greater:
                case FilterTypes.GreaterOrEquals:
                case FilterTypes.LessOrEquals:
                case FilterTypes.None:
                default:
                    throw new NotSupportedException(
                        $"Unable to find the specified filter type {filterContextStatus.FilterType}"
                    );
            }

            return this;
        }

        public PosOperationsViewModelQueryBuilder ApplyOperationModeFilter()
        {
            var operationModeFilter = _parameters.Filter.GetValueInt(nameof(PosOperationViewModel.PosOperationMode));

            if (operationModeFilter != null)
                _query = _query.Where(po => po.Mode == (PosMode)operationModeFilter.Value);

            return this;
        }

        public PosOperationsViewModelQueryBuilder ApplyFromToDateFilterByStatus()
        {
            var statusFilter = _parameters.Filter.GetValueInt(nameof(PosOperationViewModel.Status));
            var statusFilterValue = statusFilter != null ? (PosOperationStatus?)statusFilter : null;

            var fromDate = _parameters.Filter.GetItem(x => x.FilterName == "DateTimeFrom").GetDateTimeValueNullable();
            var toDate = _parameters.Filter.GetItem(x => x.FilterName == "DateTimeTo").GetDateTimeValueNullable();

            if (!fromDate.HasValue && toDate.HasValue)
            {
                var utcToDate = UtcMoscowDateTimeConverter.ConvertToUtcDateTime(toDate.Value);

                if (statusFilterValue == null)
                    _query = _query.Where(po => po.DateCompleted < utcToDate)
                        .OrderByDescending(po => po.DateCompleted);
            }

            if (fromDate.HasValue && toDate.HasValue)
            {
                var utcFromDate = UtcMoscowDateTimeConverter.ConvertToUtcDateTime(fromDate.Value);
                var utcToDate = UtcMoscowDateTimeConverter.ConvertToUtcDateTime(toDate.Value);

                switch (statusFilterValue)
                {
                    case PosOperationStatus.Opened:
                    case PosOperationStatus.PendingCheckCreation:
                        _query = _query.Where(po =>
                                (po.CompletionInitiationDate ?? po.DateStarted) >= utcFromDate &&
                                (po.CompletionInitiationDate ?? po.DateStarted) <= utcToDate)
                            .OrderByDescending(po => po.CompletionInitiationDate ?? po.DateStarted);
                        break;
                    case PosOperationStatus.Completed:
                    case PosOperationStatus.PendingPayment:
                        _query = _query.Where(po => po.DateCompleted >= utcFromDate && po.DateCompleted <= utcToDate)
                            .OrderByDescending(po => po.DateCompleted);
                        break;
                    case PosOperationStatus.Paid:
                        _query = _query.Where(po => po.DatePaid >= utcFromDate && po.DatePaid <= utcToDate)
                            .OrderByDescending(po => po.DatePaid);
                        break;
                    case null:
                        _query = _query.Where(po => (po.DatePaid ?? po.DateCompleted ?? po.DateStarted) >= utcFromDate &&
                                                  (po.DatePaid ?? po.DateCompleted ?? po.DateStarted) <= utcToDate)
                            .OrderByDescending(po => po.DatePaid ?? po.DateCompleted ?? po.DateStarted);
                        break;
                    default:
                        throw new NotSupportedException(
                            $"Unable to find the specified {nameof(PosOperationStatus)} {statusFilterValue}."
                        );
                }
            }

            return this;
        }

        public PosOperationsViewModelQueryBuilder ApplyUnverifiedChekItemsFilter()
        {
            var hasUnverifiedCheckItemsFilter =
                _parameters.Filter.GetValue(nameof(PosOperationViewModel.HasUnverifiedCheckItems));
            if (hasUnverifiedCheckItemsFilter != null &&
                bool.TryParse(hasUnverifiedCheckItemsFilter, out var result) &&
                result)
            {
                _query = _query.Where(po =>
                    po.CheckItems.Any(ci =>
                        ci.Status == CheckItemStatus.Unverified || ci.Status == CheckItemStatus.PaidUnverified) &&
                    po.Mode == PosMode.Purchase);
            }

            return this;
        }

        public PosOperationsViewModelQueryBuilder ApplyPaymentCardCryptogramSourceFilter()
        {
            var filterPaymentCardCryptogramSource =
                _parameters.Filter.GetValue(nameof(PosOperationViewModel.FilterPaymentCardCryptogramSource));

            if (filterPaymentCardCryptogramSource != null &&
                int.TryParse(filterPaymentCardCryptogramSource, out var сryptogramSource))
            {
                _query = _query.Where(po => po.BankTransactionInfos.Any(bti =>
                    bti.PaymentCard.CryptogramSource == (PaymentCardCryptogramSource)сryptogramSource));
            }

            return this;
        }

        public PosOperationsViewModelQueryBuilder ApplyFiscalizationInfoErrorsFilter()
        {
            var hasFiscalizationInfoErrorsFilter =
                _parameters.Filter.GetValue(nameof(PosOperationViewModel.HasFiscalizationInfoErrors));
            if (hasFiscalizationInfoErrorsFilter != null &&
                bool.TryParse(hasFiscalizationInfoErrorsFilter, out var result) &&
                result)
            {
                _query = _query.Where(po =>
                    po.PosOperationTransactions.Any(pot => pot.Status == PosOperationTransactionStatus.PaidUnfiscalized) &&
                    po.Mode == PosMode.Purchase);
            }

            return this;
        }

        public IQueryable<PosOperation> Build()
        {
            return _query;
        }
    }
}
