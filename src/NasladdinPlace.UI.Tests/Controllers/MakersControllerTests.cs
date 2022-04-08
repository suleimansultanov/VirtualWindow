using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Dtos;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.UI.Controllers.Api;
using NasladdinPlace.UI.Tests.DependencyInjection;
using NasladdinPlace.UI.ViewModels.Makers;
using NUnit.Framework;
using System.Linq;
using System.Security.Claims;

namespace NasladdinPlace.UI.Tests.Controllers
{
    public class MakersControllerTests : TestsBase
    {
        private const string DefaultMakerName = "Тестовый производитель";
        private const string DefaultMakerNameToAddOrEdit = "Test maker";
        private const string DefaultUserId = "1";
        private const string MakerExistErrorMessage = "Производитель с именем Тестовый производитель уже существует";
        private const string MakerNotFoundErrorMessage = "Производитель с именем Тестовый производитель не найден";
        private const string MakerHaveAnyGoodsErrorMessage =
            "В системе есть товары от этого производителя, поэтому его невозможно удалить. Обратитесь к администратору.";
        private const string MakerNotFoundOnDeletionErrorMessage = "Производитель не существует.";
        private const string MakerNameCannotBeNullOnAddErrorMessage =
            "При попытке добавления производителя произошла ошибка, попробуйте позже.";
        private const string MakerNameCannotBeNullOnEditErrorMessage =
            "При попытке редактирования производителя произошла ошибка, попробуйте позже.";
        private const int DefaultMakerId = 1;
        private const int IncorrectMakerId = 10;

        private MakersController _makersController;
        private IUnitOfWork _unitOfWork;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new CurrenciesDataSet());
            Seeder.Seed(new MakersDataSet());
            Seeder.Seed(new GoodsDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());

            var serviceProvider = new ServiceProviderFactory().CreateServiceProvider(Context);

            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkFactory>().MakeUnitOfWork();
            _makersController = serviceProvider.GetRequiredService<MakersController>();

            _makersController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, DefaultUserId)
                            },
                            "sameAuthTypeName"
                        )
                    )
                }
            };
        }

        [Test]
        public void AddMakerAsync_CorrectViewModelIsGiven_ShouldReturnOkResult()
        {
            var viewModel = new MakerViewModel
            {
                Name = DefaultMakerNameToAddOrEdit
            };
            var result = _makersController.AddMakerAsync(viewModel).GetAwaiter().GetResult();
            result.Should().BeOfType<OkResult>();

            var maker = _unitOfWork.Makers.GetByNameAsync(DefaultMakerNameToAddOrEdit).GetAwaiter().GetResult();
            maker.Should().NotBeNull();
            maker.Name.Should().Be(DefaultMakerNameToAddOrEdit);
        }

        [Test]
        public void AddMakerAsync_IncorrectViewModelIsGiven_ShouldReturnBadRequestObjectResult()
        {
            var viewModel = new MakerViewModel
            {
                Name = DefaultMakerName
            };
            var result = _makersController.AddMakerAsync(viewModel).GetAwaiter().GetResult();
            result.Should().BeOfType<BadRequestObjectResult>();

            var objectResult = result as BadRequestObjectResult;

            objectResult.Should().NotBeNull();
            var errorResponse = objectResult?.Value as ErrorResponseDto;

            errorResponse.Should().NotBeNull();
            errorResponse.Error.Should().Be(MakerExistErrorMessage);
        }

        [Test]
        public void AddMakerAsync_EmptyNameInViewModelIsGiven_ShouldReturnBadRequestObjectResult()
        {
            var viewModel = new MakerViewModel
            {
                Name = string.Empty
            };
            var result = _makersController.AddMakerAsync(viewModel).GetAwaiter().GetResult();
            result.Should().BeOfType<BadRequestObjectResult>();

            var objectResult = result as BadRequestObjectResult;

            objectResult.Should().NotBeNull();
            var errorResponse = objectResult?.Value as ErrorResponseDto;

            errorResponse.Should().NotBeNull();
            errorResponse.Error.Should().Be(MakerNameCannotBeNullOnAddErrorMessage);
        }

        [Test]
        public void EditMakerAsync_CorrectViewModelIsGiven_ShouldReturnOkResult()
        {
            var viewModel = new MakerViewModel
            {
                Id = DefaultMakerId,
                Name = DefaultMakerNameToAddOrEdit
            };
            var result = _makersController.EditMakerAsync(viewModel).GetAwaiter().GetResult();
            result.Should().BeOfType<OkResult>();

            var maker = _unitOfWork.Makers.GetById(DefaultMakerId);
            maker.Should().NotBeNull();
            maker.Name.Should().Be(DefaultMakerNameToAddOrEdit);
            maker.Name.Should().NotBe(DefaultMakerName);
        }

        [Test]
        public void EditMakerAsync_IncorrectViewModelIsGiven_ShouldReturnNotFoundObjectResult()
        {
            var viewModel = new MakerViewModel
            {
                Id = IncorrectMakerId,
                Name = DefaultMakerName
            };
            var result = _makersController.EditMakerAsync(viewModel).GetAwaiter().GetResult();
            result.Should().BeOfType<NotFoundObjectResult>();

            var objectResult = result as NotFoundObjectResult;

            objectResult.Should().NotBeNull();
            var errorResponse = objectResult?.Value as ErrorResponseDto;

            errorResponse.Should().NotBeNull();
            errorResponse.Error.Should().Be(MakerNotFoundErrorMessage);
        }

        [Test]
        public void EditMakerAsync_EmptyNameInViewModelIsGiven_ShouldReturnBadRequestObjectResult()
        {
            var viewModel = new MakerViewModel
            {
                Id = DefaultMakerId,
                Name = string.Empty
            };
            var result = _makersController.EditMakerAsync(viewModel).GetAwaiter().GetResult();
            result.Should().BeOfType<BadRequestObjectResult>();

            var objectResult = result as BadRequestObjectResult;

            objectResult.Should().NotBeNull();
            var errorResponse = objectResult?.Value as ErrorResponseDto;

            errorResponse.Should().NotBeNull();
            errorResponse.Error.Should().Be(MakerNameCannotBeNullOnEditErrorMessage);
        }

        [Test]
        public void DeleteMakerAsync_CorrectMakerIdIsGivenAndMakerHaveNotAnyGoods_ShouldReturnOkResult()
        {
            var viewModel = new MakerViewModel
            {
                Name = DefaultMakerNameToAddOrEdit
            };

            _makersController.AddMakerAsync(viewModel).GetAwaiter().GetResult();

            var maker = Context.Makers.FirstOrDefault(m => m.Name == DefaultMakerNameToAddOrEdit);

            maker.Should().NotBeNull();

            var result = _makersController.DeleteMakerAsync(maker.Id).GetAwaiter().GetResult();
            result.Should().BeOfType<OkResult>();

            var deletedMaker = Context.Makers.FirstOrDefault(m => m.Name == DefaultMakerNameToAddOrEdit);
            deletedMaker.Should().BeNull();
        }

        [Test]
        public void DeleteMakerAsync_IncorrectMakerIdIsGiven_ShouldReturnBadRequestObjectResult()
        {
            var result = _makersController.DeleteMakerAsync(IncorrectMakerId).GetAwaiter().GetResult();
            result.Should().BeOfType<BadRequestObjectResult>();

            var objectResult = result as BadRequestObjectResult;

            objectResult.Should().NotBeNull();
            var errorResponse = objectResult?.Value as ErrorResponseDto;

            errorResponse.Should().NotBeNull();
            errorResponse.Error.Should().Be(MakerNotFoundOnDeletionErrorMessage);
        }

        [Test]
        public void DeleteMakerAsync_CorrectMakerIdIsGivenAndMakerHaveGood_ShouldReturnBadRequestObjectResult()
        {
            var result = _makersController.DeleteMakerAsync(DefaultMakerId).GetAwaiter().GetResult();
            result.Should().BeOfType<BadRequestObjectResult>();

            var objectResult = result as BadRequestObjectResult;

            objectResult.Should().NotBeNull();
            var errorResponse = objectResult?.Value as ErrorResponseDto;

            errorResponse.Should().NotBeNull();
            errorResponse.Error.Should().Be(MakerHaveAnyGoodsErrorMessage);
        }
    }
}
