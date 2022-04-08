namespace NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Constants
{
    public static class PosEvents
    {
        public const string SetAntennasOutputPower = "setAntennasOutputPower";
        public const string StartPosOperationInMode = "startOperationInMode";
        public const string CompleteOperation = "completeOperation";
        public const string ContinueOperation = "continueOperation";
        public const string RequestAccountingBalances = "requestAccountingBalances";
        public const string RequestAntennasOutputPower = "requestAntennasOutputPower";
        public const string RequestDoorsState = "requestDoorsState";
        public const string ConfirmCommandDelivery = "confirmCommandDelivery";
        public const string RequestLogs = "requestLogs";
    }
}