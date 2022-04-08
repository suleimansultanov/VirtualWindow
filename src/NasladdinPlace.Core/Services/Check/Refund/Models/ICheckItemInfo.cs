namespace NasladdinPlace.Core.Services.Check.Refund.Models
{
    public interface ICheckItemInfo
    {
        int PosOperationId { get; }
        int? EditorId { get; }
    }
}
