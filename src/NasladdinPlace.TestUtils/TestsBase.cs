using Microsoft.EntityFrameworkCore;
using NasladdinPlace.DAL;
using NasladdinPlace.DAL.Contracts;
using NasladdinPlace.TestUtils.Cleaning;
using NasladdinPlace.TestUtils.Seeding;
using NasladdinPlace.TestUtils.Seeding.Contracts;
using NUnit.Framework;

namespace NasladdinPlace.TestUtils
{
    [TestFixture]
    [NonParallelizable]
    public abstract class TestsBase
    {
        private static readonly IApplicationDbContextFactory ApplicationDbContextFactory =
            new TestApplicationDbContextFactory();
        
        protected ApplicationDbContext Context { get; private set; }
        protected ISeeder Seeder { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
            Context = ProvideNewContext();
            Seeder = new GenericSeeder(Context);
            
            Context.Database.Migrate();
        }

        [TearDown]
        public virtual void TearDown()
        {
            Context.Dispose();
            using (var context = ProvideNewContext())
            {
                new SqlServerDbCleaner(context).Clean();
            }
        }

        protected ApplicationDbContext ProvideNewContext()
        {
            return ApplicationDbContextFactory.Create();
        }
    }
}