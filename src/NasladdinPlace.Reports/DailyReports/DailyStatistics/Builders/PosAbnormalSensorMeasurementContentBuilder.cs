using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Base;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.Base;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Interfaces;
using NasladdinPlace.Utilities.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders
{
    public class PosAbnormalSensorMeasurementContentBuilder : BaseBuilder, IDailyStatisticsContentBuilder
    {
        public PosAbnormalSensorMeasurementContentBuilder(
            IUnitOfWorkFactory unitOfWorkFactory,
            DailyStatisticsConfigurationModel configurationModel) : base(unitOfWorkFactory, configurationModel)
        {
        }

        public async Task<BaseContent> BuildContentWithLinkAsync(DateTimeRange reportDateTimeRange)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var posAbnormalSensorMeasurements =
                    await unitOfWork.PosAbnormalSensorMeasurements.GetForDateRangeAsync(
                        reportDateTimeRange.Start, reportDateTimeRange.End);

                var abnormalTemperatureCount =
                    posAbnormalSensorMeasurements.Count(ps => ps.Type == PosSensorType.Temperature);

                return new PosAbnormalSensorMeasurementContent(abnormalTemperatureCount, _configurationModel.PosAbnormalSensorMeasurementCountLink);
            }
        }
    }
}
