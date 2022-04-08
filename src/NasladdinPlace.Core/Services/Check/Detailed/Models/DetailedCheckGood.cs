using System;
using System.Collections.Generic;

namespace NasladdinPlace.Core.Services.Check.Detailed.Models
{
    public class DetailedCheckGood
    {
        public ICollection<DetailedCheckGoodInstance> Instances { get; }

        public DetailedCheckSummary Summary { get; }

        public int Id { get; }

        public string Name { get; }
        public int CategoryId { get; }
        public string CategoryName { get; }
        public DateTime? DatePaid { get; }

        public DetailedCheckGood(int id, string name, int categoryId, string categoryName, DateTime? datePaid,
            ICollection<DetailedCheckGoodInstance> instances)
        {
            Id = id;
            Name = name;
            CategoryId = categoryId;
            CategoryName = categoryName;
            DatePaid = datePaid;
            Instances = instances;
            Summary = DetailedCheckSummary.FromGoodInstances(instances);
        }
    }
}