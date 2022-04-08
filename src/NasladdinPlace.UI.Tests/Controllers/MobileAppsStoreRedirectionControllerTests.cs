using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.TestUtils;
using NasladdinPlace.UI.Controllers;
using NasladdinPlace.UI.Tests.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement;

namespace NasladdinPlace.UI.Tests.Controllers
{
    public class MobileAppsStoreRedirectionControllerTests : TestsBase
    {
        private const int DefaultPosId = 1;

        private MobileAppsStoreRedirection _mobileAppsStoreRedirection;
        private IStatelessPosTokenManager _statelessPosTokenManager;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var serviceProvider = new ServiceProviderFactory().CreateServiceProvider(Context);

            _mobileAppsStoreRedirection = serviceProvider.GetRequiredService<MobileAppsStoreRedirection>();
            _statelessPosTokenManager = serviceProvider.GetRequiredService<IStatelessPosTokenManager>();
        }

        [Test]
        public void ConfigUrlAndControllerMethodLink_ValidConfigLink_ShouldBeSame()
        {
            var posQrCode = Guid.NewGuid();

            var configUrlString = $"https://online.nasladdin.club/MobileAppsStoreRedirection/Index?qrCode={posQrCode}";
            var configPathAndQuery = new Uri(configUrlString).PathAndQuery;

            MethodBase controllerMethod = typeof(MobileAppsStoreRedirection).GetMethod(nameof(MobileAppsStoreRedirection.Index));
            ParameterInfo[] methodParameters = controllerMethod.GetParameters();
            methodParameters.Should().HaveCount(1);

            var controllerPathAndQuery = $"/{nameof(MobileAppsStoreRedirection)}/{controllerMethod.Name}?{methodParameters[0].Name}={posQrCode}";

            configPathAndQuery.Should().Equals(controllerPathAndQuery);
        }

        [Test]
        public void Index_CorrectQrCodeIsGiven_ShouldReturnView()
        {
            var posQrCode = GetDefaultQrCode();
            posQrCode.Should().NotBeNullOrEmpty();

            var isTokenValid = _statelessPosTokenManager.IsPosTokenValid(DefaultPosId, posQrCode);
            isTokenValid.Should().BeTrue();

            var qrCodeResult = _mobileAppsStoreRedirection.Index(posQrCode);
            qrCodeResult.Should().BeOfType<ViewResult>();
        }

        [Test]
        public void MobileAppsStoreRedirection_NoCustomAttributeIsGiven_CustomAttributeShouldNotBeFoundOnController()
        {
            var mobileAppStoreControllerType = _mobileAppsStoreRedirection.GetType();

            var mobileAppStoreControllerAuthorizeAttributes = typeof(MobileAppsStoreRedirection).GetTypeInfo()
                .GetCustomAttributes(typeof(AuthorizeAttribute), false);

            mobileAppStoreControllerType.CustomAttributes.Count().Should().Be(0);
            mobileAppStoreControllerAuthorizeAttributes.Length.Should().Be(0);
        }

        [Test]
        public void Index_NoCustomAttributeIsGiven_CustomAttributeShouldNotBeFoundOnMethod()
        {
            var mobileAppStoreControllerType = _mobileAppsStoreRedirection.GetType();
            var methodInfo = mobileAppStoreControllerType.GetMethod("Index");

            var indexAuthorizeAttributes = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), false);

            indexAuthorizeAttributes.Length.Should().Be(0);
        }

        private string GetDefaultQrCode()
        {
            return _statelessPosTokenManager.GeneratePosToken(DefaultPosId);
        }
    }
}
