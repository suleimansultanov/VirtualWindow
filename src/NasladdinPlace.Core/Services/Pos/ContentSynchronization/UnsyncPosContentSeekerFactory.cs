namespace NasladdinPlace.Core.Services.Pos.ContentSynchronization
{
    public class UnsyncPosContentSeekerFactory
    {
        public virtual IUnsyncPosContentSeeker Create(IUnitOfWork unitOfWork)
        {
            return new UnsyncPosContentSeeker(unitOfWork);
        }
    }
}