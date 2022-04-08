namespace NasladdinPlace.TestUtils.Extensions
{
    public static class ReflectionExtensions
    {
        public static void SetProperty<TSource, TProperty>(
            this TSource source,
            string propertyName,
            TProperty value)
        {
            var type = source.GetType();
            var property = type.GetProperty(propertyName);

            property.SetValue(source, value);
        }
    }
}
