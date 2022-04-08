namespace NasladdinPlace.Core.Services.Pos.OperationsManager
{
    public enum OperationsManagerFailureType
    {
        LastPosOperationBelongsToOtherUserOrMode = 0,
        PosModeNotAllowed = 1,
        Undefined = 2,
        LastPosOperationPendingCompletion = 3
    }
}