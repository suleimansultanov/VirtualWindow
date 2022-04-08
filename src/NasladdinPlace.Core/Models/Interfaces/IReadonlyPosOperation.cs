namespace NasladdinPlace.Core.Models.Interfaces
{
    public interface IReadonlyPosOperation
    {
        int Id { get; }
        int PosId { get; }
        ApplicationUser User { get;}
    }
}
