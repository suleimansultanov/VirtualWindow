using System;
using System.Threading.Tasks;

namespace NasladdinPlace.CloudPaymentsCore
{
    public abstract class CloudPaymentsBaseService
    {
        protected async Task<Response<T>> PerformRequest<T>(Func<Task<Response<T>>> apiRequest) where T : class
        {
            try
            {
                return await apiRequest();
            }
            catch (Exception ex)
            {
                return Response<T>.Failure(ex.ToString());
            }
        }
    }
}
