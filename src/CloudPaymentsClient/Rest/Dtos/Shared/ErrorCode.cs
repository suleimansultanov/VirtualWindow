namespace CloudPaymentsClient.Rest.Dtos.Shared
{
    public enum ErrorCode
    {
        ReferToCardIssuer = 5001,
        DoNotHonor = 5005,
        Error = 5006,
        InvalidTransaction = 5012,
        AmountError = 5013,
        FormatError = 5030,
        BankNotSupportedBySwitch = 5031,
        SuspectedFraud = 5034,
        LostCard = 5041,
        StolenCard = 5043,
        InsufficientFunds = 5051,
        ExpiredCard = 5054,
        TransactionNotPermitted = 5057,
        ExceedWithdrawFrequency = 5065,
        IncorrectCvv = 5082,
        Timeout = 5091,
        CannotReachNetwork = 5092,
        SystemError = 5096,
        UnableToProcess = 5204,
        AuthenticationFailed = 5206,
        AuthenticationUnavailable = 5207,
        AntiFraud = 5300
    }
}