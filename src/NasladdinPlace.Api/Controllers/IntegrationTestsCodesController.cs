using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Dtos.Sms;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.IntegrationTestsSmsReader.Common;
using NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodesSeeking;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    public class IntegrationTestsCodesController : Controller
    {
        private readonly IVerificationCodeByTypeSeeker _verificationCodeByTypeSeeker;

        public IntegrationTestsCodesController(IVerificationCodeByTypeSeeker verificationCodeByTypeSeeker)
        {
            _verificationCodeByTypeSeeker = verificationCodeByTypeSeeker;
        }

        [HttpGet("{verificationCodeType}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> FindFirstVerificationCodeOfTypeAsync(int verificationCodeType)
        {
            if (!Enum.IsDefined(typeof(VerificationCodeType), verificationCodeType))
            {
                return BadRequest("Verification code type is incorrect.");
            }

            var verificationCodeResult = await _verificationCodeByTypeSeeker.FindFirstAsync(
                (VerificationCodeType) verificationCodeType
            );

            return verificationCodeResult.Succeeded
                ? (IActionResult) Ok(new CodeDto { Value = verificationCodeResult.Value.Value })
                : NotFound();
        }
    }
}