using System;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models
{
    public class DiagnosticsStepInfo
    {
        public string Name { get; }
        public string Description { get; set; }

        public DiagnosticsStepInfo(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(nameof(name));
            
            Name = name;
            Description = string.Empty;
        }
    }
}