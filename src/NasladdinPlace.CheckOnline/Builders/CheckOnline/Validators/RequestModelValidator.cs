using System;
using System.Linq;
using System.Text;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Validators.Contracts;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Validators
{
    public class RequestModelValidator : IValidator<CheckOnlineRequest>
    {
        public string Validate(CheckOnlineRequest model)
        {
            var errorList = new StringBuilder();
            if (string.IsNullOrEmpty(model.ClientPhoneOrEmail))
                errorList.AppendLine("Email клиента отсутствует");

            if (model.InvoiceId == Guid.Empty)
                errorList.AppendLine("Неверный ID выплаты");

            if (model.Products == null || !model.Products.Any())
                errorList.AppendLine("Нет ни одного товара/услуги");
            else
            {
                var count = 1;
                model.Products.ForEach(product =>
                {
                    if (string.IsNullOrEmpty(product.Name))
                        errorList.AppendLine($"[{count}] Описание товара/услуги отсутствует");

                    if (product.Amount < 0)
                        errorList.AppendLine($"[{count}] Сумма должна быть положительной");

                    if (product.Count <= 0)
                        errorList.AppendLine($"[{count}] Количество должно быть больше нуля");

                    count++;
                });
            }

            return errorList.ToString();
        }
    }
}