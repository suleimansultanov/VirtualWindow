namespace NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer.Formatters.Contracts
{
    public interface IBaseFormatter
    {
        string ApplyFormat(string printedLink, int posId);
    }
}
