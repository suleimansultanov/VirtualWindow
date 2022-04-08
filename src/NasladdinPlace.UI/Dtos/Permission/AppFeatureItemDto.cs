using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.UI.Dtos.Permission
{
    public class AppFeatureItemDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public PermissionCategory PermissionCategory { get; set; }
    }
}
