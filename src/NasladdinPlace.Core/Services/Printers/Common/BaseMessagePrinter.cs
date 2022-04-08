using System.Collections.Generic;
using System.Text;

namespace NasladdinPlace.Core.Services.Printers.Common
{
    public abstract class BaseMessagePrinter<T> : IMessagePrinter<T>
    {
        public string Print(T entities)
        {
            var messageBuilder = new StringBuilder();

            var messagePieces = ProvideMessagePieces(entities);
            foreach (var messagePiece in messagePieces)
            {
                if (!messagePiece.EndsWith("\n"))
                {
                    messageBuilder.AppendLine(messagePiece);
                }
                else
                {
                    messageBuilder.Append(messagePiece);
                }
            }

            return messageBuilder.ToString();
        }

        protected abstract IEnumerable<string> ProvideMessagePieces(T entity);
    }
}