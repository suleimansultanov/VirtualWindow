using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.DAL.Contracts;
using NasladdinPlace.DAL.EntityConfigurations.Default;
using NasladdinPlace.DAL.Utils;
using NUnit.Framework;

namespace NasladdinPlace.DAL.Tests.SqlServer
{
    [TestFixture]
    [NonParallelizable]
    public class SqlServerTests
    {
        private const string ConnectionString = "Data Source=tcp:nasladdindbserver.database.windows.net,1433;Initial Catalog=NasladdinTestDb;User Id=Nasladdin;Password=Timelysoft_312;Max Pool Size=30;";
        private const int ConnectionsCount = 50;

        private IApplicationDbContextFactory _applicationDbContextFactory;

        [SetUp]
        public void SetUp()
        {
            var builder = new DbContextOptionsBuilder();

            builder.Setup(ConnectionString, "NasladdinPlace.DAL");
            builder.UseOpenIddict<int>();

            _applicationDbContextFactory =
                new ApplicationDbContextFactory(new EntityConfigurationsFactory(), builder.Options);
        }

        [Test]
        public void CreateSqlConnections_MaxCountConnectionsAreGiven_ShouldReturnZeroSqlExceptions()
        {
            var connectionLimitErrorsCount = 0;

            var openConnectionTasksCollection = new Collection<Task>();

            for (var i = 0; i < ConnectionsCount; i++)
            {
                openConnectionTasksCollection.Add(Task.Run(() =>
                {
                    using (var context = _applicationDbContextFactory.Create())
                    {
                        context.Database.ExecuteSqlCommand(@"WAITFOR DELAY '00:00:20'");
                    }
                }));
            }

            try
            {
                Task.WhenAll(openConnectionTasksCollection).Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Any(x => x is SqlException || x is InvalidOperationException))
                    Interlocked.Increment(ref connectionLimitErrorsCount);
            }

            connectionLimitErrorsCount.Should().Be(0);
        }
    }
}
