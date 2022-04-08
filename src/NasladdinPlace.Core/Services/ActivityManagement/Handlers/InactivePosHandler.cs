using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.ActivityManagement.Handlers
{
    public class InactivePosHandler
    {
        public InactivePosHandler()
        {
            
        }
        
        public Task Handle(int posId)
        {
            // do nothing

            return Task.CompletedTask;
        }
    }
}