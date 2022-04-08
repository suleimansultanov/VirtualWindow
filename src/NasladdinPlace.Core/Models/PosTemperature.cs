using System;

namespace NasladdinPlace.Core.Models
{
    public class PosTemperature : Entity
    {
        public int PosId { get; private set; }        
        public double Temperature { get; private set; }
        public DateTime DateCreated { get; private set; }

        public Pos Pos { get; private set; }

        protected PosTemperature()
        {
            //intentionally left empty
        }

        public PosTemperature(int posId, double temperature)
        {
            PosId = posId;
            Temperature = temperature;
            DateCreated = DateTime.UtcNow;
        }

	    public PosTemperature(int posId, double temperature, DateTime dateCreated)
        {
            PosId = posId;
            Temperature = temperature;
            DateCreated = dateCreated;
        }

        public static PosTemperature EmptyOfPos(int posId)
        {
            return new PosTemperature()
            {
                PosId = posId,
                Temperature = double.NaN,
                DateCreated = DateTime.MinValue
            };
        }

        public static PosTemperature EmptyOfPosForDate(int posId, DateTime date)
        {
            return new PosTemperature()
            {
                PosId = posId,
                Temperature = double.NaN,
                DateCreated = date
            };
        }

        public bool IsEmpty => double.IsNaN(Temperature) && DateCreated == DateTime.MinValue;
    }
}