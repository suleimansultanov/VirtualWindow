using NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Providers
{
    /// <summary>
    ///     Конвертер бизнес запроса в транспортный
    /// </summary>
    internal class CheckOnlineTransportRequestProvider
    {
        /// <summary>
        ///     Признак получения полного ответа
        /// </summary>
        private const bool FullResponse = true;

        /// <summary>
        ///     Признак способа расчёта: Полная предварительная оплата до момента передачи предмета расчёта
        /// </summary>
        private const int MethodCalculation = 1;

        /// <summary>
        ///     Признак предмета расчёта: об оказываемой услуге
        /// </summary>
        private const int SubjectCalculation = 4;

        /// <summary>
        /// Тип коррекции (0: Самостоятельная корректировка, 1: Корректировка по предписанию)
        /// </summary>
        private const int CorrectionType = 0;

        private const int RequestPassword = 30;

        /// <summary>
        ///     Конвертация суммы в копейки
        /// </summary>
        /// <param name="amount">Сумма</param>
        private long ConvertAmount(decimal amount) => Convert.ToInt64(amount * 100);

        /// <summary>
        /// Конвертация в формат даты системы чек-онлайн
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private CheckOnlineResponseTransportDate ConvertToDate(DateTime dateTime) => new CheckOnlineResponseTransportDate
        {
            Day = dateTime.Day,
            Month = dateTime.Month,
            Year = dateTime.Year - 2000
        };

        /// <summary>
        ///     Конвертация количества в QTY
        /// </summary>
        /// <param name="count">Количество</param>
        private long ConvertCountToQty(int count) => count * 1000;

        /// <summary>
        ///     Конвертация в кодировку CP866
        /// </summary>
        /// <param name="string">Входящая строка</param>
        private string EncodeToCp866(string @string)
        {
            var bytes = Encoding.UTF8.GetBytes(@string);
            var convertBytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(866), bytes);
            return Encoding.GetEncoding(866).GetString(convertBytes);
        }

        /// <summary>
        ///     Конвертация бизнес модели в транспортную модель
        /// </summary>
        /// <param name="request">Бизнес модель</param>
        /// <param name="documentType">Тип операции фискализации (приход, расход, возврат прихода, возврат расхода)</param>
        internal CheckOnlineRequestTransport ToTransportRequest(CheckOnlineRequest request, DocumentType documentType)
        {
            var lines = request.Products.Select(product => new CheckOnlineRequestTransportLines
            {
                Description = EncodeToCp866(product.Name),
                Qty = ConvertCountToQty(product.Count),
                Price = ConvertAmount(product.Amount),
                TaxId = request.TaxCode,
                LineAttribute = SubjectCalculation,
                PayAttribute = MethodCalculation
            }).ToList();

            return new CheckOnlineRequestTransport
            {
                Device = "auto",
                DocumentType = (int)documentType,
                FullResponse = FullResponse,
                PhoneOrEmail = request.ClientPhoneOrEmail,
                RequestId = request.InvoiceId.ToString(),
                NonCash = new List<long> { lines.Sum(w => w.Price) },
                Lines = lines
            };
        }

        /// <summary>
        ///     Конвертация бизнес модели в транспортную модель (Коррекция чеков)
        /// </summary>
        /// <param name="request">Бизнес модель</param>
        internal CheckOnlineBatchRequestTransport ToTransportRequest(CheckOnlineCorrectionRequest request)
        {
            var commandModel = new CheckOnlineCorrectionRequestTransport
            {
                Password = RequestPassword,
                CorrectionType = CorrectionType,
                DocumentType = (int)(request.CorrectionAmount < 0 ? DocumentType.Income : DocumentType.Withdrawal),
                NonCash = ConvertAmount(Math.Abs(request.CorrectionAmount)),
                Reason = new CheckOnlineCorrectionRequestTransportReason
                {
                    Name = EncodeToCp866(request.CorrectionReason.DocumentName),
                    Number = request.CorrectionReason.DocumentNumber,
                    Date = ConvertToDate(request.CorrectionReason.DocumentDateTime)
                }
            };

            var batchModel = new CheckOnlineBatchRequestTransport
            {
                Device = "auto",
                Duration = 300,
                RequestId = request.InvoiceId.ToString(),
                ShortResponse = false,
                Requests = new List<CheckOnlineBatchRequestTransportCommand>
                {
                    new CheckOnlineBatchRequestTransportCommand
                    {
                        ContinueWhenDeviceError = true,
                        ContinueWhenTransportError = true,
                        Path = "/fr/api/v2/MakeCorrectionDocument",
                        Request = commandModel
                    }
                }
            };

            return batchModel;
        }
    }
}