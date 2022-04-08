using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Api.Services.Spreadsheet.Creators;
using NasladdinPlace.Api.Services.Spreadsheet.Creators.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Models;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.Check.Detailed.Makers.Contracts;

namespace NasladdinPlace.Api.Services.Spreadsheet.Providers
{
    public class PurchaseReportDataBatchProvider : IReportDataBatchProvider
    {
        private readonly IPurchaseReportRecordsCreator _purchaseReportRecordsCreator;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IDetailedCheckMaker _detailedCheckMaker;
        private readonly TimeSpan _reportDataExportingPeriodInDays;

        public PurchaseReportDataBatchProvider(IServiceProvider serviceProvider, TimeSpan reportDataExportingPeriodInDays)
        {
            _detailedCheckMaker = serviceProvider.GetRequiredService<IDetailedCheckMaker>();
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _purchaseReportRecordsCreator = serviceProvider.GetRequiredService<IPurchaseReportRecordsCreator>();
            _reportDataExportingPeriodInDays = reportDataExportingPeriodInDays;
        }

        public IEnumerable<RecordsWithUploadingProgress> Provide(int batchSize)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var posOperationsPagedBatches = unitOfWork.PosOperations.GetDetailedPaidHavingCheckItemsForRecentPeriod(batchSize, _reportDataExportingPeriodInDays);
                var uploadingProgressTracker = new SpreadsheetUploadingProgressTracker(posOperationsPagedBatches.TotalItemsCount);
                foreach (var posOperations in posOperationsPagedBatches)
                {
                    var checks = _detailedCheckMaker.MakeChecks(posOperations.ToList());

                    var records = _purchaseReportRecordsCreator.CreateFromDetailedChecks(checks);

                    var uploadingProgress = uploadingProgressTracker.Track(posOperations.Count());

                    yield return new RecordsWithUploadingProgress(records, uploadingProgress);
                }
            }
        }
    }
}
