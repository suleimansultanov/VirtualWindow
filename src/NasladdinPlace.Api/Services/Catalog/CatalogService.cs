using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Dtos.GoodCategory;
using NasladdinPlace.Api.Dtos.LabeledGood;
using NasladdinPlace.Api.Dtos.Pos;
using NasladdinPlace.Api.Services.Catalog.Contracts;
using NasladdinPlace.Api.Services.Catalog.Models;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Utilities.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Api.Dtos.Good;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Dtos.Catalog;

namespace NasladdinPlace.Api.Services.Catalog
{
    public class CatalogService : ICatalogService
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfigurationReader _configurationReader;
        private readonly string _baseApiUrl;
        private readonly string _defaultImagePath;

        public CatalogService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _configurationReader = serviceProvider.GetRequiredService<IConfigurationReader>();
            _baseApiUrl = _configurationReader.GetBaseApiUrl();
            _defaultImagePath = _configurationReader.GetDefaultImagePath();
        }

        public async Task<ValueResult<List<LabeledGoodWithImageDto>>> GetCategoryItemsAsync(CategoryItemsDto categoryItemsDto)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var pageSize = _configurationReader.GetCatalogPageSize();

                    var labeledGoods = await unitOfWork.LabeledGoods.GetEnabledIncludingGoodInCategoryAsync(
                        categoryItemsDto.PosId, categoryItemsDto.CategoryId, categoryItemsDto.Page, pageSize);

                    var labeledGoodsWithImage = CompleteLabeledGoodsMapping(labeledGoods);

                    return ValueResult<List<LabeledGoodWithImageDto>>.Success(labeledGoodsWithImage);
                }
                catch (Exception ex)
                {
                    LogError($"Some error has been occured during getting category items. Verbose error: {ex}");
                    return ValueResult<List<LabeledGoodWithImageDto>>.Failure(
                        "Some error has been occured during getting category items.");
                }
            }
        }

        public async Task<ValueResult<CatalogPointOfSale>> GetPointsOfSaleAsync(byte page, int userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var pageSize = _configurationReader.GetCatalogPageSize();

                    var lastVisitedPointOfSale = await unitOfWork.PointsOfSale.GetLastVisitedPosByUserIdAsync(userId);

                    var activePointsOfSale =
                        await unitOfWork.PointsOfSale.GetActivePosesAsync(page, pageSize);

                    if (!activePointsOfSale.Any())
                        return ValueResult<CatalogPointOfSale>.Success(new CatalogPointOfSale
                        {
                            LastVisited = Mapper.Map<PointOfSaleDto>(lastVisitedPointOfSale)
                        });

                    var catalogPointsOfSale = new CatalogPointOfSale
                    {
                        LastVisited = Mapper.Map<PointOfSaleDto>(lastVisitedPointOfSale),
                        Items = Mapper.Map<ICollection<PointOfSaleDto>>(activePointsOfSale
                            .Where(p => p.PosActivityStatus == PosActivityStatus.Active)
                            .OrderBy(p => p.Id))
                    };

                    var activePosIds = activePointsOfSale.Select(p => p.Id).ToList();

                    var posRealTimeInfos =
                        unitOfWork.PosRealTimeInfos.GetByIds(activePosIds);

                    SetTemperatureFromPosRealTimeInfoToPos(catalogPointsOfSale.Items, posRealTimeInfos);

                    return ValueResult<CatalogPointOfSale>.Success(catalogPointsOfSale);
                }
                catch (Exception ex)
                {
                    LogError($"Some error has been occured during getting points of sale. Verbose error: {ex}");
                    return ValueResult<CatalogPointOfSale>.Failure(
                        "Some error has been occured during getting  points of sale.");
                }
            }
        }

        public async Task<ValueResult<List<PosContent>>> GetPosContentAsync(int posId, byte pageNumber)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var pageSize = _configurationReader.GetCatalogPageSize();
                    var categoriesSize = _configurationReader.GetCategoriesPageSizeInCatalog();

                    var categoriesDictWithLabeledGoods =
                        await unitOfWork.GoodCategories.GetGoodCategoriesDictionaryWithLabeledGoodsInPosAsync(posId,
                            pageNumber, pageSize, categoriesSize);

                    var posContents = categoriesDictWithLabeledGoods
                        .Select(categoryWithLabeledGoods => new PosContent()
                        {
                            Category = Mapper.Map<GoodCategoryDto>(categoryWithLabeledGoods.Key),
                            Goods = CompleteLabeledGoodsMapping(categoryWithLabeledGoods.Value)
                        }).ToList();

                    return ValueResult<List<PosContent>>.Success(posContents);
                }
                catch (Exception ex)
                {
                    LogError($"Some error has been occured during getting pos content. Verbose error: {ex}");
                    return ValueResult<List<PosContent>>.Failure(
                        "Some error has been occured during getting  pos content.");
                }
            }
        }

        public async Task<ValueResult<List<VirtualPosContent>>> GetVirtualPosContentAsync(byte pageNumber)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var pageSize = _configurationReader.GetVirtualCatalogPageSize();
                    var categoriesSize = _configurationReader.GetVirtualCategoriesPageSizeInCatalog();

                    var categoriesDictWithGoods =
                        await unitOfWork.GoodCategories.GetGoodCategoriesDictionaryWithLabeledGoodsAsync(
                            pageNumber, pageSize, categoriesSize);

                    var virtualPosContent = categoriesDictWithGoods
                        .Select(categoryWithGoods => new VirtualPosContent()
                        {
                            Category = Mapper.Map<GoodCategoryDto>(categoryWithGoods.Key),
                            Goods = CompleteGoodsMapping(categoryWithGoods.Value)
                        }).ToList();

                    return ValueResult<List<VirtualPosContent>>.Success(virtualPosContent);
                }
                catch (Exception ex)
                {
                    LogError($"Some error has been occured during getting pos content for virtual pos. Verbose error: {ex}");
                    return ValueResult<List<VirtualPosContent>>.Failure(
                        "Some error has been occured during getting  pos content for virtual pos.");
                }
            }
        }

        public async Task<ValueResult<List<GoodWithImageAndNutrients>>> GetVirtualCategoryItemsAsync(CategoryItemsDto categoryItemsDto)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var pageSize = _configurationReader.GetVirtualCatalogPageSize();

                    var goods = 
                        await unitOfWork.Goods.GetGoodsInCategoryAndInPreparingToPublishOrPublishedAsync(
                        categoryItemsDto.CategoryId, categoryItemsDto.Page, pageSize);

                    var goodsWithImage = CompleteGoodsMapping(goods);

                    return ValueResult<List<GoodWithImageAndNutrients>>.Success(goodsWithImage);
                }
                catch (Exception ex)
                {
                    LogError($"Some error has been occured during getting category items. Verbose error: {ex}");
                    return ValueResult<List<GoodWithImageAndNutrients>>.Failure(
                        "Some error has been occured during getting category items.");
                }
            }
        }

        private void SetTemperatureFromPosRealTimeInfoToPos(ICollection<PointOfSaleDto> pointsOfSale,
            List<PosRealTimeInfo> posRealTimeInfos)
        {
            foreach (var pointOfSaleDto in pointsOfSale)
            {
                var posRealTimeInfo = posRealTimeInfos.FirstOrDefault(p => p.Id == pointOfSaleDto.Id);
                pointOfSaleDto.Temperature = posRealTimeInfo?.TemperatureInsidePos ?? 0d;
            }
        }

        private List<GoodWithImageAndNutrients> CompleteGoodsMapping(List<Good> goods)
        {
            return Mapper.Map<List<Good>, List<GoodWithImageAndNutrients>>(goods,
                opt =>
                    opt.AfterMap((src, dest) =>
                    {
                        dest.ForEach(good =>
                            good.ImagePath =
                                string.IsNullOrEmpty(good.ImagePath)
                                    ? ConfigurationReaderExt.CombineUrlParts(_baseApiUrl, _defaultImagePath)
                                    : ConfigurationReaderExt.CombineUrlParts(_baseApiUrl, good.ImagePath));
                    }));
        }

        private List<LabeledGoodWithImageDto> CompleteLabeledGoodsMapping(List<LabeledGood> labeledGoods)
        {
            return Mapper.Map<List<LabeledGood>, List<LabeledGoodWithImageDto>>(labeledGoods,
                opt =>
                    opt.AfterMap((src, dest) =>
                    {
                        dest.ForEach(labeledGood =>
                            labeledGood.ImagePath =
                                string.IsNullOrEmpty(labeledGood.ImagePath)
                                    ? ConfigurationReaderExt.CombineUrlParts(_baseApiUrl, _defaultImagePath)
                                    : ConfigurationReaderExt.CombineUrlParts(_baseApiUrl, labeledGood.ImagePath));
                    }));
        }

        private void LogError(string errorMessage)
        {
            _logger.Error(errorMessage);
        }

        private string GetErrorMessage(string element, string methodName)
        {
            return $"Some error has been occured during getting {element} in {methodName} method";
        }
    }
}
