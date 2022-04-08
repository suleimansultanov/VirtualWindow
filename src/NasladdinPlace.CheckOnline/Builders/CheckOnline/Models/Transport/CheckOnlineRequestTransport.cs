using System.Collections.Generic;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    /// <summary>
    ///     Транспортная модель запроса chekonline
    /// </summary>
    public class CheckOnlineRequestTransport
    {
        /// <summary>
        ///     Должно иметь значение "auto"
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        ///     Уникальный идентификатор запроса
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        ///     Тип документа
        /// </summary>
        public int DocumentType { get; set; }

        /// <summary>
        ///     Массив товарных позиций
        /// </summary>
        public List<CheckOnlineRequestTransportLines> Lines { get; set; }

        /// <summary>
        ///     Телефон или электронный адрес покупателя. 
        ///     Формат «7XXXXXXXXXX» или «7-XXX-XXX-XX-XX».
        ///     Кодировка CP866
        /// </summary>
        public string PhoneOrEmail { get; set; }

        /// <summary>
        ///     Признак получения полного ответа
        /// </summary>
        public bool FullResponse { get; set; }

        /// <summary>
        ///     Разделение по типам (например: Visa, MasterCard, Мир) 
        ///     используется исключительно для внутреннего учёта пользователя устройства.
        ///     Если разбиение не требуется, то можно передавать в виде [1000].
        /// </summary>
        public List<long> NonCash { get; set; }
    }   
}