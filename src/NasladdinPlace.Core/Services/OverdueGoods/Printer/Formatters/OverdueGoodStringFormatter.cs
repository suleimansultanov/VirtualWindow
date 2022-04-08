using System;
using NasladdinPlace.Core.Services.OverdueGoods.Models;

namespace NasladdinPlace.Core.Services.OverdueGoods.Printer.Formatters
{
    public class OverdueGoodStringFormatter : IOrderedObjectStringFormatter<GoodInstance>
    {
        public string ApplyFormat(GoodInstance obj, int index)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            
            var goodInstance = obj;
            return $"#{index + 1}\n" +
                   $"Наименование: {goodInstance.Name}\n" +
                   $"Цена: {goodInstance.Price}\n" +
                   $"Метка: {goodInstance.Label}\n" +
                   $"Дата просрочки: {goodInstance.ExpirationDate}\n" +
                   $"Магазин: {goodInstance.PosName}";
        }
    }
}