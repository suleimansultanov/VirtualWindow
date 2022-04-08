using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.ViewModels.Schedules
{
    public class 
        ScheduleViewModel: BaseViewModel
    {
        [Display(Name = "Имя задачи")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        [Render(Control = RenderControl.TextArea)]
        public string Description
        {
            get
            {
                switch (Name)
                {
                    case "ping":
                        return "Оправка на витрины Keep Alive сообщения, поддерживая web socket соединение";
                    case "OngoingPurchaseActivityManager":
                        return "Проверка активности покупки пользователя (открыта витрина, клиент выбирает товар)";
                    case "UnpaidPurchasesMonitor":
                        return "Поиск неоплаченных корзин и попытка оплатить";
                    case "PromotionAgent_VerifyPhoneNumber":
                        return "Рассылка уведомлений для промо акции 'Подтверждение регистрации'";
                    case "PromotionAgent_VerifyPaymentCard":
                        return "Рассылка уведомлений для промо акции 'Привязка карты'";
                    case "PromotionAgent_FirstPay":
                        return "Рассылка уведомлений для промо акции 'Первая покупка'";
                    case "PosTemperatureAgent":
                        return "Уведомление в Telegram о ненормальной температуре внутри витрин";
                    case "CheckOnlineAgent":
                        return "Повторная отправка на фискализацию чеков со статусом 'ожидает фискализации'";
                    case "PosDiagnosticsAgent":
                        return "Отчет по ежедневной диагностике витрин";
                    case "PosComponentsActivityMonitor":
                        return "Уведомление в Telegram доступности элементов витрины";
                    case "PosLogsAgent":
                        return "Ежедневный запрос логов у витрин";
                    case "ConditionalPurchasesAgent":
                        return "Ежедневное завершение условных покупок";
                    case "PosNotificationsAgent":
                        return "Уведомление в Telegram при отключении оповещений витрин";
                    case "UntiedLabeledGoodsAgent":
                        return "Ежедневное уведомление в Telegram о непривязанных метках";
                    case "PosDisplayAgent_CommandsDelivery":
                        return "Повторная отправка команд на экран витрины";
                    case "Tactical Diagnostics":
                        return "Ежедневная тактическая диагностика";
                    case "AbnormalUserBonusPointsSeekingAgent":
                        return "Ежедневная проверка пользовательских бонусов";
                    case "ReportsAgent":
                        return "Ежедневная статистика и уведомление";
                    case "PointsOfSaleStateHistoricalDataDeletingAgent":
                        return "Ежедневное удаление устаревших данных о температуре витрин и положении дверей";
                    case "PointsOfSaleTokenUpdateAgent":
                        return "Обновление QR-кода на витринах";
                    case "UsersOldLogsDeletionAgent":
                        return "Удаление старых логов";
                    default: 
                        return "";
                }
            }
        }

        [Display(Name = "Дата следующего выполнения")]
        [Render(Control = RenderControl.DateTime)]
        public DateTime NextRun { get; set; }

        [Display(Name = "Задача доступна")]
        [Render(Control = RenderControl.YesNo)]
        public bool Enabled { get; set; }

        public override Type EntityType() => typeof(Entity);
    }
}
