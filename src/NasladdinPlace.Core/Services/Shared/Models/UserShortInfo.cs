namespace NasladdinPlace.Core.Services.Shared.Models
{
    public class UserShortInfo
    {
        public int Id { get; }
        public string UserName { get; }

        public UserShortInfo(int id, string userName)
        {
            Id = id;
            UserName = userName;
        }
    }
}