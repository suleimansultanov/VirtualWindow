using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Validators.Contracts;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Validators
{
    public class CheckOnlineModelValidator : IValidator<CheckOnlineAuth>
    {
        public string Validate(CheckOnlineAuth model)
        {
            var errorList = new StringBuilder();
            if (string.IsNullOrEmpty(model.CertificateData))
                errorList.AppendLine("Данные о сертификате отсутствуют");

            if (string.IsNullOrEmpty(model.CertificatePassword))
                errorList.AppendLine("Пароль сертификата отсутствуют");

            if (string.IsNullOrEmpty(model.ServiceUrl))
                errorList.AppendLine("Адрес сервиса chekonline отсутствует");

            try
            {
                var certificateData = Convert.FromBase64String(model.CertificateData);
                var _ = new X509Certificate2(certificateData, model.CertificatePassword);
            }
            catch (Exception e)
            {
                errorList.AppendLine($"Ошибка сертификата безопасности для чек-онлайн ({e})");
            }

            return errorList.ToString();
        }
    }
}