using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Core.Models.Goods
{
    public class GoodCategory : Entity
    {
        public static readonly GoodCategory Default = new GoodCategory(0, "Default");

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException($"{nameof(Name)} value must not be null.");

                _name = value;
            }
        }

        public string ImagePath { get; private set; }

        public ICollection<Good> Goods { get; private set; }

        protected GoodCategory()
        {
            Goods = new Collection<Good>();
        }

        public GoodCategory(int id, string name) : this(name)
        {
            Id = id;
        }

        public GoodCategory(string name) : this()
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public void SetImagePath(string imagePath)
        {
            ImagePath = imagePath;
        }

        public string GetImagePath()
        {
            return ImagePath;
        }
    }
}
