using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NasladdinPlace.DAL;
using NasladdinPlace.DAL.Contracts;
using NasladdinPlace.DAL.EntityConfigurations.Default;
using NasladdinPlace.DAL.Utils;

namespace NasladdinPlace.TestUtils
{
    public class TestApplicationDbContextFactory : IApplicationDbContextFactory
    {
        private readonly IApplicationDbContextFactory _applicationDbContextFactory;
        
        public TestApplicationDbContextFactory()
        { 
            var config = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json", reloadOnChange: true, optional: false)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");
            
            var builder = new DbContextOptionsBuilder();
            
            builder.Setup(connectionString, "NasladdinPlace.DAL");
            builder.UseOpenIddict<int>();

            _applicationDbContextFactory = 
                new ApplicationDbContextFactory(new EntityConfigurationsFactory(), builder.Options);
        }
        
        public ApplicationDbContext Create()
        {
           
            return _applicationDbContextFactory.Create();
        }
    }
}