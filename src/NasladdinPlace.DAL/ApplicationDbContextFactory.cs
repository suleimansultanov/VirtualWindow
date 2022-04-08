using Microsoft.EntityFrameworkCore;
using NasladdinPlace.DAL.Contracts;
using NasladdinPlace.DAL.EntityConfigurations.Contracts;

namespace NasladdinPlace.DAL
{
    public class ApplicationDbContextFactory : IApplicationDbContextFactory
    {
        private readonly IEntityConfigurations _entityConfigurations;
        private readonly DbContextOptions _options;

        public ApplicationDbContextFactory(
            IEntityConfigurationsFactory entityConfigurationsFactory,
            DbContextOptions options)
        {
            _entityConfigurations = entityConfigurationsFactory.MakeEntityConfigurations();
            _options = options;
        }

        public ApplicationDbContext Create()
        {
            return new ApplicationDbContext(_options, _entityConfigurations);
        }
    }
}