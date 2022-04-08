using System;
using System.Collections.Generic;
using System.Web;

namespace NasladdinPlace.Core.Services.MessageSender.Sms.Builders
{
    public class SmsRequestMessageBuilder
    {
        private const string ApiIdQueryName = "api_id";
        private const string FromQueryName = "from";
        private const string ToQueryName = "to";
        private const string MessageQueryName = "msg";
        private const string JsonResponseQueryName = "json";
        private const string TestQueryName = "test";
        private const string QueryValue = "1";

        private readonly string _requestUrl;
        private readonly Dictionary<string, string> _queryParameters;

        public SmsRequestMessageBuilder(string requestUrl)
        {
            if(string.IsNullOrEmpty(requestUrl))
                throw new ArgumentNullException(nameof(requestUrl));

            _requestUrl = requestUrl;
            _queryParameters = new Dictionary<string, string>();
        }

        public SmsRequestMessageBuilder From(string value)
        {
            AddQuery(FromQueryName, value);
            return this;
        }

        public SmsRequestMessageBuilder To(string value)
        {
            AddQuery(ToQueryName, value);
            return this;
        }

        public SmsRequestMessageBuilder Message(string message)
        {
            AddQuery(MessageQueryName, message);
            return this;
        }

        public SmsRequestMessageBuilder MarkJsonResponse()
        {
            AddQuery(JsonResponseQueryName, QueryValue);
            return this;
        }

        public SmsRequestMessageBuilder ApiId(string apiId)
        {
            AddQuery(ApiIdQueryName, apiId);
            return this;
        }

        public SmsRequestMessageBuilder MarkAsTestRequest()
        {
            AddQuery(TestQueryName, QueryValue);
            return this;
        }

        public string Build()
        {
            var collection = HttpUtility.ParseQueryString(string.Empty);

            foreach (var key in _queryParameters.Keys)
            {
                if (_queryParameters.TryGetValue(key, out var value))
                {
                    collection[key] = value;
                }
            }

            var builder = new UriBuilder(_requestUrl) {Query = collection.ToString()};

            return builder.ToString();
        }

        private void AddQuery(string key, string value)
        {
            if (!_queryParameters.ContainsKey(key))
                _queryParameters.Add(key, value);
        }
    }
}