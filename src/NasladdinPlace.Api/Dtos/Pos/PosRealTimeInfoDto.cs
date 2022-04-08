using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Api.Dtos.LabeledGood;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Dtos;

namespace NasladdinPlace.Api.Dtos.Pos
{
    public class PosRealTimeInfoDto
    {
        public int Id { get; set; }
        public string Version { get; set; }
        public int ConnectionStatus { get; set; }
        public int DoorsState { get; set; }
        public double Temperature { get; set; }
        public double RfidTemperature { get; set; }
        public int LabelsNumber { get; set; }
        public int OverdueGoodsNumber { get; set; }

        public ScreenResolution ScreenResolution { get; set; }
        public Core.Enums.PosAntennasOutputPower AntennasOutputPower { get; set; }
        public IEnumerable<SensorMeasurementsDto> SensorsMeasurements { get; set; }
        public ICollection<HardToDetectLabelDto> HardToDetectLabels { get; set; }

        public PosRealTimeInfoDto()
        {
            HardToDetectLabels = new Collection<HardToDetectLabelDto>();
            AntennasOutputPower = Core.Enums.PosAntennasOutputPower.Dbm25;
            SensorsMeasurements = new Collection<SensorMeasurementsDto>();
        }
    }
}
