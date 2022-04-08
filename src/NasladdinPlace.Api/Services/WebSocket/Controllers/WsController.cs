using System;
using System.Threading.Tasks;
using NasladdinPlace.Core;

namespace NasladdinPlace.Api.Services.WebSocket.Controllers
{
    public abstract class WsController
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        protected WsController()
        {
        }

        protected WsController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            _unitOfWorkFactory = unitOfWorkFactory;
        }

        protected async Task ExecuteAsync(Func<IUnitOfWork, Task> func)
        {
            using (var resolve = _unitOfWorkFactory.MakeUnitOfWork())
            {
                await func(resolve);
            }
        }
    }
}