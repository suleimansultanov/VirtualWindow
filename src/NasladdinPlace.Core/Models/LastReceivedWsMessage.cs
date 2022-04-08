using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.Core.Models
{
    public class LastReceivedWsMessage
    {
        private readonly object _lastReceivedWsMessageLock = new object();

        private DateTime _dateReceived;
        private string _controllerName;
        private string _method;
        private PosActivityStatus _posActivityStatus;
        private bool _isUpdated;

        public DateTime DateReceived
        {
            get
            {
                lock (_lastReceivedWsMessageLock)
                {
                    return _dateReceived;
                }
            }
            private set
            {
                lock (_lastReceivedWsMessageLock)
                {
                    _dateReceived = value;
                }
            }
        }

        public string ControllerName
        {
            get
            {
                lock (_lastReceivedWsMessageLock)
                {
                    return _controllerName;
                }
            }
            private set
            {
                lock (_lastReceivedWsMessageLock)
                {
                    _controllerName = value;
                }
            }
        }

        public string Method
        {
            get
            {
                lock (_lastReceivedWsMessageLock)
                {
                    return _method;
                }
            }
            private set
            {
                lock (_lastReceivedWsMessageLock)
                {
                    _method = value;
                }
            }
        }

        public bool IsDeactivatedPosReceivingWsMessages
        {
            get
            {
                lock (_lastReceivedWsMessageLock)
                {
                    return _isUpdated && _posActivityStatus == PosActivityStatus.Inactive;
                }
            }
        }

        public void Update(string controllerName, string method, PosActivityStatus posActivityStatus)
        {
            lock (_lastReceivedWsMessageLock)
            {
                ControllerName = controllerName;
                Method = method;
                DateReceived = DateTime.UtcNow;
                _posActivityStatus = posActivityStatus;
                _isUpdated = true;
            }
        }

        public override string ToString()
        {
            return _isUpdated ?
                $"{UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(DateReceived)} витрина получила запрос к {ControllerName}.{Method}":
                string.Empty;
        }
    }
}