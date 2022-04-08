using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.Spreadsheet.Creators;
using NasladdinPlace.Api.Services.Spreadsheet.Models;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Goods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NasladdinPlace.Api.Services.Spreadsheet.Providers
{
    public class PointsOfSaleContentReportDataBatchProvider : IReportDataBatchProvider
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PointsOfSaleContentReportDataBatchProvider(IServiceProvider serviceProvider)
        {
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
        }

        public IEnumerable<RecordsWithUploadingProgress> Provide(int batchSize)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var tiedLabeledGoods = unitOfWork.LabeledGoods.GetAllTiedIncludingGoodAndPosAndCurrencyAndCategory();
                var uploadingProgressTracker = new SpreadsheetUploadingProgressTracker(tiedLabeledGoods.Count);

                var records = CreateReportRecords(tiedLabeledGoods);
                var uploadingProgress = uploadingProgressTracker.Track(tiedLabeledGoods.Count);

                return new List<RecordsWithUploadingProgress>
                {
                    new RecordsWithUploadingProgress(records, uploadingProgress)
                };
            }
        }

        private IEnumerable<IReportRecord> CreateReportRecords(IEnumerable<LabeledGood> batchTiedLabeledGoods)
        {
            var reportItems = new List<IReportRecord>();

            var groupLabeledGoodsByGoodAndPrice = batchTiedLabeledGoods.GroupBy(lg => new { lg.GoodId, lg.Price, lg.PosId });

            foreach (var groupLabeledGood in groupLabeledGoodsByGoodAndPrice)
            {
                var labeledGood = groupLabeledGood.First();
                var pricePerItem = labeledGood.Price ?? 0M;
                var goodCount = groupLabeledGood.Count();

                reportItems.Add(new PosGoodReportRecord
                {
                    PosId = labeledGood.PosId ?? Core.Models.Pos.Default.Id,
                    PosName = labeledGood.Pos.Name ?? Core.Models.Pos.Default.Name,
                    GoodId = labeledGood.GoodId ?? Good.Unknown.Id,
                    GoodName = labeledGood.Good.Name ?? Good.Unknown.Name,
                    GoodCount = goodCount,
                    GoodCategoryId = labeledGood.Good.GoodCategoryId,
                    GoodCategory = labeledGood.Good.GoodCategory.Name,
                    PricePerItem = pricePerItem,
                    Price = pricePerItem * goodCount
                });
            }

            return reportItems;
        }
    }
}
