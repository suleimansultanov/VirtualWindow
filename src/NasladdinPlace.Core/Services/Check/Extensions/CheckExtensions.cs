using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Check.Detailed.Makers;
using NasladdinPlace.Core.Services.Check.Detailed.Makers.Contracts;
using NasladdinPlace.Core.Services.Check.Detailed.Makers.Utilities.LabeledGoodsGrouper;
using NasladdinPlace.Core.Services.Check.Detailed.Makers.Utilities.LabeledGoodsGrouper.Contracts;
using NasladdinPlace.Core.Services.Check.Detailed.Mappers;
using NasladdinPlace.Core.Services.Check.Detailed.Mappers.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers;
using NasladdinPlace.Core.Services.Check.Simple.Makers.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Mappers;
using NasladdinPlace.Core.Services.Check.Simple.Mappers.Contracts;
using NasladdinPlace.Core.Services.Configuration.Reader;

namespace NasladdinPlace.Core.Services.Check.Extensions
{
    public static class CheckExtensions
    {
	    public static void AddDetailedCheck( this IServiceCollection services,
		    IConfigurationReader configurationReader )
	    {
		    var adminPageBaseUrl = configurationReader.GetAdminPageBaseUrl();
		    var baseApiUrl = configurationReader.GetBaseApiUrl();
		    var fiscalizationQrCodeUrlTemplate = configurationReader.GetFiscalizationQrCodeUrlTemplate();
		    var fiscalCheckUrlTemplate = configurationReader.GetFiscalCheckUrlTemplate();


		    services.AddTransient<IDetailedCheckGoodInstanceCreator, DetailedCheckGoodInstanceCreator>();
		    services.AddTransient<IDetailedCheckMaker>( sp => new DetailedCheckMaker(
				    sp.GetRequiredService<IDetailedCheckGoodInstanceCreator>(),
				    ConfigurationReaderExt.CombineUrlParts( baseApiUrl, fiscalizationQrCodeUrlTemplate ),
				    ConfigurationReaderExt.CombineUrlParts( adminPageBaseUrl, fiscalCheckUrlTemplate )
			    )
		    );
	    }

	    public static void AddSimpleCheck(this IServiceCollection services)
        {
            services.AddTransient<ILabeledGoodsByGoodGrouper, LabeledGoodsByGoodGrouper>();
            services.AddTransient<ISimpleCheckMapper, SimpleCheckMapper>();
            services.AddTransient<ISimpleCheckMaker, SimpleCheckMaker>();
            services.AddTransient<IUserLatestOperationCheckMaker>(sp =>
                new UserLatestOperationCheckMaker(
                    sp.GetRequiredService<IUnitOfWorkFactory>(),
                    sp.GetRequiredService<ISimpleCheckMaker>()
                )
            );
        }
    }
}
