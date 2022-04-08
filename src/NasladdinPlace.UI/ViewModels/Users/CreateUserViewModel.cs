using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.ViewModels.Users
{
    public class CreateUserViewModel
    {
        public int Id { get; set; }

        public string Email { get; set; }

        [Render(Control = RenderControl.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Render(Control = RenderControl.Password)]
        public string PasswordConfirm { get; set; }

        public List<int> SelectedRoles { get; set; }


        public bool IsActive { get; set; }
    }
}
