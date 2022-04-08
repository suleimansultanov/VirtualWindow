using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.WebSocket.Factories;
using NasladdinPlace.Api.Services.WebSocket.Factories.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Models;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Extensions
{
    public static class PosSensorsMeasurementsExtensions
    {
	    public static void AddPosSensorsMeasurementsManagement( this IServiceCollection services,
		    IConfigurationReader configurationReader )
	    {
		    var posSensorMeasurementsSettingsModel = new PosSensorMeasurementsSettingsModel();

		    var lowerNormalAmperage = configurationReader.GetLowerNormalAmperage();
		    var upperNormalAmperage = configurationReader.GetUpperNormalAmperage();
		    var frontPanelPositionAbnormalValueAsInt = configurationReader.GetFrontPanelPositionAbnormalValue();

		    posSensorMeasurementsSettingsModel.FrontPanelPositionAbnormalPosition =
			    TryGetFrontPanelPositionFromInt( frontPanelPositionAbnormalValueAsInt );
		    posSensorMeasurementsSettingsModel.LowerNormalAmperage = lowerNormalAmperage;
		    posSensorMeasurementsSettingsModel.UpperNormalAmperage = upperNormalAmperage;

		    services.AddSingleton<IPosSensorsMeasurementsManagerFactory>( sp =>
			    new PosSensorsMeasurementsManagerFactory(
				    sp.GetRequiredService<ITelegramChannelMessageSender>(),
				    posSensorMeasurementsSettingsModel,
					sp.GetRequiredService<ILogger>()
			    ) );

		    services.AddTransient<IPosSensorControllerMeasurementsTracker, PosSensorControllerMeasurementsTracker>();
	    }

	    private static FrontPanelPosition TryGetFrontPanelPositionFromInt(int value)
        {
            if (!Enum.IsDefined(typeof(FrontPanelPosition), value))
            {
                throw new NotSupportedException($"Abnormal value {value} of front panel position is not defined");
            }

            return (FrontPanelPosition)value;
        }
    }
}
