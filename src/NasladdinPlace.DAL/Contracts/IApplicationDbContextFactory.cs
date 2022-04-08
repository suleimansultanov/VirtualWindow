namespace NasladdinPlace.DAL.Contracts
{
    public interface IApplicationDbContextFactory
    {
        ApplicationDbContext Create();
    }
}