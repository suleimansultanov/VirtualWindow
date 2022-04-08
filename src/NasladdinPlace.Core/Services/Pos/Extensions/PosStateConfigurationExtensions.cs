using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureSettings;

namespace NasladdinPlace.Core.Services.Pos.Extensions
{
    public static class PosStateConfigurationExtensions
    {
        public static void AddPosStateSettingsProvider(this IServiceCollection services, IConfigurationReader configurationReader)
        {
	        PosStateChartSettings posStateChartSettings;

            var lowerNormalTemperature = configurationReader.GetLowerNormalTemperature();
            var upperNormalTemperature = configurationReader.GetUpperNormalTemperature();
            var calcAverageTemperatureEveryInMinutes = configurationReader.GetCalcAverageTemperatureEveryInMinutes();
            var notifyIfNoTemperatureUpdatesMoreThanInMinutes =
	            configurationReader.GetNotifyIfNoTemperatureUpdatesMoreThanInMinutes();
            var preventNotifyAbnormalTemperatureAfterAdminOperationInMinutes =
	            configurationReader.GetPreventNotifyAbnormalTemperatureAfterAdminOperationInMinutes();

            var posTemperatureMeasurementsSettings = new PosTemperatureMeasurementsSettings(
	            lowerNormalTemperature,
	            upperNormalTemperature,
	            calcAverageTemperatureEveryInMinutes,
	            notifyIfNoTemperatureUpdatesMoreThanInMinutes,
	            preventNotifyAbnormalTemperatureAfterAdminOperationInMinutes
            );

            var posStateDataLifeTimePeriodInDays = configurationReader.GetPosStateDataLifeTimePeriodInDays();
            var deletingObsoleteHistoricalDataStartTime =
	            configurationReader.GetDeletingObsoleteHistoricalDataStartTime();

            var posStateHistoricalDataSettings = new PosStateHistoricalDataSettings(posStateDataLifeTimePeriodInDays, deletingObsoleteHistoricalDataStartTime);

            var posStateChartMeasurementDefaultPeriodInMinutes =
	            configurationReader.GetPosStateChartMeasurementDefaultPeriodInMinutes();
            var posStateChartRefreshFrequencyInSeconds =
	            configurationReader.GetPosStateChartRefreshFrequencyInSeconds();
            var posStateChartDateTimeDisplayFormat = configurationReader.GetPosStateChartDateTimeDisplayFormat();

            posStateChartSettings = new PosStateChartSettings(
                posStateChartMeasurementDefaultPeriodInMinutes,
                posStateChartRefreshFrequencyInSeconds,
                posStateChartDateTimeDisplayFormat);

            services.AddSingleton<IPosStateSettingsProvider>(sp => new PosStateSettingsProvider(
                new PosStateSetting(
                    posTemperatureMeasurementsSettings,
                    posStateHistoricalDataSettings,
                    posStateChartSettings)));

        }
    }
}
