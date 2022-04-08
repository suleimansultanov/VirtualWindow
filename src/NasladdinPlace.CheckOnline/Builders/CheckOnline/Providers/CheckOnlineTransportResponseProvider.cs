using System;
using System.Linq;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport;
using NasladdinPlace.CheckOnline.Infrastructure.Helpers;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Providers
{
    /// <summary>
    ///     Конвертер транспортного ответа в бизнес
    /// </summary>
    internal class CheckOnlineTransportResponseProvider
    {
        /// <summary>
        ///     Путь к объекту о закрытии документа
        /// </summary>
        private const string PathCloseDocument = "/fr/api/v2/CloseDocument";

        /// <summary>
        ///     Конвертация транспортной модели в бизнес модель
        /// </summary>
        /// <param name="transportModel">Транспортная модель</param>
        public CheckOnlineResponse ToBusinessModel(CheckOnlineResponseTransport transportModel)
        {
            if (transportModel == null)
                return new CheckOnlineResponse { Errors = "ChekOnlineResponseTransport null" };

            if (transportModel.Response == null)
                return new CheckOnlineResponse { Errors = "ChekOnlineResponseTransport.Response null" };

            if (transportModel.Response.Error != 0)
                return new CheckOnlineResponse { Errors = string.Join(Environment.NewLine, transportModel.Response.ErrorMessages) };

            if (transportModel.Responses == null)
                return new CheckOnlineResponse { Errors = "ChekOnlineResponseTransport.Responses null" };

            var response = transportModel.Responses.FirstOrDefault(w => w.Path == PathCloseDocument)?.Response;
            if (string.IsNullOrEmpty(response?.ReceiptFullText))
                return new CheckOnlineResponse { Errors = "Отсутствует информация о закрытии чека" };
            
            if (string.IsNullOrWhiteSpace(transportModel.QrCode))
                return new CheckOnlineResponse { Errors = "QR код пустой" };
            
            return new CheckOnlineResponse
            {
                DocumentDateTime = new DateTime(
                    transportModel.Date.Date.Year, transportModel.Date.Date.Month, transportModel.Date.Date.Day, 
                    transportModel.Date.Time.Hour, transportModel.Date.Time.Minute, transportModel.Date.Time.Second),
                ReceiptInfo = response.ReceiptFullText,
                QrCodeValue = transportModel.QrCode,
                FiscalData = new CheckOnlineResponseFiscalData
                {
                    Number = transportModel.FiscalDocNumber.ToString(),
                    Serial = transportModel.FnSerialNumber,
                    Sign = transportModel.FiscalSign.ToString()
                },
                IsSuccess = true,
                TotalFiscalizationAmount = Convert.ToDecimal(transportModel.GrandTotal) / 100
            };
        }

        /// <summary>
        ///     Конвертация транспортной модели в бизнес модель
        /// </summary>
        /// <param name="batchTransportModel">Транспортная модель batch</param>
        public CheckOnlineCorrectionResponse ToCorrectionCheckBusinessModel(CheckOnlineBatchResponseTransport batchTransportModel)
        {
            var correctionCheckCommandResponse = batchTransportModel.Responses.FirstOrDefault();
            if (correctionCheckCommandResponse == null)
                return new CheckOnlineCorrectionResponse { Errors = "Отсутствует информация о результате выполнения коррекции чека" };

            if (!string.IsNullOrEmpty(correctionCheckCommandResponse.ExchangeError))
                return new CheckOnlineCorrectionResponse { Errors = $"Ошибка при выполнении команды: {correctionCheckCommandResponse.ExchangeError}" };

            var transportModel = correctionCheckCommandResponse.Response.ToObject<CheckOnlineCorrectionResponseTransport>();
            if (transportModel.ErrorCode > 0)
                return new CheckOnlineCorrectionResponse { Errors = $"Ошибка данных команды. Код ошибки: {transportModel.ErrorCode}. Описание: {string.Join("; ", transportModel.ErrorMessages)}"};

            if (transportModel.FiscalDocument == null)
                return new CheckOnlineCorrectionResponse { Errors = "Отсутствует фискальный документ" };

            return new CheckOnlineCorrectionResponse
            {
                DocumentDateTime = new DateTime(
                    transportModel.Date.Date.Year, transportModel.Date.Date.Month, transportModel.Date.Date.Day,
                    transportModel.Date.Time.Hour, transportModel.Date.Time.Minute, transportModel.Date.Time.Second),
                ReceiptInfo = DocumentBuilder.BuildCorrectionDocument(transportModel.FiscalDocument.Tags),
                FiscalData = new CheckOnlineResponseFiscalData
                {
                    Number = transportModel.FiscalDocNumber.ToString(),
                    Serial = transportModel.FiscalSign.ToString()
                },
                IsSuccess = true
            };
        }
    }
}