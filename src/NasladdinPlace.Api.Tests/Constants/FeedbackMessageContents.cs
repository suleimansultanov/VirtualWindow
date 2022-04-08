using System.Collections.Generic;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Tests.Constants
{
    public static class FeedbackRussianMessageContents
    {
        public static readonly Dictionary<string, string> AdminPageLinkFakeFormat = new Dictionary<string, string>
        {
            {"UsersListPage",  "https://admin/usersList/{0}"},
            {"PosDetailsPage",  "https://admin/posDetails/{0}"}
        };


        public const string FeedbackContentTemplate = "*" + Emoji.Loudspeaker + " {0}* \r\n\r\n";

        public const string FeedbackAuthorizedUserInfoTemplate = UserFullNameLabel + "[{0}/{1}]({2})\r\n";

        public const string FeedbackPhoneNumberTemplate = UserPhoneNumberLabel + "+{0}\r\n";

        public const string FeedbackPosEmptyTemplate = PosNameLabel + "{0}\r\n";
        public const string FeedbackPosTemplate = PosNameLabel + "[{0}]({1})\r\n";

        public const string FeedbackLastPurchaseEmptyTemplate = LastPurchaseLabel + " {0}\r\n";
        public const string FeedbackLastPurchaseTemplate = 
                                                LastPurchaseLabel + "\r\n" +
                                                "[1. {0} 1 pc. {1}\r\n" +
                                                "Total: {1}\r\n" +
                                                "]({2}{3})\r\n"; 

        public const string FeedbackPurchaseDateTemplate = PurchaseDateLabel + "{0}\r\n";
        public const string FeedbackUserBalanceTemplate = UserBalanceLabel + "{0}\r\n";
        public const string FeedbackDateCreatedTemplate = DateCreatedLabel + "{0}\r\n";

        public const string FeedbackDeviceInfoTemplate = DeviceNameAndOsLabel + "{0}, {1}\r\n" +
                                                         AppVersionLabel + "{2}\r\n";

        private const string UserFullNameLabel = "*User Name/ID:* ";
        private const string UserPhoneNumberLabel = "*Phone:* ";
        private const string PosNameLabel = "*POS:* ";
        private const string DateCreatedLabel = "*Date:* ";
        private const string PurchaseDateLabel = "*Purchase date:* ";
        private const string UserBalanceLabel = "*User balance:* ";
        private const string DeviceNameAndOsLabel = "*Phone model, OS:* ";
        private const string AppVersionLabel = "*App version:* ";
        private const string LastPurchaseLabel = "*Last purchase:*";

        public const string NoInfoLabel = "No info";
        public const string NoPurchasesYetLabel = "No purchases yet";

        public const string UserFullName = "Тестовая Маша";
        public const string UserPhoneNumber = "79262103058";
        public const string PosName = "Витрина 2";
        public const string DeviceName = "Huawei";
        public const string DeviceOperatingSystem = "Android";
        public const string AppVersion = "1.0";
        public const string GoodName = "Огурцы";
        public const string CurrencyName = "руб";
        public const string FeedbackContent =
          "Добрый день! " +
          "Давным-давно в далекой Галактике... Старая Республика пала. " +
          "На ее руинах Орден сихтов создал галактическую Империю, подчиняющую одну за другой планетные системы. " +
          "Силы Альянса стремятся свергнуть Темного Императора и восстановить свободное правление в Галактике.";

    }
}
