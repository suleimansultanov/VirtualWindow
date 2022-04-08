using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NasladdinPlace.IntegrationTestsSmsReader.Common;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.EmailTypeDetection
{
    public class EmailTypeCompositeDetector : IEmailTypeDetector
    {
        private readonly IEnumerable<IEmailTypeDetector> _emailTypeDetectors;
        
        public EmailTypeCompositeDetector(IEnumerable<IEmailTypeDetector> emailTypeDetectors)
        {
            if (emailTypeDetectors == null)
                throw new ArgumentNullException(nameof(emailTypeDetectors));
            
            _emailTypeDetectors = emailTypeDetectors.ToImmutableList();
        }
        public EmailType Detect(Email email)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));
            
            foreach (var emailTypeDetector in _emailTypeDetectors)
            {
                var type = emailTypeDetector.Detect(email);
                if (type != EmailType.Unknown)
                {
                    return type;
                }
            }

            return EmailType.Unknown;
        }
    }
}