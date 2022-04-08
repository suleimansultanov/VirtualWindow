using System;

namespace NasladdinPlace.Logging.Models
{
    public class Log
    {
        public string Level { get; }
        public DateTimeOffset Timestamp { get; }
        public string Content { get; }

        public Log(string level, DateTimeOffset timestamp, string content)
        {
            Level = level;
            Timestamp = timestamp;
            Content = content;
        }
    }
}

