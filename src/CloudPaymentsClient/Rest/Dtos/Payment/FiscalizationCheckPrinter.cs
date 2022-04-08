using System.Text;
using CloudPaymentsClient.Rest.Dtos.Fiscalization;

namespace CloudPaymentsClient.Rest.Dtos.Payment
{
    public class FiscalizationCheckPrinter : IFiscalizationCheckPrinter
    {
        public string Print(ReceiptDto receipt)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"КАССОВЫЙ ЧЕК / {GetFiscalizationType(receipt.Type)}");
            stringBuilder.AppendLine("ООО \"Насладдин\" ИНН 5048047172");
            stringBuilder.AppendLine($"Фискальный документ {receipt.FiscalNumber}");
            stringBuilder.AppendLine($"Дата выдачи {receipt.DateTime:yyyy-MM-dd HH-mm-ss}");
            stringBuilder.AppendLine($"РН ККТ {receipt.RegNumber}");
            stringBuilder.AppendLine($"ФН {receipt.FiscalNumber}");
            stringBuilder.AppendLine($"Заводской номер ККТ {receipt.DeviceNumber}");
            stringBuilder.AppendLine($"Фискальный признак {receipt.FiscalSign}");
            stringBuilder.AppendLine($"ОФД {receipt.Ofd}");
            stringBuilder.AppendLine($"Номер смены {receipt.SessionNumber}");
            stringBuilder.AppendLine($"Номер чека в смене {receipt.DocumentNumber}");
            stringBuilder.AppendLine("Место осуществления расчета https://nasladdin.ru");
            stringBuilder.AppendLine("Сайт ФНС www.nalog.ru");

            foreach (var item in receipt.Receipt.Items)
                stringBuilder.AppendLine($"{item.Label} {item.Price}");

            stringBuilder.AppendLine($"ИТОГ {receipt.Amount}");
            stringBuilder.AppendLine("   в т.ч. НДС0% 0");
            stringBuilder.AppendLine($"Безналичными {receipt.Amount}");
            // уточнить систему налогообложения
            stringBuilder.AppendLine("Система налогообложения ОСН");

            return stringBuilder.ToString();
        }

        private string GetFiscalizationType(string typeFromCloudCashier)
        {
            switch (typeFromCloudCashier)
            {
                case "Income":
                    return "Приход";
                case "IncomeReturn":
                    return "Возврат прихода";
                case "Expense":
                    return "Расход";
                case "ExpenseReturn":
                    return "Возврат расхода";
                default: return string.Empty; 
            }
        }
    }
}
