namespace NasladdinPlace.Api.Extensions
{
    public static class StringExtensions
    {
        public static string ToHtml(this string text)
        {
            return CommonMark.CommonMarkConverter.Convert(text);
        }
    }
}