using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NasladdinPlace.Core.Models
{
    public class PosScreenTemplate : Entity, ICommonHandbook
    {
        private string _name;

        public ICollection<Pos> PointsOfSale { get; private set; }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException($"{nameof(_name)} must not be null.");

                _name = value;
            }
        }

        protected PosScreenTemplate()
        {
            PointsOfSale = new Collection<Pos>();
        }

        public PosScreenTemplate(string name) : this()
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public PosScreenTemplate(int id, string name) : this(name)
        {
            Id = id;
        }

        public void UnlinkPointsOfSale(int defaultTemplate)
        {
            foreach(var pointOfSale in PointsOfSale)
            {
                pointOfSale.UpdatePosScreenTemplate(defaultTemplate);
            }
        }

        public bool HasLinkedPointsOfSale => PointsOfSale.Any();
    }
}