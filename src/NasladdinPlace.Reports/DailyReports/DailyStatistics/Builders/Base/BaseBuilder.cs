using NasladdinPlace.Core;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;
using System;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Base
{
    public abstract class BaseBuilder
    {
        protected readonly IUnitOfWorkFactory _unitOfWorkFactory;
        protected readonly DailyStatisticsConfigurationModel _configurationModel;

        protected BaseBuilder(IUnitOfWorkFactory unitOfWorkFactory, DailyStatisticsConfigurationModel configurationModel)
        {
            if (configurationModel == null)
                throw new ArgumentNullException(nameof(configurationModel));
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            _unitOfWorkFactory = unitOfWorkFactory;
            _configurationModel = configurationModel;
        }
    }
}
