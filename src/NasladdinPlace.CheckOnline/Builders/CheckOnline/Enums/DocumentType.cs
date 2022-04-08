namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums
{
    public enum DocumentType
    {
        /// <summary>
        ///     Тип документа: Приход
        /// </summary>
        Income = 0,

        /// <summary>
        ///     Тип документа: Расход
        /// </summary>
        Withdrawal = 1,

        /// <summary>
        ///     Тип документа: Вовзрат прихода
        /// </summary>
        IncomeRefund = 2,

        /// <summary>
        ///     Тип документа: Возврат расходя
        /// </summary>
        WithdrawalRefund = 3,

    }
}
