using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Logging;
using NasladdinPlace.UI.Services.GoodCategories.Contracts;
using NasladdinPlace.UI.Services.NasladdinApi;
using NasladdinPlace.UI.ViewModels.GoodCategories;
using NasladdinPlace.Utilities.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Services.GoodCategories
{
    public class GoodCategoryService : IGoodCategoryService
    {
        private const int MaxFileNameLength = 500;
        private static readonly string[] AllowedContentTypes = { "image/jpg", "image/jpeg", "image/png", "image/webp" };
        private static readonly string[] AllowedFileExtension = { ".jpg", ".jpeg", ".png", ".webp" };

        private readonly int _imageSizeLimitInKbytes;

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger _logger;
        private readonly INasladdinApiClient _nasladdinApiClient;
        private readonly IConfigurationReader _configurationReader;

        public GoodCategoryService(IServiceProvider serviceProvider)
        {
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _nasladdinApiClient = serviceProvider.GetRequiredService<INasladdinApiClient>();
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _configurationReader = serviceProvider.GetRequiredService<IConfigurationReader>();
            _imageSizeLimitInKbytes = _configurationReader.GetImageSizeLimit();
        }

        public async Task<Result> AddGoodCategoryAsync(GoodCategoryViewModel viewModel, int userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var isFileValidResult = IsFileValid(viewModel);

                    if (!isFileValidResult.Succeeded)
                        return Result.Failure(isFileValidResult.Error);

                    var goodCategories = await unitOfWork.GoodCategories.GetByNameAsync(viewModel.Name);

                    if (goodCategories.Any())
                        return Result.Failure("Невозможно добавить категорию товаров, которая уже существует");

                    var goodCategory = new GoodCategory(viewModel.Name);

                    var addImageResult = await TrySaveFileAsync(viewModel.Image, goodCategory, userId, int.MinValue);

                    if (!addImageResult.Succeeded)
                        return Result.Failure(addImageResult.Error);

                    unitOfWork.GoodCategories.Add(goodCategory);

                    await unitOfWork.CompleteAsync();

                    return Result.Success();
                }
                catch (Exception ex)
                {
                    LogError($"An error has occurred during adding a category. Verbose error: {ex}");
                    return Result.Failure("Произошла ошибка при добавлении категории. Пожалуйста, обратитесь к администратору.");
                }
            }
        }

        public async Task<Result> EditGoodCategoryAsync(GoodCategoryViewModel viewModel, int userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var isFileValidResult = IsFileValid(viewModel);

                    if (!isFileValidResult.Succeeded)
                        return Result.Failure(isFileValidResult.Error);

                    var goodCategory = await unitOfWork.GoodCategories.GetAsync(viewModel.Id);

                    if (goodCategory == null)
                        return Result.Failure("Категория товаров не найдена");

                    var addImageResult = await TrySaveFileAsync(viewModel.Image, goodCategory, userId, goodCategory.Id);

                    if (!addImageResult.Succeeded)
                        return Result.Failure(addImageResult.Error);

                    goodCategory.Name = viewModel.Name;

                    unitOfWork.GoodCategories.Update(goodCategory);

                    await unitOfWork.CompleteAsync();

                    return Result.Success();
                }
                catch (Exception ex)
                {
                    LogError($"An error has occurred during editing a category. Verbose error: {ex}");
                    return Result.Failure("Произошла ошибка при редактировании категории. Пожалуйста, обратитесь к администратору.");
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

        public ValueResult<string> GetDefaultImagePath()
        {
            try
            {
                var defaultImagePath = _configurationReader.GetDefaultImagePath();
                return ValueResult<string>.Success(defaultImagePath);
            }
            catch (Exception ex)
            {
                LogError($"An error has occurred during getting a default image path. Verbose error: {ex}");
                return ValueResult<string>.Failure("Произошла ошибка при получении изображения по умолчанию. Пожалуйста, обратитесь к администратору.");
            }
        }

        private void LogError(string errorMessage)
        {
            _logger.LogError(errorMessage);
        }

        private void LogInfo(string infoMessage)
        {
            _logger.LogInfo(infoMessage);
        }

        private Result IsFileValid(GoodCategoryViewModel viewModel)
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

        private async Task<Result> TrySaveFileAsync(IFormFile image, GoodCategory goodCategory, int userId, int goodCategoryId)
        {
            if (image == null)
                return Result.Success();

            var addImageResult = await _nasladdinApiClient.AddCategoryImageAsync(goodCategoryId, image, userId);

            if (!addImageResult.IsRequestSuccessful)
                return Result.Failure(addImageResult.Error);

            LogInfo(
                $"User {userId} modified category with name {goodCategory.Name} and added image {addImageResult.Result}, previous image path was {goodCategory.ImagePath}");

            goodCategory.SetImagePath(addImageResult.Result);

            return Result.Success();
        }
    }
}
