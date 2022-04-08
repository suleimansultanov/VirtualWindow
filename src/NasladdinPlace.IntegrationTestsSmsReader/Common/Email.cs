using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NasladdinPlace.IntegrationTestsSmsReader.Common
{
    public sealed class Email
    {
        public string Content { get; }
        public IEnumerable<string> Senders { get; }
        public DateTime Date { get; }

        public Email(string content, IEnumerable<string> senders, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentNullException(nameof(content));
            if (senders == null)
                throw new ArgumentNullException(nameof(senders));
            
            Content = content;
            Senders = senders.ToImmutableSortedSet();
            Date = date;
        }

        public bool CheckWhetherSentFrom(string sender)
        {
            if (string.IsNullOrWhiteSpace(sender))
                throw new ArgumentNullException(nameof(sender));

            return Senders.Contains(sender);
        }

        public override string ToString()
        {
            var senders = string.Join(", ", Senders);
            return $"From: {senders}\n" +
                   Content;
        }
    }
}