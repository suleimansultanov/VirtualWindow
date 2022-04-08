using System;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using NasladdinPlace.UI.Services.Authorization;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Contracts;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Models;
using NasladdinPlace.UI.Tests.Scenarios.Auth.Models;

namespace NasladdinPlace.UI.Tests.Scenarios.Auth
{
    public class AuthorizationPermissionApiHandlerShould 
    {
        private const string DefaultUserId = "1";
        private const string CookieKeyAuthToken = "AuthToken";

        private AuthorizationHandlerContext _context;
        private AuthorizationPermissionApiHandler _authorizationPermissionApiHandler;
        private Mock<IAuthTokenManager> _mockAuthTokenManager;
        private Mock<HttpContext> _httpContextMock;

        [SetUp]
        public void SetUp()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, DefaultUserId)
                    },
                    "sameAuthTypeName"
                )
            );

            var responseCookiesMock = new Mock<IResponseCookies>();
            var requestCookiesMock = new Mock<IRequestCookieCollection>();

            var httpResponseMock = new Mock<HttpResponse>();
            httpResponseMock.Setup(mock => mock.Cookies).Returns(responseCookiesMock.Object);

            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(mock => mock.Cookies).Returns(requestCookiesMock.Object);

            _httpContextMock = new Mock<HttpContext>();

            _httpContextMock.Setup(mock => mock.Response).Returns(httpResponseMock.Object);
            _httpContextMock.Setup(mock => mock.Request).Returns(httpRequestMock.Object);

            responseCookiesMock.Setup(r => r.Append(CookieKeyAuthToken,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("fr-CA")),
                It.IsAny<CookieOptions>()));

            requestCookiesMock.Setup(r => r.Keys).Returns(new List<string>() {CookieKeyAuthToken});

            var authFilterContext = new AuthorizationFilterContext(
                new ActionContext(_httpContextMock.Object, new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>());

            _context = new AuthorizationHandlerContext(new[] {new TestAuthorizationRequirement()}, user, authFilterContext);

            _mockAuthTokenManager = new Mock<IAuthTokenManager>();

            _authorizationPermissionApiHandler = new AuthorizationPermissionApiHandler(_mockAuthTokenManager.Object);
        }
        
        [Test]
        public void ShouldReturnSucceed()
        {
            _mockAuthTokenManager.Setup(mock => mock.RetrieveAsync())
                .Returns(Task.FromResult(new AuthToken(CookieKeyAuthToken, TimeSpan.FromDays(1))));

            _authorizationPermissionApiHandler.HandleAsync(_context).Wait();

            _context.HasSucceeded.Should().BeTrue();
            _httpContextMock.Invocations.Count.Should().Be(0);
        }

        [Test]
        public void ShouldRedirectToLoginPageAndClearCookies()
        {
            _mockAuthTokenManager.Setup(mock => mock.RetrieveAsync())
                .Returns(Task.FromResult((AuthToken) null));

            _authorizationPermissionApiHandler.HandleAsync(_context).Wait();

            _context.HasSucceeded.Should().BeFalse();
            _httpContextMock.Invocations.Count.Should().Be(2);
        }
    }
}