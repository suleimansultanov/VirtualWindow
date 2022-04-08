using NasladdinPlace.Core.Services.Purchase.Completion.Printer.Formatters.Contracts;

namespace NasladdinPlace.Core.Services.Purchase.Completion.Printer.Formatters
{
    public class PrintedCheckLinkFormatter : IPrintedCheckLinkFormatter
    {
        private readonly string _administrationDetailsLink;

        public PrintedCheckLinkFormatter(string administrationDetailsLink)
        {
            _administrationDetailsLink = administrationDetailsLink;
        }

        public string ApplyFormat(string printedCheck, int operationId)
        {
            return $"[{printedCheck}]" +
                   $"({_administrationDetailsLink}{operationId})";
        }
    }
}
