using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Logging;
using NasladdinPlace.UI.Services.Goods.Contracts;
using NasladdinPlace.UI.Services.NasladdinApi;
using NasladdinPlace.UI.ViewModels.Goods;
using NasladdinPlace.Utilities.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.UI.Services.Goods
{
    public class GoodService : IGoodService
    {
        private const int MaxFileNameLength = 500;
        private static readonly string[] AllowedContentTypes = { "image/jpg", "image/jpeg", "image/png", "image/webp" };
        private static readonly string[] AllowedFileExtension = { ".jpg", ".jpeg", ".png", ".webp" };

        private readonly int _imageSizeLimitInKbytes;

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger _logger;
        private readonly INasladdinApiClient _nasladdinApiClient;
        private readonly IConfigurationReader _configurationReader;

        public GoodService(IServiceProvider serviceProvider)
        {
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _nasladdinApiClient = serviceProvider.GetRequiredService<INasladdinApiClient>();
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _configurationReader = serviceProvider.GetRequiredService<IConfigurationReader>();
            _imageSizeLimitInKbytes = _configurationReader.GetImageSizeLimit();
        }

        public async Task<ValueResult<string>> AddGoodAsync(GoodsFormViewModel viewModel, int userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var isFileValidResult = IsFileValid(viewModel);

                    if (!isFileValidResult.Succeeded)
                        return ValueResult<string>.Failure(isFileValidResult.Error);

                    var maker = await unitOfWork.Makers.GetAsync(viewModel.MakerId ?? Maker.Default.Id);
                    if (maker == null)
                        return ValueResult<string>.Failure($"Производитель с идентификатором {viewModel.MakerId} не существует");

                    var goodCategory =
                        await unitOfWork.GoodCategories.GetAsync(viewModel.GoodCategoryId ?? GoodCategory.Default.Id);

                    if (goodCategory == null)
                        return ValueResult<string>.Failure($"Категории с идентификатором {viewModel.GoodCategoryId} не существует");

                    var good = CreateGoodWithNutrients(viewModel, maker.Id, goodCategory.Id);

                    SetCompositionOfGood(good, viewModel.Composition);

                    if (viewModel.PublishingStatus.HasValue)
                        good.SetGoodStatus((GoodPublishingStatus)viewModel.PublishingStatus);

                    var addImageResult = await TrySaveFileAsync(viewModel.Image, good, userId, unitOfWork);

                    if (!addImageResult.Succeeded)
                        return ValueResult<string>.Failure(addImageResult.Error);

                    unitOfWork.Goods.Add(good);

                    await unitOfWork.CompleteAsync();

                    LogInfo(
                        $"User {userId} added good with name {good.Name} and added image {good.GetGoodImagePath()}");

                    return ValueResult<string>.Success(good.Id.ToString());
                }
                catch (Exception ex)
                {
                    LogError($"An error has occurred during adding a good. Verbose error: {ex}");
                    return ValueResult<string>.Failure("Произошла ошибка при добавлении товара. Пожалуйста, обратитесь к администратору.");
                }
            }
        }

        public async Task<Result> DeleteGoodAsync(int id, int userId)
        {
            var deleteGoodResult = await _nasladdinApiClient.DeleteGoodAsync(id);

            if (deleteGoodResult.IsRequestSuccessful)
            {
                LogInfo($"User {userId} has removed a good {id}");
                return Result.Success();
            }

            LogError($"Some error has been occured during removing a good. Verbose error: {deleteGoodResult.Exception}");

            return Result.Failure("Невозможно удалить товар, пожалуйста обратитесь к администратору.");
        }

        public async Task<Result> EditGoodAsync(GoodsFormViewModel viewModel, int userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var isFileValidResult = IsFileValid(viewModel);

                    if (!isFileValidResult.Succeeded)
                        return Result.Failure(isFileValidResult.Error);

                    var good = await unitOfWork.Goods.GetIncludingImagesAsync(viewModel.Id);

                    if (good == null)
                        return Result.Failure("Информация о товаре не найдена");

                    var maker = await unitOfWork.Makers.GetAsync(viewModel.MakerId ?? Maker.Default.Id);
                    if (maker == null)
                        return Result.Failure($"Производитель с идентификатором {viewModel.MakerId} не существует");

                    var goodCategory =
                        await unitOfWork.GoodCategories.GetAsync(viewModel.GoodCategoryId ?? GoodCategory.Default.Id);
                    if (goodCategory == null)
                        return Result.Failure($"Категории с идентификатором {viewModel.GoodCategoryId} не существует");

                    good.Name = viewModel.Name;
                    good.Description = viewModel.Description;
                    good.UpdateGoodParameters(new GoodParameters(maker.Id, goodCategory.Id)
                    {
                        Volume = viewModel.Volume,
                        NetWeight = viewModel.NetWeight
                    });

                    AddNutrientsToGood(viewModel, good);

                    SetCompositionOfGood(good, viewModel.Composition);
                    
                    if(viewModel.PublishingStatus.HasValue)
                        good.SetGoodStatus((GoodPublishingStatus)viewModel.PublishingStatus);

                    var addImageResult = await TrySaveFileAsync(viewModel.Image, good, userId, unitOfWork);

                    if (!addImageResult.Succeeded)
                        return Result.Failure(addImageResult.Error);

                    unitOfWork.Goods.Update(good);
                    await unitOfWork.CompleteAsync();

                    return Result.Success();
                }
                catch (Exception ex)
                {
                    LogError($"An error has occurred during editing a good. Verbose error: {ex}");
                    return Result.Failure("Произошла ошибка при редактировании товара. Пожалуйста, обратитесь к администратору.");
                }
            }
        }

        public ValueResult<string> GetBaseApiUrl()
        {
            try
            {
                var baseApiUrl = _configurationReader.GetBaseApiUrl();
                return ValueResult<string>.Success(baseApiUrl);
            }
            catch (Exception ex)
            {
                LogError($"An error has occurred during getting a base api url. Verbose error: {ex}");
                return ValueResult<string>.Failure("Произошла ошибка при получении базового URL-адреса API. Пожалуйста, обратитесь к администратору.");
            }
        }

        private void SetCompositionOfGood(Good good, string composition)
        {
            if (!string.IsNullOrEmpty(composition))
                good.SetComposition(composition);
        }

        private void LogError(string errorMessage)
        {
            _logger.LogError(errorMessage);
        }

        private void LogInfo(string infoMessage)
        {
            _logger.LogInfo(infoMessage);
        }

        private Result IsFileValid(GoodsFormViewModel viewModel)
        {
            if (viewModel.Image == null)
                return Result.Success();

            if (!IsFileExtensionValid(viewModel.Image))
                return Result.Failure("Загружаемый файл должен иметь нужное расширение .jpg, .jpeg, .webp или .png");

            var imageSizeLimitInBytes = _imageSizeLimitInKbytes * 1024;

            if (viewModel.Image.Length > imageSizeLimitInBytes)
                return Result.Failure("Загружаемый файл превышает размер в 500 КБайт.");

            if (viewModel.Image.FileName.Length > MaxFileNameLength)
                return Result.Failure("Имя загружаемого файла превышает 500 символов, переименуйте файл.");

            return Result.Success();
        }

        private bool IsFileExtensionValid(IFormFile file)
        {
            return AllowedContentTypes.Contains(file.ContentType.ToLower()) &&
                   AllowedFileExtension.Contains(Path.GetExtension(file.FileName).ToLower());
        }

        private async Task<Result> TrySaveFileAsync(IFormFile image, Good good, int userId, IUnitOfWork unitOfWork)
        {
            if (image == null)
                return Result.Success();

            var addImageResult = await _nasladdinApiClient.AddGoodImageAsync(good.Id, image, userId);

            if (!addImageResult.IsRequestSuccessful)
                return Result.Failure(addImageResult.Error);

            LogInfo(
                $"User {userId} edited good with name {good.Name} and added image {addImageResult.Result}, previous image path was {good.GetGoodImagePath()}");

            var goodImage = await unitOfWork.GoodImages.GetByGoodIdAsync(good.Id);

            if (goodImage == null)
            {
                goodImage = new GoodImage(good.Id, addImageResult.Result);
                unitOfWork.GoodImages.Add(goodImage);
            }
            else
            {
                goodImage.SetImagePath(addImageResult.Result);
                unitOfWork.GoodImages.Update(goodImage);
            }

            if (good.GoodImages.Any())
                good.GoodImages.Clear();

            good.GoodImages.Add(goodImage);

            return Result.Success();
        }

        private Good CreateGoodWithNutrients(GoodsFormViewModel viewModel, int makerId, int goodCategoryId)
        {
            var good = new Good(
                viewModel.Name,
                viewModel.Description,
                new GoodParameters(makerId, goodCategoryId)
                {
                    Volume = viewModel.Volume,
                    NetWeight = viewModel.NetWeight
                });

            AddNutrientsToGood(viewModel, good);

            return good;
        }

        private void AddNutrientsToGood(GoodsFormViewModel viewModel, Good good)
        {
            var proteinsFatsCarbohydratesCalories = new ProteinsFatsCarbohydratesCalories(
                viewModel.ProteinsInGrams.Value,
                viewModel.FatsInGrams.Value,
                viewModel.CarbohydratesInGrams.Value,
                viewModel.CaloriesInKcal.Value
            );
            good.ProteinsFatsCarbohydratesCalories = proteinsFatsCarbohydratesCalories;
        }
    }
}
