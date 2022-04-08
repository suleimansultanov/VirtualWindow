using System;
using System.Text;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Providers;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Validators.Contracts;
using NasladdinPlace.CheckOnline.Infrastructure;
using NasladdinPlace.CheckOnline.Infrastructure.IModels;
using NasladdinPlace.CheckOnline.Infrastructure.Models;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline
{
    /// <summary>
    ///     ЧЕК онлайн.
    ///     URL: http://chekonline.ru
    /// </summary>
    public class CheckOnlineBuilder : ICheckOnlineBuilder
    {
        private readonly CheckOnlineTransportRequestProvider _transportRequestProvider;
        private readonly CheckOnlineTransportResponseProvider _transportResponseProvider;
        private readonly ICheckOnlineRequestProvider _requestProvider;
        private readonly IValidator<CheckOnlineAuth> _checkOnlineModelValidator;
        private readonly IValidator<CheckOnlineRequest> _requestModelValidator;
        private readonly IValidator<CheckOnlineCorrectionRequest> _checkOnlineCorrectionValidator;

        public CheckOnlineBuilder(
            ICheckOnlineRequestProvider checkOnlineRequest,
            IValidator<CheckOnlineAuth> checkOnlineModelValidator,
            IValidator<CheckOnlineRequest> requestModelValidator,
            IValidator<CheckOnlineCorrectionRequest> checkOnlineCorrectionValidator)
        {
            if(checkOnlineRequest == null)
                throw new ArgumentNullException(nameof(checkOnlineRequest));
            if (checkOnlineModelValidator == null)
                throw new ArgumentNullException(nameof(checkOnlineModelValidator));
            if (requestModelValidator == null)
                throw new ArgumentNullException(nameof(requestModelValidator));
            if (checkOnlineCorrectionValidator == null)
                throw new ArgumentNullException(nameof(checkOnlineCorrectionValidator));

            _requestProvider = checkOnlineRequest;
            _transportRequestProvider = new CheckOnlineTransportRequestProvider();
            _transportResponseProvider = new CheckOnlineTransportResponseProvider();
            _checkOnlineModelValidator = checkOnlineModelValidator;
            _requestModelValidator = requestModelValidator;
            _checkOnlineCorrectionValidator = checkOnlineCorrectionValidator;
        }

        /// <summary>
        ///     Получение чека
        /// </summary>
        /// <param name="authData">Данные авторизации</param>
        /// <param name="request">Модель выставленного счета</param>
        /// <param name="fiscalizationType">Тип фискализации (обычная, возврат прихода, коррекция)</param>
        public IOnlineCashierResponse BuildCheck(IOnlineCashierAuth authData, IOnlineCashierRequest request, FiscalizationType fiscalizationType)
        {
            DocumentType documentType;
            switch (fiscalizationType)
            {
                case FiscalizationType.Income:
                    documentType = DocumentType.Income;
                    break;
                case FiscalizationType.IncomeRefund:
                    documentType = DocumentType.IncomeRefund;
                    break;
                default:
                    throw new ArgumentException(nameof(DocumentType));
            }

            var transportRequest = _transportRequestProvider.ToTransportRequest((CheckOnlineRequest) request, documentType);

            var responseTransport = _requestProvider.SendRequest((CheckOnlineAuth) authData, transportRequest);
            if (!string.IsNullOrEmpty(responseTransport.RequestError))
                return new BaseOnlineCashierResponse {Errors = responseTransport.RequestError};
            
            var businessResponse = _transportResponseProvider.ToBusinessModel(responseTransport);
            return businessResponse;
        }

        /// <summary>
        ///     Получение чека коррекции
        /// </summary>
        /// <param name="authData">Данные авторизации</param>
        /// <param name="request">Модель для коррекции чека</param>
        public IOnlineCashierResponse BuildCorrectionCheck(IOnlineCashierAuth authData, IOnlineCashierCorrectionRequest request)
        {
            var transportRequest = _transportRequestProvider.ToTransportRequest((CheckOnlineCorrectionRequest)request);

            var responseTransport = _requestProvider.SendRequest((CheckOnlineAuth)authData, transportRequest);
            if (!string.IsNullOrEmpty(responseTransport.RequestError))
                return new BaseOnlineCashierResponse { Errors = responseTransport.RequestError };

            var businessResponse = _transportResponseProvider.ToCorrectionCheckBusinessModel(responseTransport);
            return businessResponse;
        }

        /// <summary>
        ///     Валидация запроса
        /// </summary>
        /// <param name="authData">Данные авторизации</param>
        /// <param name="request">Модель выставленного счета</param>
        /// <param name="errors">Ошибки</param>
        public bool ValidateRequest(IOnlineCashierAuth authData, IOnlineCashierRequest request, out string errors)
        {
            if (!(authData is CheckOnlineAuth))
            {
                errors = "Неверная авторизационная модель запроса. Поддерживается только " + nameof(CheckOnlineAuth);
                return false;
            }

            if (!(request is CheckOnlineRequest))
            {
                errors = "Неверная бизнес модель запроса. Поддерживается только " + nameof(CheckOnlineRequest);
                return false;
            }

            var errorList = new StringBuilder();

            var authDataModel = (CheckOnlineAuth)authData;
            var requestModel = (CheckOnlineRequest)request;

            var authModelValidationErrors = _checkOnlineModelValidator.Validate(authDataModel);
            errorList.Append(authModelValidationErrors);
            var requestModelValidationErrors = _requestModelValidator.Validate(requestModel);
            errorList.Append(requestModelValidationErrors);

            errors = errorList.ToString().Trim();
            return string.IsNullOrEmpty(errors);
        }

        /// <summary>
        ///     Валидация модели коррекции чека
        /// </summary>
        /// <param name="authData">Данные авторизации</param>
        /// <param name="request">Модель на создание чека</param>
        /// <param name="errors">Ошибки</param>
        public bool ValidateCorrectionRequest(IOnlineCashierAuth authData, IOnlineCashierCorrectionRequest request, out string errors)
        {
            if (!(authData is CheckOnlineAuth))
            {
                errors = "Неверная авторизационная модель запроса. Поддерживается только " + nameof(CheckOnlineAuth);
                return false;
            }

            if (!(request is CheckOnlineCorrectionRequest))
            {
                errors = "Неверная бизнес модель запроса. Поддерживается только " + nameof(CheckOnlineCorrectionRequest);
                return false;
            }

            var errorList = new StringBuilder();

            var authDataModel = authData as CheckOnlineAuth;
            var requestModel = request as CheckOnlineCorrectionRequest;

            var authModelValidationErrors = _checkOnlineModelValidator.Validate(authDataModel);
            errorList.Append(authModelValidationErrors);

            var correctionModelValidationErrors = _checkOnlineCorrectionValidator.Validate(requestModel);
            errorList.Append(correctionModelValidationErrors);

            errors = errorList.ToString().Trim();
            return string.IsNullOrEmpty(errors);
        }
    }
}