using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.Core.Services.Authorization.Contracts;
using NasladdinPlace.Core.Services.Authorization.Providers;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.TestUtils.Utils;
using NasladdinPlace.UI.Controllers;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Tests.DependencyInjection;
using NasladdinPlace.UI.ViewModels.Logs;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Threading.Tasks;
using LogsController = NasladdinPlace.UI.Controllers.Api.LogsController;

namespace NasladdinPlace.UI.Tests.Scenarios.Auth
{
    public class PermissionAttributeShould : TestsBase
    {
        private const string UserName = "testuser@domain.com";
        private const string UserPassword = "Testpassword_312";
        private const bool IsActive = true;
        private const int DefaultPosId = 1;
        private const int DefaultUserId = 1;

        private PointsOfSaleController _pointsOfSaleController;
        private LogsController _logsController;

        private IAccessGroupAppFeaturesAccessManager _accessManager;
        private IServiceProvider _serviceProvider;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<Role> _roleManager;
        private SignInManager<ApplicationUser> _signInManager;


        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());

            _serviceProvider = new ServiceProviderFactory().CreateServiceProvider(Context);
            
            var controllerContext = ControllerContextFactory.MakeForUserWithId(DefaultUserId);

            _pointsOfSaleController = _serviceProvider.GetRequiredService<PointsOfSaleController>();
            _pointsOfSaleController.ControllerContext = controllerContext;

            _logsController = _serviceProvider.GetRequiredService<LogsController>();
            _logsController.ControllerContext = controllerContext;

            _userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _roleManager = _serviceProvider.GetRequiredService<RoleManager<Role>>();
            _signInManager = _serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
            _accessManager = _serviceProvider.GetRequiredService<IAccessGroupAppFeaturesAccessManager>();
        }

        [Test]
        public void UserWithWrongPermissionAndWithoutAccessToPos_ShouldReturnForbidResultFromPointsOfSaleController()
        {
            PrepareUserAsync(Roles.Logistician, _pointsOfSaleController, typeof(AclManagementPermission))
                .GetAwaiter().GetResult();

            var result = _pointsOfSaleController.EditPos(DefaultPosId).GetAwaiter().GetResult();

            result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public void UserWithWrongPermission_ShouldReturnForbidResult()
        {
            PrepareUserAsync(Roles.Logistician, _pointsOfSaleController, typeof(AclManagementPermission))
                .GetAwaiter().GetResult();

            var actionExecutingContext = ExecuteActionExecutingContext(nameof(ReadOnlyAccess));

            var result = actionExecutingContext.Result;

            result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public void UserIsAdmin_ShouldReturnNull()
        {
            PrepareUserAsync(Roles.Admin, _pointsOfSaleController, typeof(ReadOnlyAccess))
                .GetAwaiter().GetResult();

            var actionExecutingContext = ExecuteActionExecutingContext(nameof(AclManagementPermission));

            var result = actionExecutingContext.Result;

            result.Should().BeNull();
        }

        [Test]
        public void UserHaveRightPermission_ShouldReturnNull()
        {
            PrepareUserAsync(Roles.Logistician, _pointsOfSaleController, typeof(DocumentGoodsMovingPermission))
                .GetAwaiter().GetResult();

            var actionContext = CreateActionContextForController(_pointsOfSaleController);

            var actionExecutingContext =
                CreateActionExecutingContextForController(actionContext, _pointsOfSaleController);

            var next = new ActionExecutionDelegate(() => Task.FromResult(CreateActionExecutedContext(actionExecutingContext)));

            var filter = new AuthorizeResourceFilter(_accessManager, nameof(DocumentGoodsMovingPermission), _roleManager);

            filter.OnActionExecutionAsync(actionExecutingContext, next).GetAwaiter().GetResult();
            var result = actionExecutingContext.Result;

            result.Should().BeNull();
        }

        [Test]
        public void UserHaveRightPermission_ShouldReturnNullAndOkResultFromApi()
        {
            PrepareUserAsync(Roles.Admin, _logsController, typeof(ReadOnlyAccess))
                .GetAwaiter().GetResult();

            var actionContext = CreateActionContextForController(_logsController);

            var actionExecutingContext =
                CreateActionExecutingContextForController(actionContext, _logsController);

            var next = new ActionExecutionDelegate(() => Task.FromResult(CreateActionExecutedContext(actionExecutingContext)));

            var filter = new AuthorizeResourceFilter(_accessManager, nameof(ReadOnlyAccess), _roleManager);

            filter.OnActionExecutionAsync(actionExecutingContext, next).GetAwaiter().GetResult();
            var result = actionExecutingContext.Result;

            result.Should().BeNull();

            var posViewModel = new PosLogViewModel
            {
                PosId = DefaultPosId
            };

            var response = _logsController.RequestLogsAsync(posViewModel).GetAwaiter().GetResult();

            response.Should().BeOfType<OkResult>();
        }

        private async Task PrepareUserAsync(string roleName, Controller controller, Type appFeature)
        {
            var user = await CreateUserAsync(UserName, UserPassword, roleName);

            var claimsPrincipal = await SignInAsync(user);

            controller.ControllerContext.HttpContext.User = claimsPrincipal;

            await CreatePermissionAndGrantAccessAsync(roleName, appFeature);
        }

        private async Task<ApplicationUser> CreateUserAsync(string userName, string password, string roleName)
        {
            var user = await CreateUserFromManagerAsync(userName, password);
            user.SetUserActivity(IsActive);
            await CreateAndAddRoleToUser(user, roleName);
            return user;
        }

        private async Task CreatePermissionAndGrantAccessAsync(string roleName, Type appFeature)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            var appFeatureRepository = _serviceProvider.GetRequiredService<IAppFeatureItemsRepository>();

            var appFeatureInitializer = (IPermissionInitializer)Activator.CreateInstance(appFeature);
            var permission = appFeatureRepository
                .GetPermissionForInitialize(appFeatureInitializer, role.Id);

            if (permission == null) return;

            Seeder.Seed(new Collection<AppFeatureItem>
            {
                permission
            });

            Context.SaveChanges();
        }

        private async Task<ApplicationUser> CreateUserFromManagerAsync(string userName, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userName
            };

            var result = await _userManager.CreateAsync(user, password);
            result.Succeeded.Should().BeTrue();

            return user;
        }

        private async Task CreateAndAddRoleToUser(ApplicationUser user, string roleName)
        {
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<Role>>();

            var roleCreationResult = await roleManager.CreateAsync(Role.FromName(roleName));
            roleCreationResult.Succeeded.Should().BeTrue();


            var roleAdditionResult = await _userManager.AddToRoleAsync(user, roleName);
            roleAdditionResult.Succeeded.Should().BeTrue();
        }

        private async Task<ClaimsPrincipal> SignInAsync(ApplicationUser user)
        {
            var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            return claimsPrincipal;
        }

        private ActionExecutedContext CreateActionExecutedContext(ActionExecutingContext context)
        {
            return new ActionExecutedContext(context, context.Filters, context.Controller)
            {
                Result = context.Result,
            };
        }

        private ActionExecutingContext CreateActionExecutingContextForController(ActionContext context, Controller controller)
        {
            return new ActionExecutingContext(
                context,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                controller
            );
        }

        private ActionContext CreateActionContextForController(Controller controller)
        {
            return new ActionContext(
                controller.HttpContext,
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>()
            );
        }

        private ActionExecutingContext ExecuteActionExecutingContext(string appFeatureName)
        {
            var actionContext = CreateActionContextForController(_pointsOfSaleController);

            var actionExecutingContext =
                CreateActionExecutingContextForController(actionContext, _pointsOfSaleController);

            var next = new ActionExecutionDelegate(() => Task.FromResult(CreateActionExecutedContext(actionExecutingContext)));

            var filter = new AuthorizeResourceFilter(_accessManager, appFeatureName, _roleManager);

            filter.OnActionExecutionAsync(actionExecutingContext, next).GetAwaiter().GetResult();

            return actionExecutingContext;
        }
    }
}
