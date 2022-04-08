namespace NasladdinPlace.Utilities.Email
{
    public static class EmailStringExtension
    {
        public static bool IsEmail(this string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
