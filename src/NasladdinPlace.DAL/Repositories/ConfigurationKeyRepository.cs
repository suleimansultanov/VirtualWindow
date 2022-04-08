using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.DAL.Contracts;

namespace NasladdinPlace.DAL.Repositories
{
    public class ConfigurationKeyRepository : IConfigurationKeyRepository
    {
        private readonly IApplicationDbContext _context;

        public ConfigurationKeyRepository(IApplicationDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            
            _context = context;
        }

        public IEnumerable<ConfigurationKey> GetAllIncludingValues()
        {
            return _context.ConfigurationKeys
                .Include(ck => ck.Values)
                .ToImmutableList();
        }

        public Task<ConfigurationKey> GetByIdIncludingValuesAsync(ConfigurationKeyIdentifier keyIdentifier)
        {
            return _context.ConfigurationKeys
                .Include(ck => ck.Values)
                .SingleOrDefaultAsync(ck => ck.Id == keyIdentifier);
        }

        public void Add(ConfigurationKey configurationKey)
        {
            if (configurationKey == null)
                throw new ArgumentNullException(nameof(configurationKey));
            
            _context.ConfigurationKeys.Add(configurationKey);
        }
    }
}