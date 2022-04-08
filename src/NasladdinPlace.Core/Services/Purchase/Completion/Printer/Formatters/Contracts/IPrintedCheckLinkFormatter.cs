namespace NasladdinPlace.Core.Services.Purchase.Completion.Printer.Formatters.Contracts
{
    public interface IPrintedCheckLinkFormatter
    {
        string ApplyFormat(string printedCheck, int operationId);
    }
}
