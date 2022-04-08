using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Dtos.User;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Users.Bonus.Manager.Contracts;
using NasladdinPlace.Core.Services.Users.Search.Model;
using NasladdinPlace.DAL.Constants;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAbnormalUserBonusPointsSeekingManager _abnormalUserBonusPointsSeekingManager;

        public UsersController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork,
            IAbnormalUserBonusPointsSeekingManager abnormalUserBonusPointsSeekingManager)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _abnormalUserBonusPointsSeekingManager = abnormalUserBonusPointsSeekingManager;
        }

        [HttpPost("abnormalUserBonusesSearch")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult RunAbnormalUserBonusesSeekingAgent()
        {
            Task.Run(() => _abnormalUserBonusPointsSeekingManager.SeekAsync());

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync() =>
            Ok((await _userManager.Users.ToListAsync()).Select(Mapper.Map<ApplicationUser, UserFullInfoDto>));

        [HttpPost("search")]
        public async Task<IActionResult> GetUsersAsyncByFilter([FromBody] FilterDto filter)
        {
            var users = await _unitOfWork.Users.GetByFilterAsync(Transform(filter));

            return Ok(users.Select(Mapper.Map<ApplicationUser, UserFullInfoDto>));
        }

        [HttpPut("{userId:int}/password")]
        public async Task<IActionResult> ChangeUserPassword(int userId, [FromBody] UserPasswordDto userPasswordDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return NotFound();

            return await TryChangeUserPasswordAsync(user, userPasswordDto.Password)
                ? (IActionResult) Ok()
                : BadRequest();
        }
        
        private async Task<bool> TryChangeUserPasswordAsync(ApplicationUser user, string password)
        {
            var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordToken, password);
            return result.Succeeded;
        }
        
        public static Filter Transform(FilterDto dto)
        {
            return new Filter
            {
                Email = dto.Email,
                UserName = dto.UserName,
                PhoneNumber = dto.PhoneNumber,
                PhoneNumberConfirmed = dto.PhoneNumberConfirmed,
                Page = dto.Page,
                PageSize = dto.PageSize,
                Search = dto.Search,
                SortBy = dto.SortBy
            };
        }
    }
}
