using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models
{
    public class MessengerContact : Entity
    {
        public MessengerType Type { get; private set; }
        public string PhoneNumber { get; private set; }

        public MessengerContact(MessengerType type, string phoneNumber)
        {
            Type = type;
            PhoneNumber = phoneNumber;
        }
    }
}
