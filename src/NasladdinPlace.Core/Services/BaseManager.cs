using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services
{
    public class BaseManager
    {
        public readonly IUnitOfWorkFactory UnitOfWorkFactory;

        public BaseManager(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            UnitOfWorkFactory = unitOfWorkFactory;
        }

        protected async Task<TResult> ExecuteAsync<TResult>(Func<IUnitOfWork, Task<TResult>> func)
        {
            using (var resolve = UnitOfWorkFactory.MakeUnitOfWork())
            {
                return await func(resolve);
            }
        }

        protected async Task ExecuteAsync(Func<IUnitOfWork, Task> func)
        {
            using (var resolve = UnitOfWorkFactory.MakeUnitOfWork())
            {
                await func(resolve);
            }
        }

    }
}
