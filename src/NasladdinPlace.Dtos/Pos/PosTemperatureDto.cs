using System;

namespace NasladdinPlace.Dtos.Pos
{
    public class PosTemperatureDto
    {
        public int PosId { get; set; }
        public DateTime DateTimeTemperatureReceipt { get; set; }
        public double Temperature { get; set; }
    }
}
