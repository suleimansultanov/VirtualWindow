using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FirebaseCloudMessagingClient.Utils
{
    public class UriQueryUnescapingHandler : DelegatingHandler
    {   
        public UriQueryUnescapingHandler()
            : base(new HttpClientHandler()) { }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uri = request.RequestUri;
            var unescapedUri = Uri.UnescapeDataString(uri.ToString());
            request.RequestUri = new Uri(unescapedUri);
            return base.SendAsync(request, cancellationToken);
        }
    }
}