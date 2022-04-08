using System;

namespace NasladdinPlace.Core.Models
{
    public class Maker : Entity, ICommonHandbook
    {
        public static readonly Maker Default = new Maker(0, "Default");
        
        public string Name { get; private set; }

        protected Maker()
        {
            // required for EF
        }

        public Maker(int id, string name)
        {
            SetName(name);
            Id = id;
        }

        public Maker(string name)
        {
            SetName(name);
        }

        public void SetMakerName(string name)
        {
            SetName(name);
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }
    }
}
