namespace NasladdinPlace.Api.Dtos.OneCSync
{
    public class OneCSyncResult<T> where T : class
    {
        public RequestSyncParam RequestParam { get; }  
        public bool Success { get; }
        public T Data { get; }

        public static readonly OneCSyncResult<T> Empty = new OneCSyncResult<T>(null, false, null);
        public static OneCSyncResult<T> ForPurchases(RequestSyncParam requestSyncParams, T data)
        {
            return new OneCSyncResult<T>(requestSyncParams, true, data);
        }

        public static OneCSyncResult<T> ForInventoryBalances(T data)
        {
            return new OneCSyncResult<T>(null, true, data);
        }

        public static OneCSyncResult<T> ForGoodsMoving(RequestSyncParam requestSyncParams, T data)
        {
            return new OneCSyncResult<T>(requestSyncParams, true, data);
        }

        private OneCSyncResult (RequestSyncParam requestSyncParams, bool success, T data)
        {
            RequestParam = requestSyncParams;
            Success = success;
            Data = data;
        }
    }
}