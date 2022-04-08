using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NasladdinPlace.IntegrationTestsSmsReader.Common
{
    public sealed class InboxEmails : IEnumerable<Email>
    {
        private readonly IEnumerable<Email> _emails;
        
        public InboxEmails(IEnumerable<Email> emails)
        {
            if (emails == null)
                throw new ArgumentNullException(nameof(emails));
            
            _emails = emails.ToImmutableList();
        }
        
        public IEnumerator<Email> GetEnumerator()
        {
            return _emails.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join("\n\n", _emails);
        }
    }
}