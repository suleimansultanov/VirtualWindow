using System;
using NasladdinPlace.Api.Dtos.Good;
using NasladdinPlace.Core.Services.MicroNutrients.Models;
using System.Collections.Generic;

namespace NasladdinPlace.Api.Services.MicroNutrients.Models
{
    public class UserNutrientsHistory
    {
        public Nutrients Nutrients { get; set; }
        public Nutrients Normal { get; set; }
        public DateTime Date { get; set; }
        public IList<GoodWithPfccDto> Products { get; set; }
    }
}
