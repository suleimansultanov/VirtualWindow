using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Pos.Sensors.Models;
using NasladdinPlace.Core.Services.Pos.WebSocket.CommandsExecution;
using Newtonsoft.Json;

namespace NasladdinPlace.Core.Models
{
    public class PosRealTimeInfo
    {   
        private readonly object _connectionStatusLock = new object();
        private readonly object _doorsStateLock = new object();
        private readonly object _labelsNumberLock = new object();
        private readonly object _hardToDetectLabelsLock = new object();
        private readonly object _overdueGoodsNumberLock = new object();
        private readonly object _antennasOutputPowerLock = new object();
        private readonly object _posIpAddressesLock = new object();
        private readonly object _sensorMeasurementsLock = new object();
        private readonly object _rfidTemperatureLock = new object();
        private readonly object _versionLock = new object();
        private readonly object _purchaseProgressLock = new object();
        private readonly object _posActivityStatusLock = new object();
        private readonly object _devicesLock = new object();

        public int Id { get; }

        private PosConnectionStatus _connectionStatus;
        private DoorsState _doorsState;
        
        private int _labeledGoodsNumber;
        private int _overdueGoodsNumber;
        private PosAntennasOutputPower _antennasOutputPower;
        private readonly ConcurrentDictionary<string, byte> _hardToDetectLabelsDictionary;
        private readonly ConcurrentDictionary<IPAddress, byte> _ipAddressesDictionary;
        private readonly ConcurrentDictionary<SensorPosition, SensorMeasurements> _sensorMeasurementsDictionary;
        private DateTime _contentSyncDateTime;
        private DateTime _doorsStateSyncDateTime;
        private double _rfidTemperature;
        private string _version;
        private bool _isPurchaseInProgress;
        private readonly ICollection<PosDeviceInfo> _devices;
        private PosActivityStatus _posActivityStatus;

        public PosRealTimeInfo(int id)
        {
            Id = id;
            ConnectionStatus = PosConnectionStatus.Disconnected;
            DoorsState = DoorsState.DoorsClosed;
            LabelsNumber = 0;
            AntennasOutputPower = PosAntennasOutputPower.Dbm0;
            _hardToDetectLabelsDictionary = new ConcurrentDictionary<string, byte>();
            _ipAddressesDictionary = new ConcurrentDictionary<IPAddress, byte>();
            _sensorMeasurementsDictionary = new ConcurrentDictionary<SensorPosition, SensorMeasurements>();
            UpdatableScreenResolution = new UpdatableScreenResolution();
            LastReceivedWsMessage = new LastReceivedWsMessage();
            _devices = new List<PosDeviceInfo>();

            DoorsStateSyncDateTime = DateTime.UtcNow;
            ContentSyncDateTime = DateTime.UtcNow;
        }

        public PosConnectionStatus ConnectionStatus
        {
            get
            {
                lock (_connectionStatusLock)
                {
                    return _connectionStatus;
                }
            }
            set
            {
                lock (_connectionStatusLock)
                {
                    _connectionStatus = value;
                }
            }
        }

        public DoorsState DoorsState
        {
            get
            {
                lock (_doorsStateLock)
                {
                    return _doorsState;
                }
            }
            set
            {
                lock (_doorsStateLock)
                {
                    _doorsState = value;
                }
            }
        }

        public DateTime DoorsStateSyncDateTime
        {
            get
            {
                lock (_doorsStateLock)
                {
                    return _doorsStateSyncDateTime;
                }
            }
            set
            {
                lock (_doorsStateLock)
                {
                    _doorsStateSyncDateTime = value;
                }
            }
        }

        public int LabelsNumber
        {
            get
            {
                lock (_labelsNumberLock)
                {
                    return _labeledGoodsNumber;
                }
            }
            set
            {
                if (value < 0)
                    return;
                
                lock (_labelsNumberLock)
                {
                    _labeledGoodsNumber = value;
                }
            }
        }

        public int OverdueGoodsNumber
        {
            get
            {
                lock (_overdueGoodsNumberLock)
                {
                    return _overdueGoodsNumber;
                }
            }
            set
            {
                lock (_overdueGoodsNumberLock)
                {
                    _overdueGoodsNumber = value;
                }
            }
        }

        public PosAntennasOutputPower AntennasOutputPower
        {
            get
            {
                lock (_antennasOutputPowerLock)
                {
                    return _antennasOutputPower;
                }
            }
            set
            {
                lock (_antennasOutputPowerLock)
                {
                    _antennasOutputPower = value;
                }
            }
        }

        [JsonIgnore]
        public IEnumerable<IPAddress> IpAddresses
        {
            get
            {
                lock (_posIpAddressesLock)
                {
                    return _ipAddressesDictionary.Keys;
                }
            }
            set
            {
                lock (_posIpAddressesLock)
                {
                    _ipAddressesDictionary.Clear();

                    foreach (var ipAddress in value)
                    {
                        _ipAddressesDictionary[ipAddress] = byte.MaxValue;

                    }
                }
            }
        }

        public double TemperatureInsidePos
        {
            get
            {
                lock (_sensorMeasurementsLock)
                {
                    return _sensorMeasurementsDictionary.ContainsKey(SensorPosition.InsidePos)
                        ? _sensorMeasurementsDictionary[SensorPosition.InsidePos].Temperature
                        : 0;
                }
            }
        }

        public double PosElectricCircuitAmperage
        {
            get
            {
                lock (_sensorMeasurementsLock)
                {
                    return _sensorMeasurementsDictionary.ContainsKey(SensorPosition.InsidePos)
                        ? _sensorMeasurementsDictionary[SensorPosition.InsidePos].Temperature
                        : 0;
                }
            }
        }

        public FrontPanelPosition FrontPanelPosition
        {
            get
            {
                lock (_sensorMeasurementsLock)
                {
                    return _sensorMeasurementsDictionary.ContainsKey(SensorPosition.InsidePos)
                        ? _sensorMeasurementsDictionary[SensorPosition.InsidePos].FrontPanelPosition
                        : FrontPanelPosition.NotFound;
                }
            }
        }

        public double RfidTemperature
        {
            get
            {
                lock (_rfidTemperatureLock)
                {
                    return _rfidTemperature;
                }

            }
            set
            {
                lock (_rfidTemperatureLock)
                {
                    _rfidTemperature = value;
                }
            }
        }
        

        public IEnumerable<SensorMeasurements> SensorsMeasurements
        {
            get
            {
                lock (_sensorMeasurementsLock)
                {
                    return _sensorMeasurementsDictionary.Values.OrderBy(sm => sm.SensorPosition).ToImmutableList();
                }
            }
            set
            {
                lock (_sensorMeasurementsLock)
                {
                    _sensorMeasurementsDictionary.Clear();

                    foreach (var sensorMeasurements in value)
                    {
                        _sensorMeasurementsDictionary[sensorMeasurements.SensorPosition] = sensorMeasurements;
                    }
                }
            }
        }

        public ICollection<string> HardToDetectLabels
        {
            get
            {
                lock (_hardToDetectLabelsLock)
                {
                    return _hardToDetectLabelsDictionary.Keys;
                }
            }
            set
            {
                lock (_hardToDetectLabelsLock)
                {
                    _hardToDetectLabelsDictionary.Clear();

                    foreach (var hardToDetectLabel in value)
                    {
                        _hardToDetectLabelsDictionary[hardToDetectLabel] = byte.MaxValue;
                    }
                }

            }
        }

        public DateTime ContentSyncDateTime
        {
            get
            {
                lock (_connectionStatusLock)
                {
                    return _contentSyncDateTime;
                }
            }
            set
            {
                lock (_connectionStatusLock)
                {
                    _contentSyncDateTime = value;
                }
            }
        }

        public string Version
        {
            get
            {
                lock (_versionLock)
                {
                    return _version;
                }
            }
            set
            {   
                lock (_versionLock)
                {
                    _version = value;
                }
            }
        }

        public bool IsPurchaseInProgress
        {
            get
            {
                lock (_purchaseProgressLock)
                {
                    return _isPurchaseInProgress;
                }
            }
            set
            {
                lock (_purchaseProgressLock)
                {
                    _isPurchaseInProgress = value;
                }
            }
        }

        public PosActivityStatus PosActivityStatus
        {
            get
            {
                lock (_posActivityStatusLock)
                {
                    return _posActivityStatus;
                }
            }
            set
            {
                lock (_posActivityStatusLock)
                {
                    _posActivityStatus = value;
                }
            }
        }

        public ICollection<PosDeviceInfo> Devices
        {
            get
            {
                lock (_devicesLock)
                {
                    return _devices;
                }
            }
            set
            {
                lock (_devicesLock)
                {
                    _devices.Clear();

                    foreach (var device in value)
                    {
                        _devices.Add(device);
                    }
                }

            }
        }

        public LastReceivedWsMessage LastReceivedWsMessage { get; }

        public UpdatableScreenResolution UpdatableScreenResolution { get; }

        [JsonIgnore]
        public IWsCommandsQueueProcessor CommandsQueueProcessor { get; set; }


    }
}