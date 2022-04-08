using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;

namespace NasladdinPlace.DAL.Repositories
{
    public class MessengerContactRepository : Repository<MessengerContact>, IMessengerContactRepository
    {
        public MessengerContactRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}