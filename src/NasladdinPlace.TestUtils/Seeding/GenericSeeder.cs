using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.TestUtils.Seeding.Contracts;

namespace NasladdinPlace.TestUtils.Seeding
{
    public class GenericSeeder : ISeeder
    {
        private readonly DbContext _context;

        public GenericSeeder(DbContext context)
        {
            _context = context;
        }
        
        public IEnumerable<T> Seed<T>(IEnumerable<T> seeds) where T: class
        {
            var seedsCollection = seeds.ToImmutableList();
            _context.Set<T>().AddRange(seedsCollection);
            _context.SaveChanges();
            return seedsCollection;
        }
    }
}