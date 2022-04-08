using System;

namespace NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings
{
    public class PosStateHistoricalDataSettings
    {
        public TimeSpan PosStateDataLifeTimePeriod { get; }
        public TimeSpan DeletingObsoleteHistoricalDataStartTime { get; }
        
        public PosStateHistoricalDataSettings(
            int posStateDataLifeTimePeriodInDays,
            TimeSpan deletingObsoleteHistoricalDataStartTime)
        {
            if (posStateDataLifeTimePeriodInDays < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(posStateDataLifeTimePeriodInDays),
                    posStateDataLifeTimePeriodInDays,
                    "Pos state data lifetime period hould be greater than 0");

            PosStateDataLifeTimePeriod =
                TimeSpan.FromDays(posStateDataLifeTimePeriodInDays);
            DeletingObsoleteHistoricalDataStartTime = deletingObsoleteHistoricalDataStartTime;
        }
    }
}