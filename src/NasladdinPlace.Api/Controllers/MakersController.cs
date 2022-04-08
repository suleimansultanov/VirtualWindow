using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Dtos.Maker;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.DAL.Constants;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
    public class MakersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MakersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> AddMakerAsync([FromBody] MakerDto dto)
        {
            var maker = new Maker(0, dto.Name);
            
            _unitOfWork.Makers.Add(maker);
            await _unitOfWork.CompleteAsync();

            return StatusCode(StatusCodes.Status201Created, maker.Id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMakerAsync(int id)
        {
            var maker = await _unitOfWork.Makers.GetAsync(id);
            return maker == null
                ? (IActionResult) NotFound()
                : Ok(Mapper.Map<MakerDto>(maker));
        }

        [HttpGet]
        public async Task<IActionResult> GetMakersAsync()
            => Ok((await _unitOfWork.Makers.GetAllAsync()).Select(Mapper.Map<MakerDto>));

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMakerAsync(int id)
        {
            var maker = await _unitOfWork.Makers.GetAsync(id);
            if (maker == null)
                return NotFound();

            _unitOfWork.Makers.Remove(maker);
            _unitOfWork.CompleteAsync().Wait();

            return Ok(id);
        }
    }
}
