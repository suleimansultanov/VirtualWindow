using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Core;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.UI.Managers.PosScreenTemplates;
using NasladdinPlace.UI.Managers.PosScreenTemplates.Files.Contracts;
using NasladdinPlace.UI.Managers.PosScreenTemplates.Validators;
using NasladdinPlace.UI.Tests.DependencyInjection;
using NasladdinPlace.UI.ViewModels.PointsOfSale;
using NasladdinPlace.Utilities.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Tests.Scenarios.PosScreenTemplate
{
    public class PosScreenTemplateManagerIntegrationTests : TestsBase
    {
        private const string CustomPosScreenTemplate = "Custom";
        private const int IncorrectPosScreenTemplateId = 9999;
        private const int DefaultPosId = 1;
        private const string DefaultFileName = "index.html";
        private const string DefaultContentFile = "Fake text";

        private IServiceProvider _serviceProvider;
        private Mock<IPosScreenTemplateFilesManager> _mockTemplateFileManager;
        private IPosScreenTemplateManager _posScreenTemplateManager;
        private Core.Models.PosScreenTemplate _defaultPosScreenTemplate;

        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());

            Mapper.Reset();

            _serviceProvider = new ServiceProviderFactory().CreateServiceProvider(Context);

            _mockTemplateFileManager = new Mock<IPosScreenTemplateFilesManager>();

            var unitOfWorkFactory = _serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _defaultPosScreenTemplate = Context.PosScreenTemplates.First();
            var posScreenTemplateValidator = new PosScreenTemplateValidator(unitOfWorkFactory, _defaultPosScreenTemplate.Id);

            _posScreenTemplateManager = new PosScreenTemplateManager(unitOfWorkFactory, _mockTemplateFileManager.Object,
                posScreenTemplateValidator, _defaultPosScreenTemplate.Id);
        }

        [Test]
        public void DeletePosScreenTemplateAsync_DefaultTemplateIsGiven_ShouldReturnFailureResult()
        {
            var removePosScreenTemplateResult = _posScreenTemplateManager
                .DeletePosScreenTemplateAsync(_defaultPosScreenTemplate.Id).Result;

            removePosScreenTemplateResult.Should().NotBeNull();
            removePosScreenTemplateResult.Succeeded.Should().BeFalse();
        }

        [Test]
        public void DeletePosScreenTemplateAsync_PinnedToPosNotDefaultTemplateIsGiven_ShouldReturnFailureResult()
        {
            var posScreenTemplate = new Core.Models.PosScreenTemplate(CustomPosScreenTemplate);

            Seeder.Seed(new List<Core.Models.PosScreenTemplate>
            {
                posScreenTemplate
            });

            var pos = Context.PointsOfSale.First(p => p.Id == DefaultPosId);
            pos.UpdatePosScreenTemplate(posScreenTemplate.Id);
            Context.SaveChanges();

            var removePosScreenTemplateResult = _posScreenTemplateManager.DeletePosScreenTemplateAsync(posScreenTemplate.Id).Result;

            removePosScreenTemplateResult.Should().NotBeNull();
            removePosScreenTemplateResult.Succeeded.Should().BeFalse();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void
            DeletePosScreenTemplateAsync_NotPinnedToPosNotDefaultTemplateWithDifferentRemovalFileManagerResultsAreGiven_ShouldReturnExpectedResult(
                bool templateFolderDeletionResult)
        {
            var posScreenTemplate = new Core.Models.PosScreenTemplate(CustomPosScreenTemplate);

            Seeder.Seed(new List<Core.Models.PosScreenTemplate>
            {
                posScreenTemplate
            });

            _mockTemplateFileManager.Setup(m => m.DeleteTemplateDirectoryAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(templateFolderDeletionResult ? Result.Success() : Result.Failure()));

            var removePosScreenTemplateResult =
                _posScreenTemplateManager.DeletePosScreenTemplateAsync(posScreenTemplate.Id).Result;

            removePosScreenTemplateResult.Should().NotBeNull();
            removePosScreenTemplateResult.Succeeded.Should().Be(templateFolderDeletionResult);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void
            UploadTemplateFileAsync_PosScreenTemplateWithDifferentUploadFileManagerResultsAreGiven_ShouldReturnExpectedResult(
                bool templateUploadingFileResult)
        {
            var fileMock = new Mock<IFormFile>();

            fileMock.Setup(f => f.FileName).Returns(DefaultFileName);
            fileMock.Setup(f => f.Length).Returns(DefaultContentFile.Length);

            _mockTemplateFileManager.Setup(m => m.UploadTemplateFileAsync(It.IsAny<int>(), It.IsAny<IFormFile>()))
                .Returns(Task.FromResult(templateUploadingFileResult ? Result.Success() : Result.Failure()));

            var uploadPosScreenTemplateFileResult = _posScreenTemplateManager
                .UploadTemplateFileAsync(_defaultPosScreenTemplate.Id, fileMock.Object).Result;

            uploadPosScreenTemplateFileResult.Should().NotBeNull();
            uploadPosScreenTemplateFileResult.Succeeded.Should().Be(templateUploadingFileResult);
        }

        [Test]
        public void UploadTemplateFileAsync_UploadFileIsEmpty_ShouldReturnFailureResult()
        {
            var fileMock = new Mock<IFormFile>();

            var uploadPosScreenTemplateFileResult = _posScreenTemplateManager
                .UploadTemplateFileAsync(_defaultPosScreenTemplate.Id, fileMock.Object).Result;

            uploadPosScreenTemplateFileResult.Should().NotBeNull();
            uploadPosScreenTemplateFileResult.Succeeded.Should().BeFalse();
        }

        [Test]
        public void UploadTemplateFileAsync_PosScreenTemplateIsNotGiven_ShouldReturnFailureResult()
        {
            var fileMock = new Mock<IFormFile>();

            var uploadPosScreenTemplateFileResult = _posScreenTemplateManager
                .UploadTemplateFileAsync(IncorrectPosScreenTemplateId, fileMock.Object).Result;

            uploadPosScreenTemplateFileResult.Should().NotBeNull();
            uploadPosScreenTemplateFileResult.Succeeded.Should().BeFalse();
        }

        [Test]
        public void CreatePosScreenTemplateAsync_PosScreenTemplateNameAlreadyExistsIsGiven_ShouldReturnFailureResult()
        {
            var createPosScreenTemplateResult = _posScreenTemplateManager
                .CreatePosScreenTemplateAsync(_defaultPosScreenTemplate.Name).Result;

            createPosScreenTemplateResult.Should().NotBeNull();
            createPosScreenTemplateResult.Succeeded.Should().BeFalse();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void
            CreatePosScreenTemplateAsync_PosScreenTemplateNameNotExistWithDifferentCreateFolderResultsAreGiven_ShouldReturnExpectedResult(
                bool templateFolderCreationResult)
        {
            _mockTemplateFileManager.Setup(m => m.CreateTemplateDirectoryAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(templateFolderCreationResult ? Result.Success() : Result.Failure()));

            var createPosScreenTemplateResult = _posScreenTemplateManager
                .CreatePosScreenTemplateAsync(CustomPosScreenTemplate).Result;

            createPosScreenTemplateResult.Should().NotBeNull();
            createPosScreenTemplateResult.Succeeded.Should().Be(templateFolderCreationResult);
        }

        [Test]
        public void DeleteTemplateFileAsync_CorrectPosScreenTemplateIsGiven_ShouldReturnFailureResult()
        {
            var deletePosScreenTemplateFileResult = _posScreenTemplateManager
                .DeleteTemplateFileAsync(IncorrectPosScreenTemplateId, DefaultFileName).Result;

            deletePosScreenTemplateFileResult.Should().NotBeNull();
            deletePosScreenTemplateFileResult.Succeeded.Should().BeFalse();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteTemplateFileAsync_PosScreenTemplateIsGiven_ShouldReturnExpectedResult(bool templateDeletionFileResult)
        {
            _mockTemplateFileManager.Setup(m => m.DeleteTemplateFileAsync(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.FromResult(templateDeletionFileResult ? Result.Success() : Result.Failure()));

            var deletePosScreenTemplateFileResult = _posScreenTemplateManager
                .DeleteTemplateFileAsync(_defaultPosScreenTemplate.Id, DefaultFileName).Result;

            deletePosScreenTemplateFileResult.Should().NotBeNull();
            deletePosScreenTemplateFileResult.Succeeded.Should().Be(templateDeletionFileResult);
        }

        [Test]
        public void EditPosScreenTemplateAsync_PosScreenTemplateNotExistIsGiven_ShouldReturnFailureResult()
        {
            var editPosScreenTemplateResult = _posScreenTemplateManager
                .EditPosScreenTemplateAsync(IncorrectPosScreenTemplateId, _defaultPosScreenTemplate.Name,
                    new List<PosBasicInfoViewModel>()).Result;

            editPosScreenTemplateResult.Should().NotBeNull();
            editPosScreenTemplateResult.Succeeded.Should().BeFalse();
        }

        [Test]
        public void EditPosScreenTemplateAsync_PosScreenTemplateWithIdenticalNamesAreGiven_ShouldReturnFailureResult()
        {
            var posScreenTemplate = new Core.Models.PosScreenTemplate(CustomPosScreenTemplate);

            Seeder.Seed(new List<Core.Models.PosScreenTemplate>
            {
                posScreenTemplate
            });

            var editPosScreenTemplateResult = _posScreenTemplateManager
                .EditPosScreenTemplateAsync(_defaultPosScreenTemplate.Id, CustomPosScreenTemplate,
                    new List<PosBasicInfoViewModel>()).Result;

            editPosScreenTemplateResult.Should().NotBeNull();
            editPosScreenTemplateResult.Succeeded.Should().BeFalse();
        }

        [Test]
        public void EditPosScreenTemplateAsync_CorrectPosScreenTemplateIsGiven_ShouldReturnSuccessResult()
        {
            var posScreenTemplate = new Core.Models.PosScreenTemplate(CustomPosScreenTemplate);

            Seeder.Seed(new List<Core.Models.PosScreenTemplate>
            {
                posScreenTemplate
            });

            var pointsOfSale = Context.PointsOfSale.Where(p => p.PosScreenTemplateId == _defaultPosScreenTemplate.Id)
                .Select(p => new PosBasicInfoViewModel
                {
                    PosId = p.Id,
                    Name = p.Name
                }).ToList();

            _mockTemplateFileManager.Setup(m => m.GetMissingRequiredFileNamesForTemplate(It.IsAny<int>()))
                .Returns(new List<string>());

            var editPosScreenTemplateResult = _posScreenTemplateManager
                .EditPosScreenTemplateAsync(posScreenTemplate.Id, CustomPosScreenTemplate, pointsOfSale).Result;

            editPosScreenTemplateResult.Should().NotBeNull();
            editPosScreenTemplateResult.Succeeded.Should().BeTrue();
            Context.PointsOfSale.Where(p => p.PosScreenTemplateId == _defaultPosScreenTemplate.Id).AsNoTracking().Should()
                .HaveCount(0);
        }

        [Test]
        public void EditPosScreenTemplateAsync_PosScreenTemplateWithMissingFilesIsGiven_ShouldReturnFailureResult()
        {
            _mockTemplateFileManager.Setup(m => m.GetMissingRequiredFileNamesForTemplate(It.IsAny<int>()))
                .Returns(new List<string> {DefaultFileName});

            var editPosScreenTemplateResult = _posScreenTemplateManager
                .EditPosScreenTemplateAsync(_defaultPosScreenTemplate.Id, _defaultPosScreenTemplate.Name,
                    new List<PosBasicInfoViewModel>()).Result;

            editPosScreenTemplateResult.Should().NotBeNull();
            editPosScreenTemplateResult.Succeeded.Should().BeFalse();
        }
    }
}