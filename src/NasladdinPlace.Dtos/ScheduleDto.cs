using System;

namespace NasladdinPlace.Dtos
{
    public class ScheduleDto
    {
        public string Name { get; set; }

        public DateTime NextRun { get; set; }

        public bool Disabled { get; set; }
    }
}
