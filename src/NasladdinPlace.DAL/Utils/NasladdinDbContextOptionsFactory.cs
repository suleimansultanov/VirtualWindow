using Microsoft.EntityFrameworkCore;

namespace NasladdinPlace.DAL.Utils
{
    public static class NasladdinDbContextOptionsFactory
    {
        private const int CommandTimeoutInSeconds = 60;

        public static void Setup(this DbContextOptionsBuilder builder,
            string connectionString,
            string migrationAssembly = null)
        {
            if (migrationAssembly == null)
            {
                builder.UseSqlServer(connectionString, options =>
                {
                    options.CommandTimeout(CommandTimeoutInSeconds);
                });
            }
            else
            {
                builder.UseSqlServer(connectionString,
                    options =>
                    {
                        options.MigrationsAssembly(migrationAssembly);
                        options.CommandTimeout(CommandTimeoutInSeconds);
                    });
            }

            builder.UseOpenIddict<int>();
        }
    }
}