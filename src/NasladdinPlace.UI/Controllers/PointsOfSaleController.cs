using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.Core.Services.Authorization.Contracts;
using NasladdinPlace.Core.Services.Authorization.Models;
using NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureSettings;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.Dtos;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.Helpers;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.ViewModels.PointsOfSale;
using NasladdinPlace.Utilities.EnumHelpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    public class PointsOfSaleController : ReferenceBaseController
    {
        private readonly IUserAppFeaturesAccessChecker _userAppFeaturesAccessChecker;
        private readonly IAccessGroupAppFeaturesAccessManager _accessGroupAppFeaturesAccessManager;
        private readonly PosStateChartSettings _chartSettings;
        private readonly RoleManager<Role> _roleManager;

        public PointsOfSaleController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _userAppFeaturesAccessChecker = serviceProvider.GetRequiredService<IUserAppFeaturesAccessChecker>();
            _accessGroupAppFeaturesAccessManager =
                serviceProvider.GetRequiredService<IAccessGroupAppFeaturesAccessManager>();
            _chartSettings = serviceProvider.GetRequiredService<IPosStateSettingsProvider>().GetChartSettings();
            _roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        }

        [Permission(nameof(PosCrudPermission))]
        public async Task<IActionResult> AddPos()
        {
            var cities = await UnitOfWork.Cities.GetAllAsync();

            if (!cities.Any())
                return this.RedirectToHome();

            var roles = await UnitOfWork.Roles.GetAllAsync();

            if (!roles.Any())
                return this.RedirectToHome();

            var viewModel = new PosFormViewModel
            {
                CitySelectList = cities.ToSelectList(),
                ScreenResolutionSelectList = GetSelectListFromEnum<ScreenResolutionType>(null),
                SensorControllerTypeSelectList = GetSelectListFromEnum<SensorControllerType>(null),
                PosActivityStatusSelectList = GetSelectListFromEnum<PosActivityStatus>(PosActivityStatus.Test),
                RolesMultiSelectList = RolesToMultiSelectList(roles)
            };

            return View("PosForm", viewModel);
        }

        [HttpPost]
        [Permission(nameof(PosCrudPermission))]
        public async Task<IActionResult> AddPos(PosFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return await GetPosFormOrRedirectToHome(viewModel);

            var city = await UnitOfWork.Cities.GetAsync(viewModel.CityId ?? 0);

            if (city == null)
                return BadRequest(new ErrorResponseDto { Error = $"Город с идентификатором {viewModel.CityId} не найден" });

            if (!Location.TryParse(viewModel.Latitude, viewModel.Longitude, out var location))
            {
                return BadRequest(new ErrorResponseDto
                {
                    Error = $"Широта или долгота неверны." +
                            $"Широта = {viewModel.Latitude}, долгота = {viewModel.Longitude}."
                });
            }

            var address = Address.FromCityStreetAtCoordinates(city.Id, viewModel.Street, location, viewModel.AccurateLocation);

            var pos = new Pos(
                id: 0,
                name: viewModel.Name,
                abbreviatedName: viewModel.AbbreviatedName,
                address: address,
                useNewPaymentSystem: true)
            {
                AreNotificationsEnabled = true,
                QrCodeGenerationType = viewModel.QrCodeGenerationType.Value
            };

            if (viewModel.PosActivityStatus.HasValue)
            {
                var posActivityStatus = GetEnumValue(viewModel.PosActivityStatus.Value, PosActivityStatus.Test);
                pos.ChangeActivityStatus(posActivityStatus);
            }

            foreach (var roleId in viewModel.SelectedRoles)
            {
                pos.AssignedRoles.Add(new PointsOfSaleToRole(roleId, pos.Id));
            }

            if (viewModel.RequiredScreenResolutionType.HasValue)
            {
                pos.ScreenResolutionOrNull = GetPosScreenResolution(viewModel.RequiredScreenResolutionType.Value);
            }

            pos.UpdateAllowedModes(new List<PosMode> { PosMode.GoodsPlacing });

            pos.SetRestrictedAccess(viewModel.IsRestrictedAccess);

            var userRoles = GetUserRolesFromClaims();

            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);

                var pointOfSaleToRole = new PointsOfSaleToRole(role.Id, pos.Id);

                if (!pos.AssignedRoles.Select(r => r.RoleId).Contains(role.Id))
                    pos.AssignedRoles.Add(pointOfSaleToRole);
            }

            UnitOfWork.PointsOfSale.Add(pos);

            await UnitOfWork.CompleteAsync();

            return Ok();
        }

        [Permission(nameof(ReadOnlyAccess))]
        public async Task<IActionResult> EditPos(int id)
        {
            var userRoles = GetUserRolesFromClaims();

            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);

                if (!await _accessGroupAppFeaturesAccessManager.IsAccessGrantedAsync(role.Id, id) &&
                    !User.IsInRole(nameof(Roles.Admin)))
                    return new ForbidResult();
            }

            var pos = await UnitOfWork.PointsOfSale.GetByIdIncludingAllowedOperationModesAsync(id);

            if (pos == null)
                return this.RedirectToHome();

            var cities = await UnitOfWork.Cities.GetAllAsync();

            if (!cities.Any())
                return this.RedirectToHome();

            var roles = await UnitOfWork.Roles.GetAllAsync();

            if (!roles.Any())
                return this.RedirectToHome();

            var viewModel = Mapper.Map<PosFormViewModel>(pos);
            viewModel.CitySelectList = cities.ToSelectList();

            viewModel.RolesMultiSelectList = RolesToMultiSelectList(roles);
            viewModel.SelectedRoles = pos.AssignedRoles.Select(x => x.RoleId).ToList();

            var selectedIndex = GetScreenResolutionSelectedIndex(pos.ScreenResolutionOrNull);

            viewModel.ScreenResolutionSelectList = GetSelectListFromEnum<ScreenResolutionType>(selectedIndex);
            viewModel.SensorControllerTypeSelectList = GetSelectListFromEnum<SensorControllerType>(pos.SensorControllerType);
            viewModel.PosActivityStatusSelectList = GetSelectListFromEnum<PosActivityStatus>(pos.PosActivityStatus);

            var posAllowedOperationModes = pos.AllowedModes;

            var operationModes = Enum.GetValues(typeof(PosMode))
                .OfType<PosMode>()
                .ToImmutableList();
            viewModel.SelectablePosModes = operationModes
                .Select(m => new SelectableOperationModeViewModel
                {
                    IsSelected = posAllowedOperationModes.Contains(m),
                    OperationModeAsInt = (int)m
                })
                .ToList();

            var userId = GetUserIdAsInt();
            var hasUserAccessToCreateOrEditPosModes =
                await _userAppFeaturesAccessChecker.IsAccessToFeatureGrantedAsync(
                    userId, AppFeature.AllowedPosMode_CreateOrDelete
                );
            viewModel.IsViewingPosModesAllowed = hasUserAccessToCreateOrEditPosModes;

            HttpContext.Session.SetCurrentPosId(pos.Id);

            return View("PosForm", viewModel);
        }

        [HttpPost]
        [Permission(nameof(PosCrudPermission))]
        public async Task<IActionResult> EditPos(PosFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return await GetPosFormOrRedirectToHome(viewModel);

            var pos = await UnitOfWork.PointsOfSale.GetByIdIncludingAllowedOperationModesAsync(viewModel.Id);

            if (pos == null)
                return NotFound(new ErrorResponseDto { Error = $"Информация о витрине не найдена" });

            var city = await UnitOfWork.Cities.GetAsync(viewModel.CityId ?? 0);
            if (city == null)
                return BadRequest(new ErrorResponseDto { Error = $"Город с идентификатором {viewModel.CityId} не найден" });

            if (Location.TryParse(viewModel.Latitude, viewModel.Longitude, out var location))
            {
                var address = Address.FromCityStreetAtCoordinates(city.Id, viewModel.Street, location, viewModel.AccurateLocation);
                pos.UpdateAddress(address);
            }

            if (viewModel.SelectedRoles.Count > 0)
                pos.AssignedRoles.Clear();

            foreach (var roleId in viewModel.SelectedRoles)
            {
                pos.AssignedRoles.Add(new PointsOfSaleToRole(roleId, pos.Id));
            }

            if (viewModel.RequiredScreenResolutionType.HasValue)
            {
                pos.ScreenResolutionOrNull = GetPosScreenResolution(viewModel.RequiredScreenResolutionType.Value);
            }

            if (viewModel.SensorControllerType.HasValue)
            {
                pos.SensorControllerType = GetEnumValue(viewModel.SensorControllerType.Value, SensorControllerType.Esp);
            }

            if (viewModel.PosActivityStatus.HasValue)
            {
                var posActivityStatus = GetEnumValue(viewModel.PosActivityStatus.Value, PosActivityStatus.Test);
                pos.ChangeActivityStatus(posActivityStatus);
            }

            pos.Name = viewModel.Name;
            pos.AbbreviatedName = viewModel.AbbreviatedName;
            pos.QrCodeGenerationType = viewModel.QrCodeGenerationType.Value;

            var userId = GetUserIdAsInt();

            var hasUserAccessToCreateOrEditPosModes =
                await _userAppFeaturesAccessChecker.IsAccessToFeatureGrantedAsync(
                    userId, AppFeature.AllowedPosMode_CreateOrDelete
                );

            if (hasUserAccessToCreateOrEditPosModes)
            {
                var modesToAllow = viewModel.SelectablePosModes
                    .Where(som => som.IsSelected)
                    .Select(som => som.GetOperationMode())
                    .ToList();
                modesToAllow.Add(PosMode.GoodsPlacing);
                pos.UpdateAllowedModes(modesToAllow);
            }

            pos.SetRestrictedAccess(viewModel.IsRestrictedAccess);

            await UnitOfWork.CompleteAsync();

            return Ok();
        }

        [ActionName("All")]
        [Permission(nameof(ReadOnlyAccess))]
        public IActionResult GetPointsOfSale()
        {
            var isAdmin = IsUserAdmin();

            var context = new List<FilterItemModel>();

            context.Add(new FilterItemModel
            {
                Value = isAdmin.ToString(),
                FilterType = FilterTypes.Contains,
                ForceCastType = CastTypes.None,
                PropertyName = nameof(PosViewModel.AssignedRoles),
                FilterName = nameof(PosViewModel.AssignedRoles)
            });

            return Reference<PosViewModel>(
                context: context,
                configuration: new ConfigReference
                {
                    Title = "Список витрин",
                    Actions = new List<Command>
                    {
                        new Command
                        {
                            Type = CommandType.Custom,
                            Name = "<em class='fa fa-code-fork'></em> Установить версию",
                            Binding = "click: editVersion",
                            ClassStyle = "btn-primary"
                        },
                        new Command
                        {
                            Type = CommandType.OpenUrl,
                            Name = "<em class='fa fa-plus'></em> Добавить",
                            Url = Url.Action("AddPos", "PointsOfSale"),
                            ClassStyle = "btn-primary"
                        }
                    },
                    BeforeGridPartialName = "~/Views/PointsOfSale/_beforePointsOfSaleGridPartial.cshtml",
                });
        }

        [Route("PointsOfSale/{posId}/Monitoring", Order = 1)]
        [Permission(nameof(PosMonitoringAndSupportPermission))]
        public async Task<IActionResult> Monitoring(int posId)
        {
            var pos = await UnitOfWork.PointsOfSale.GetByIdAsync(posId);

            if (pos == null)
                return this.RedirectToHome();

            ViewBag.PosId = pos.Id;

            return View("Monitoring");
        }

        [Route("PointsOfSale/{posId}/TemperatureState", Order = 1)]
        [Permission(nameof(PosMonitoringAndSupportPermission))]
        public async Task<IActionResult> TemperatureState(int posId)
        {
            var pos = await UnitOfWork.PointsOfSale.GetByIdAsync(posId);

            if (pos == null)
                return this.RedirectToHome();

            var posModel = Mapper.Map<PosBasicInfoViewModel>(pos);

            var chartRenderingViewModel = new PosEquipmentStateChartRenderingViewModel();
            chartRenderingViewModel.SetMeasurementPeriod(DateTime.UtcNow, _chartSettings);

            ViewBag.ChartRenderInfo = chartRenderingViewModel;

            return View(posModel);
        }

        [Route("PointsOfSale/{posId}/TemperatureDetails")]
        [Permission(nameof(PosMonitoringAndSupportPermission))]
        public IActionResult GetTemperatureDetails(int posId)
        {
            return Reference<PosTemperatureDetailsViewModel>(

                configuration: new ConfigReference
                {
                    Title = "Последние температуры",
                    BreadCrumbs = new List<string>
                    {
                        GetBreadCrumb(Url.Action("All", "PointsOfSale"), "Домой"),
                        GetBreadCrumb(Url.Action("EditPos", "PointsOfSale", new {id = posId}), "Карточка витрины"),
                        GetBreadCrumb(Url.Action("Monitoring", "PointsOfSale", new {posId = posId}),
                            "Мониторинг состояния")
                    },
                    SetDefaultValueFilter = (filter, infos) =>
                    {
                        filter.SetSort(nameof(PosTemperatureDetailsViewModel.DateTimeTemperatureReceipt),
                            SortTypes.Desc);
                        filter.SetValueFilter(nameof(PosTemperatureDetailsViewModel.PosId), posId);
                    },
                    IsRenderModalFilter = false,
                    AfterGridPartialName = "~/Views/PointsOfSale/_afterGridTemperatureDetailsPartial.cshtml"
                });
        }

        private async Task<IActionResult> GetPosFormOrRedirectToHome(PosFormViewModel viewModel)
        {
            var cities = await UnitOfWork.Cities.GetAllAsync();

            if (!cities.Any())
                return this.RedirectToHome();

            viewModel.CitySelectList = cities.ToSelectList();
            viewModel.ScreenResolutionSelectList = GetSelectListFromEnum<ScreenResolutionType>(null);
            viewModel.SensorControllerTypeSelectList = GetSelectListFromEnum<SensorControllerType>(null);

            return View("PosForm", viewModel);
        }

        private SelectList GetSelectListFromEnum<T>(object selectedValue) where T : struct
        {
            selectedValue = selectedValue == null ? null : (object)(int)selectedValue;
            return new SelectList(
                Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .Select(x => new SelectListItem
                    {
                        Text = (x as Enum)?.GetDescription(),
                        Value = Convert.ToInt32(x).ToString(),
                    }), "Value", "Text", selectedValue);
        }

        private int GetUserIdAsInt()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        private static ScreenResolution? GetPosScreenResolution(int typeAsInt)
        {
            if (Enum.IsDefined(typeof(ScreenResolutionType), typeAsInt) &&
                ScreenResolutionsProvider.TryProvideResolutionForType(
                    (ScreenResolutionType)typeAsInt, out var posScreenResolution))
            {
                return posScreenResolution;
            }

            return null;
        }

        private static T GetEnumValue<T>(int typeAsInt, T defaultValue) where T : struct
        {
            if (Enum.IsDefined(typeof(T), typeAsInt))
            {
                return (T)(object)typeAsInt;
            }

            return defaultValue;
        }

        private static object GetScreenResolutionSelectedIndex(ScreenResolution? screenResolution)
        {
            return screenResolution.HasValue && ScreenResolutionsProvider.TryProvideTypeForResolution(
                       screenResolution.Value,
                       out var resolutionType)
                ? (int)resolutionType
                : (object)null;
        }

        private MultiSelectList RolesToMultiSelectList(List<Role> roles)
        {
            return new MultiSelectList(roles, nameof(Role.Id), nameof(Role.Name));
        }

        private bool IsUserAdmin()
        {
            return User.IsInRole(nameof(Roles.Admin));
        }

        private IEnumerable<string> GetUserRolesFromClaims()
        {
            return User.Claims.Where(r => r.Type == ClaimTypes.Role).Select(claim => claim.Value);
        }
    }
}