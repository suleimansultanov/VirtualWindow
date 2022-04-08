using NasladdinPlace.Core.Services.BankingCardConfirmation.Models;

namespace NasladdinPlace.Core.Services.BankingCardConfirmation.Contracts
{
    public interface IForm3DsHtmlMaker
    {
        string Make(Form3DsInfo form3DsInfo);
    }
}