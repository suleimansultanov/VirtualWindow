using System.Collections.Generic;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    public class CheckOnlineBatchRequestTransport
    {
        /// <summary>
        ///     Номер кассы
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        ///     Определяет стратегию выбора устройства.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        ///     Уникальный идентификатор запроса
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        ///     Признак получения короткого ответа
        /// </summary>
        public bool ShortResponse { get; set; }

        /// <summary>
        ///     Пакет с командами для выполнения
        /// </summary>
        public List<CheckOnlineBatchRequestTransportCommand> Requests { get; set; }
    }
}
