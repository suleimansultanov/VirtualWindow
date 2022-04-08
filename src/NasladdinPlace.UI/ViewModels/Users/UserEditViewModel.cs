using NasladdinPlace.Core.Enums;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using System;
using System.Collections.Generic;

namespace NasladdinPlace.UI.ViewModels.Users
{
    public class UserEditViewModel
    {
        public int Id { get; set; }

        public string Birthday { get; set; }

        [Render(Control = RenderControl.DateTime, DisplayType = DisplayType.Hide)]
        public DateTime? Birthdate { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string UserName { get; set; }

        public Gender Gender { get; set; }

        public List<int> SelectedRoles { get; set; }

        [Render(Control = RenderControl.Password)]
        public bool? IsActive { get; set; }

        public UserEditViewModel()
        {
            SelectedRoles = new List<int>();
        }
    }
}
