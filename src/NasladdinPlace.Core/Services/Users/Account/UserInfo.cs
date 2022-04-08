namespace NasladdinPlace.Core.Services.Users.Account
{
    public class UserInfo
    {
        public int UserId { get; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int? Gender { get; set; }
        public long? BirthDateMilliseconds { get; set; }
        public int? Goal { get; set; }
        public int? Activity { get; set; }
        public int? Pregnancy { get; set; }
        public int? Age { get; set; }
        public int? Height { get; set; }
        public int? Weight { get; set; }

        public UserInfo(int userId)
        {
            UserId = userId;
        }
    }
}
