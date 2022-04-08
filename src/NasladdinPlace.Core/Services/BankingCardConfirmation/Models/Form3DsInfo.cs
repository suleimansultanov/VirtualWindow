using NasladdinPlace.Payment.Models;

namespace NasladdinPlace.Core.Services.BankingCardConfirmation.Models
{
    public class Form3DsInfo
    {
        public int UserId { get; }
        public Info3Ds Info3Ds { get; }

        public Form3DsInfo(int userId, Info3Ds info3Ds)
        {
            UserId = userId;
            Info3Ds = info3Ds;
        }
    }
}