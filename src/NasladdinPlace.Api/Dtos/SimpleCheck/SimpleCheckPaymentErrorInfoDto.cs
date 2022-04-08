using System;

namespace NasladdinPlace.Api.Dtos.SimpleCheck
{
    public sealed class SimpleCheckPaymentErrorInfoDto
    {
        public string Message { get; set; }
        public DateTime NextPaymentAttemptDate { get; set; }
    }
}
