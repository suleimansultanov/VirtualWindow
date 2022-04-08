namespace NasladdinPlace.CloudPaymentsCore
{
    public class ServiceInfo
    {
        
        public string Id { get; }
        public string Secret { get; }

        public ServiceInfo(string id, string secret)
        {
            Id = id;
            Secret = secret;
        }
    }
}