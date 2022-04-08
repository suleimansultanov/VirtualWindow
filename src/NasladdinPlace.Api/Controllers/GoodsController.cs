using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Dtos.Good;
using NasladdinPlace.Api.Services.FileStorage;
using NasladdinPlace.Core;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.Logging;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
    public class GoodsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorage _fileStorage;
        private readonly ILogger _logger;
        public GoodsController(
            IUnitOfWork unitOfWork,
            IFileStorage fileStorage,
            ILogger logger)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));
            if (fileStorage == null)
                throw new ArgumentNullException(nameof(fileStorage));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGoodAsync(int id)
        {
            var good = await _unitOfWork.Goods.GetAsync(id);
            return good == null
                ? (IActionResult)NotFound()
                : Ok(Mapper.Map<GoodDto>(good));
        }

        [HttpGet]
        public async Task<IActionResult> GetGoodsAsync()
            => Ok((await _unitOfWork.Goods.GetAllAsync()).Select(Mapper.Map<GoodDto>));

        [HttpGet("published")]
        public async Task<IActionResult> GetPublishedGoodsAsync()
            => Ok((await _unitOfWork.Goods.GetAllPublishedAsync()).Select(Mapper.Map<GoodDto>));

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoodAsync(int id)
        {
            var good = await _unitOfWork.Goods.GetIncludingImagesAsync(id);
            if (good == null)
                return NotFound();

            try
            {
                var goodImagePath = good.GetGoodImagePath();
                if(!string.IsNullOrEmpty(goodImagePath))
                    await _fileStorage.DeleteFile(goodImagePath);

                _unitOfWork.Goods.Remove(good);

                await _unitOfWork.CompleteAsync();

                return Ok(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Some error has been occured during removing a good. Verbose error: {ex}");
                return BadRequest("Some error has been occured during removing a good.");
            }
        }
    }
}
