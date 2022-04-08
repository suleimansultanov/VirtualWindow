using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.Spreadsheet.Creators;
using NasladdinPlace.Api.Services.Spreadsheet.Models;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Utilities.DateTimeConverter;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Api.Services.Spreadsheet.Providers
{
    public class GoodsMovingReportDataBatchProvider : IReportDataBatchProvider
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public GoodsMovingReportDataBatchProvider(IServiceProvider serviceProvider)
        {
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
        }

        public IEnumerable<RecordsWithUploadingProgress> Provide(int batchSize)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var reportPeriodInTicks = TimeSpan.TicksPerDay;
                var moscowYesterday = UtcMoscowDateTimeConverter.MoscowNow.AddTicks(-reportPeriodInTicks).Date;
                var dateStart = UtcMoscowDateTimeConverter.ConvertToUtcDateTime(moscowYesterday);
                var dateEnd = dateStart.AddTicks(reportPeriodInTicks);
                var range = DateTimeRange.From(dateStart, dateEnd);
                var documentsGoodsMovingPagedBatches =
                    unitOfWork.DocumentsGoodsMoving.GetNotDeletedWithinPeriodIncludingTablePartGoodWithCategoryAndPos(batchSize, range);

                var uploadingProgressTracker = new SpreadsheetUploadingProgressTracker(documentsGoodsMovingPagedBatches.TotalItemsCount);

                foreach (var documents in documentsGoodsMovingPagedBatches)
                {
                    var records = documents.SelectMany(d => d.TablePart.OrderBy(r => r.Id), MapToGoodMovingReportRecord);

                    var uploadingProgress = uploadingProgressTracker.Track(documents.Count());

                    yield return new RecordsWithUploadingProgress(records, uploadingProgress);
                }
            }
        }

        private GoodMovingReportRecord MapToGoodMovingReportRecord(DocumentGoodsMoving document, DocumentGoodsMovingTableItem tablePart)
        {
            return Mapper.Map<GoodMovingReportRecord>(tablePart);
        }
    }
}