using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Dtos.PosImage;
using NasladdinPlace.Api.Services.FileStorage;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.DAL.Constants;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Controllers
{
    [Route("api/pointsOfSale/{posId}/images")]
    [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
    public class PosImagesController : Controller
    {
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _unitOfWork;

        public PosImagesController(
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork)
        {
            _fileStorage = fileStorage;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll(int posId) =>
            Ok(_unitOfWork.PosImages.GetByPos(posId).Select(Mapper.Map<PosImageDto>));

        [HttpPost]
        public async Task<IActionResult> AddAsync(int posId, IFormFile imageFile)
        {
            var pos = await _unitOfWork.PointsOfSale.GetByIdAsync(posId);

            if (pos == null)
                return NotFound();

            var result = await _fileStorage.SaveFile(imageFile, "images/plant");

            if (!result.Succeeded)
                BadRequest("Some error has occured during saving an image.");

            var posImage = new PosImage(posId, result.FileRelativePath);

            _unitOfWork.PosImages.Add(posImage);
            await _unitOfWork.CompleteAsync();

            return StatusCode(StatusCodes.Status201Created, posImage.Id);
        }

        [HttpDelete("{posImageId}")]
        public async Task<IActionResult> DeleteAsync(int posId, int posImageId)
        {
            var posImage = _unitOfWork.PosImages.GetById(posImageId);

            await _fileStorage.DeleteFile(posImage.ImagePath);
            
            _unitOfWork.PosImages.Remove(posImageId);
            await _unitOfWork.CompleteAsync();

            return Ok(posImageId);
        }
    }
}