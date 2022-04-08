namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    public class CheckOnlineBatchRequestTransportCommand
    {
        /// <summary>
        /// Адрес команды для выполнения
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Продолжать выполнение последующих команд в случае транспортной ошибки
        /// </summary>
        public bool ContinueWhenTransportError { get; set; }

        /// <summary>
        /// Продолжать выполнение последующих команд в случае ошибки на устройстве
        /// </summary>
        public  bool ContinueWhenDeviceError { get; set; }

        /// <summary>
        /// Тело запроса
        /// </summary>
        public object Request { get; set; }
    }
}
