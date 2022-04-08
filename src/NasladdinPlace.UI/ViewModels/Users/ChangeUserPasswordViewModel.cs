using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.ViewModels.Users
{
    public class ChangeUserPasswordViewModel
    {
        public int Id { get; set; }

        public string Email { get; set; }

        [Render(Control = RenderControl.Password)]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [Render(Control = RenderControl.Password)]
        public string NewPasswordConfirmation { get; set; }
    }
}
