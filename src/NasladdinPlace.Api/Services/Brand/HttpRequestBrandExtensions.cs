using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace NasladdinPlace.Api.Services.Brand
{
    public static class HttpRequestBrandExtensions
    {
        private const string BrandHeader = "Brand";
        
        public static Core.Enums.Brand GetBrandHeaderValue(this HttpRequest request)
        {
            var brandHeaderValue = request.Headers[BrandHeader].FirstOrDefault();

            return Enum.TryParse(typeof(Core.Enums.Brand), brandHeaderValue, out var brand)
                ? (Core.Enums.Brand) brand
                : Core.Enums.Brand.Invalid;
        }
    }
}