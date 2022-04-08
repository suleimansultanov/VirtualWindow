using NasladdinPlace.CheckOnline.Builders.CheckOnline.Providers;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Validators;
using NasladdinPlace.CheckOnline.Infrastructure;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Factory
{
    public static class CheckOnlineBuilderFactory
    {
        public static ICheckOnlineBuilder Create(ICheckOnlineRequestProvider checkOnlineRequestProvider)
        {
            var checkOnlineModelValidator = new CheckOnlineModelValidator();
            var requestModelValidator = new RequestModelValidator();
            var correctionModelValidator = new CheckOnlineCorrectionModelValidator();
            return new CheckOnlineBuilder(
                checkOnlineRequest: checkOnlineRequestProvider,
                checkOnlineModelValidator: checkOnlineModelValidator,
                requestModelValidator: requestModelValidator,
                checkOnlineCorrectionValidator: correctionModelValidator);
        }
    }
}
