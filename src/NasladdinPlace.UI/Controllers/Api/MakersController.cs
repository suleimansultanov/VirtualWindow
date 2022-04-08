using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Dtos;
using NasladdinPlace.Logging;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.ViewModels.Makers;
using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Helpers.ACL;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Route(Routes.Api)]
    [Authorize]
    [Permission(nameof(MakerCrudPermission))]
    public class MakersController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public MakersController(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
        }

        [HttpPut]
        public async Task<IActionResult> EditMakerAsync([FromBody] MakerViewModel viewModel)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var maker = await unitOfWork.Makers.GetAsync(viewModel.Id);

                if (maker == null)
                    return NotFound(new ErrorResponseDto { Error = $"Производитель с именем {viewModel.Name} не найден" });

                try
                {
                    maker.SetMakerName(viewModel.Name);

                    unitOfWork.Makers.Update(maker);

                    await unitOfWork.CompleteAsync();

                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{User.Identity.Name} tried to edit maker {maker.Name}. Verbose error: {ex}");
                    return BadRequest(new ErrorResponseDto
                    {
                        Error = "При попытке редактирования производителя произошла ошибка, попробуйте позже."
                    });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddMakerAsync([FromBody] MakerViewModel viewModel)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var maker = await unitOfWork.Makers.GetByNameAsync(viewModel.Name);

                    if (maker != null)
                        return BadRequest(
                            new ErrorResponseDto { Error = $"Производитель с именем {viewModel.Name} уже существует" });

                    unitOfWork.Makers.Add(
                        new Maker(viewModel.Name));

                    await unitOfWork.CompleteAsync();

                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{User.Identity.Name} tried to add maker. Verbose error: {ex}");
                    return BadRequest(new ErrorResponseDto
                    {
                        Error = "При попытке добавления производителя произошла ошибка, попробуйте позже."
                    });
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMakerAsync(int id)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var maker = await unitOfWork.Makers.GetAsync(id);

                if (maker == null)
                    return BadRequest(new ErrorResponseDto
                    {
                        Error = "Производитель не существует."
                    });

                try
                {
                    unitOfWork.Makers.Remove(maker);

                    await unitOfWork.CompleteAsync();

                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{User.Identity.Name} tried to delete maker {maker.Name}. Verbose error: {ex}");
                    return BadRequest(new ErrorResponseDto
                    {
                        Error = "В системе есть товары от этого производителя, поэтому его невозможно удалить. Обратитесь к администратору."
                    });
                }
            }
        }
    }
}