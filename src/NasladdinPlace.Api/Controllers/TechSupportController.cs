using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using NasladdinPlace.Api.Dtos.MessengerContact;
using NasladdinPlace.Core;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [AllowAnonymous]
    public class TechSupportController : Controller
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public TechSupportController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        [HttpGet("contacts")]
        public IActionResult GetMessengerContacts()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var messengerContacts = unitOfWork.MessengerContacts.GetAll().ToList();
                var messengerContactDtos = Mapper.Map<List<MessengerContactDto>>(messengerContacts);

                return Ok(messengerContactDtos);
            }
        }
    }
}