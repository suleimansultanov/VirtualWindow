using System;

namespace NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings
{
    public class PosTemperatureMeasurementsSettings
    {
        public double LowerNormalTemperature { get; }
        public double UpperNormalTemperature { get; }
        public TimeSpan AverageTemperaturePeriodForComputation { get; }
        public TimeSpan TemperatureUpdateMaxDelay { get; }
        public TimeSpan AbnormalTemperatureAlertMutingPeriodAfterAdminPosOperation { get; }

        public PosTemperatureMeasurementsSettings(
            double lowerNormalTemperature,
            double upperNormalTemperature,
            int calcAverageTemperatureEveryInMinutes,
            int noTemperatureUpdatesTimeoutInMinutes,
            int abnormalTemperatureAlertMutingPeriodAfterAdminPosOperationInMinutes)
        {
            if (calcAverageTemperatureEveryInMinutes < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(calcAverageTemperatureEveryInMinutes),
                    calcAverageTemperatureEveryInMinutes,
                    $"Average temperature computation period should be greater than 0, but found {calcAverageTemperatureEveryInMinutes}");

            if (noTemperatureUpdatesTimeoutInMinutes < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(noTemperatureUpdatesTimeoutInMinutes),
                    noTemperatureUpdatesTimeoutInMinutes,
                    $"Temperature update max delay should be greater than 0, but found {noTemperatureUpdatesTimeoutInMinutes}");

            if (abnormalTemperatureAlertMutingPeriodAfterAdminPosOperationInMinutes < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(abnormalTemperatureAlertMutingPeriodAfterAdminPosOperationInMinutes),
                    abnormalTemperatureAlertMutingPeriodAfterAdminPosOperationInMinutes,
                    $"Abnormal temperature alert muting period after admin posOperation should be greater than 0, but found {abnormalTemperatureAlertMutingPeriodAfterAdminPosOperationInMinutes}");

            LowerNormalTemperature = lowerNormalTemperature;
            UpperNormalTemperature = upperNormalTemperature;
            AverageTemperaturePeriodForComputation = TimeSpan.FromMinutes(calcAverageTemperatureEveryInMinutes);
            TemperatureUpdateMaxDelay = TimeSpan.FromMinutes(noTemperatureUpdatesTimeoutInMinutes);
            AbnormalTemperatureAlertMutingPeriodAfterAdminPosOperation =
                TimeSpan.FromMinutes(abnormalTemperatureAlertMutingPeriodAfterAdminPosOperationInMinutes);
        }
    }
}