using System;
using System.Text;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Validators.Contracts;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Validators
{
    public class CheckOnlineCorrectionModelValidator : IValidator<CheckOnlineCorrectionRequest>
    {
        public string Validate(CheckOnlineCorrectionRequest model)
        {
            var errorList = new StringBuilder();
            if (model.InvoiceId == Guid.Empty)
                errorList.AppendLine("Неверный ID запроса");

            if (model.CorrectionAmount == 0)
                errorList.AppendLine("Сумма коррекции не может быть равна 0");

            return errorList.ToString();
        }
    }
}
