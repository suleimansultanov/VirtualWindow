using NasladdinPlace.Api.ViewModels;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Contracts;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Models;
using NasladdinPlace.Payment.Models;

namespace NasladdinPlace.Api.Services.BankingCardConfirmation
{
    public class Form3DsHtmlMaker : IForm3DsHtmlMaker
    {
        private readonly ViewRender.ViewRender _viewRender;
        private readonly string _termUrlWithUserIdFormat;

        public Form3DsHtmlMaker(
            ViewRender.ViewRender viewRender,
            string termUrlWithUserIdFormat)
        {
            _viewRender = viewRender;
            _termUrlWithUserIdFormat = termUrlWithUserIdFormat;
        }
        
        public string Make(Form3DsInfo form3DsInfo)
        {
            var info3Ds = form3DsInfo.Info3Ds;
            var transactionId = form3DsInfo.Info3Ds.TransactionId;
            
            var form3DsViewModel = new Form3DsViewModel(
                info3Ds.AcsUrl,
                info3Ds.PaReq,
                transactionId,
                string.Format(_termUrlWithUserIdFormat, form3DsInfo.UserId)
            );
            return _viewRender.Render("Form3Ds/Form3Ds", form3DsViewModel);
        }
    }
}