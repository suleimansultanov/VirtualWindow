using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Dtos.GoodCategory;
using NasladdinPlace.UI.Dtos.Maker;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.ViewModels.Goods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class GoodsController : ReferenceBaseController
    {
        private readonly IConfigurationReader _configurationReader;

        public GoodsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _configurationReader = serviceProvider.GetRequiredService<IConfigurationReader>();
        }

        [Permission(nameof(GoodCrudPermission))]
        public async Task<IActionResult> AddGoodAsync()
        {
            return await AddSelectListsAndReturnGoodFormOrRedirectToGoodListAsync(new GoodsFormViewModel());
        }

        [Permission(nameof(GoodCrudPermission))]
        public async Task<IActionResult> EditGoodAsync(int id)
        {
            var good = await UnitOfWork.Goods.GetIncludingImagesAsync(id);

            if (good == null)
                return this.RedirectToHome();

            var viewModel = Mapper.Map<GoodsFormViewModel>(good);

            return await AddSelectListsAndReturnGoodFormOrRedirectToGoodListAsync(viewModel);
        }

        public IActionResult GetGoods()
        {
            return Reference<GoodViewModel>(
                configuration: new ConfigReference
                {
                    Title = "Товары",
                    FilterType = RenderFilter.WithAdditionFilters,
                    BeforeGridPartialName = "~/Views/Goods/_beforeGridPartial.cshtml",
                    Actions = new List<Command>
                    {
                        new Command
                        {
                            Type = CommandType.Custom,
                            Name = "<i class='fa fa-plus'></i> Добавить",
                            Binding = "click: add"
                        }
                    }
                }
            );
        }

        private async Task<bool> AddGoodCategoryAndMakerSelectListAsync(GoodsFormViewModel viewModel)
        {
            var makersResult = (await UnitOfWork.Makers.GetAllAsync()).Select(Mapper.Map<MakerDto>);
            var goodCategoriesResult =
                (await UnitOfWork.GoodCategories.GetAllAsync()).Select(Mapper.Map<GoodCategoryDto>);

            if (!goodCategoriesResult.Any() || !makersResult.Any())
                return false;

            viewModel.GoodCategorySelectList = goodCategoriesResult.ToSelectList();
            viewModel.MakerSelectList = makersResult.ToSelectList();
            viewModel.PublishingStatusSelectList = GetPublishingStatuSelectList(viewModel.PublishingStatusSelectList);

            return true;
        }

        private async Task<IActionResult> AddSelectListsAndReturnGoodFormOrRedirectToGoodListAsync(
            GoodsFormViewModel viewModel)
        {
            GetDefaultImagePath(viewModel);

            return !await AddGoodCategoryAndMakerSelectListAsync(viewModel)
                ? (ActionResult)this.RedirectToHome()
                : View("GoodForm", viewModel);
        }

        private SelectList GetPublishingStatuSelectList(object selectedValue)
        {
            return new SelectList(
                Enum.GetValues(typeof(GoodPublishingStatus))
                    .Cast<GoodPublishingStatus>()
                    .Select(x => new SelectListItem
                    {
                        Text = x.GetDescription(),
                        Value = Convert.ToByte(x).ToString(),
                    }), "Value", "Text", selectedValue);
        }

        private void GetDefaultImagePath(GoodsFormViewModel viewModel)
        {
            var baseApiUrl = _configurationReader.GetBaseApiUrl();
            var defaultImagePath = _configurationReader.GetDefaultImagePath();

            viewModel.DefaultImagePath = ConfigurationReaderExt.CombineUrlParts(baseApiUrl, defaultImagePath);
        }
    }
}