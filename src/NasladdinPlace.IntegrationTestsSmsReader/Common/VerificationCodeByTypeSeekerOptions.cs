using System;

namespace NasladdinPlace.IntegrationTestsSmsReader.Common
{
    public sealed class VerificationCodeByTypeSeekerOptions
    {
        public Inbox Inbox { get; }

        public byte EmailsLimit { get; set; } = 10;
        public byte MaxEmailsReadingAttemptsNumber { get; set; } = 3;
        public TimeSpan SearchRepetitionInterval { get; set; } = TimeSpan.FromSeconds(5);
        public byte MaxSearchAttempts { get; set; } = 3;

        public VerificationCodeByTypeSeekerOptions(Inbox inbox)
        {
            if (inbox == null)
                throw new ArgumentNullException(nameof(inbox));
            
            Inbox = inbox;
        }
    }
}