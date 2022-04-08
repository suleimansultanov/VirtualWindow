using System;

namespace NasladdinPlace.Fiscalization.Models
{
    public class CheckInfoRequest
    {
        public string Id { get; }

        public CheckInfoRequest(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            Id = id;
        }
    }
}
