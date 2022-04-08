namespace NasladdinPlace.Core.Services.OverdueGoods.Printer.Formatters
{
    public interface IOrderedObjectStringFormatter<in T>
    {
        string ApplyFormat(T obj, int index);
    }
}