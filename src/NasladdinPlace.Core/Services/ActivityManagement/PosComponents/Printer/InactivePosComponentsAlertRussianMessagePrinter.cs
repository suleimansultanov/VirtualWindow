using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Models;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Printer.Contracts;
using NasladdinPlace.Core.Utils;
using NasladdinPlace.Utilities.TimeSpanConverter;

namespace NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Printer
{
    public class InactivePosComponentsAlertRussianMessagePrinter : IInactivePosComponentsAlertMessagePrinter
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public InactivePosComponentsAlertRussianMessagePrinter(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<string> PrintAsync(IEnumerable<PosComponentsInactivityInfo> inactivePosComponentsWithInactiveTime)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var messageBuilder = new StringBuilder();
                var index = 0;

                foreach (var groupInactivePosComponents in inactivePosComponentsWithInactiveTime.GroupBy(pci => pci.Key))
                {
                    var inactivePos = await unitOfWork.PointsOfSale.GetByIdAsync(groupInactivePosComponents.Key);

                    if (!inactivePos.AreNotificationsEnabled)
                        continue;

                    messageBuilder.AppendLine($"{++index}. Витрина {inactivePos.AbbreviatedName}:");

                    foreach (var groupInactivePosComponent in groupInactivePosComponents)
                    {
                        switch (groupInactivePosComponent.Type)
                        {
                            case Enums.PosComponentType.Display:
                                AppendMessageForComponent(messageBuilder, $"Экран {Emoji.Tv}",
                                    groupInactivePosComponent);
                                break;
                            case Enums.PosComponentType.Pos:
                                AppendMessageForComponent(messageBuilder, $"Сервис {Emoji.Electric_Plug}",
                                    groupInactivePosComponent);
                                break;
                            case Enums.PosComponentType.Battery:
                                messageBuilder.AppendLine($"Аккумулятор {Emoji.Battery} разряжается. Уровень заряда - {groupInactivePosComponent.BatteryLevel}%.");
                                break;
                        }
                    }
                }

                if (messageBuilder.Length > 0)
                {
                    messageBuilder.Insert(0,
                        $"{Emoji.Satellite} Нет связи:{Environment.NewLine}");                  
                }

                return messageBuilder.ToString();
            }
        }

        private static void AppendMessageForComponent(StringBuilder messageBuilder, string messageHeader,
            PosComponentsInactivityInfo inactivityInfo)
        {
            var inactivityPeriod = $"{TimeSpanToStringConverter.ToReadableString(inactivityInfo.InactivityPeriod)}";
            var lastActivityMessage =
                DateTimeUtils.GetLastResponseTimeMessage(inactivityInfo.LastActivityTime);
            messageBuilder.AppendLine($"{messageHeader} - {inactivityPeriod} с {lastActivityMessage}");
        }
    }
}

