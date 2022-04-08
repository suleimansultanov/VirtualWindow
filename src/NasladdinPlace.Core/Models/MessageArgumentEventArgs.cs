using System;

namespace NasladdinPlace.Core.Models
{
    public class MessageArgumentEventArgs<T> : EventArgs
    {
        public T Message { get; }

        public MessageArgumentEventArgs(T message)
        {
            Message = message;
        }
    }
}