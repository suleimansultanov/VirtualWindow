using System;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos.OneCSync
{
    public class DateRangeDto   
    {
        [Required]
        public DateTime FromDate { get; set; }  
        public DateTime? ToDate { get; set; }  
    }
}