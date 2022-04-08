using System;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.DAL;
using NasladdinPlace.DAL.Contracts;
using NasladdinPlace.DAL.EntityConfigurations.Default;
using NasladdinPlace.DAL.Utils;

namespace NasladdinPlace.Infra.IoC.Factories.DbContext
{
    public static class ApplicationDbContextFactoryFactory
    {
        public static IApplicationDbContextFactory Create(string dbConnectionString)
        {
            if (string.IsNullOrWhiteSpace(dbConnectionString))
                throw new ArgumentNullException(nameof(dbConnectionString));
            
            var options = new DbContextOptionsBuilder();
            options.Setup(dbConnectionString, "NasladdinPlace.DAL");
            options.UseOpenIddict<int>();
            return new ApplicationDbContextFactory(
                new EntityConfigurationsFactory(), 
                options.Options
            );
        }
    }
}