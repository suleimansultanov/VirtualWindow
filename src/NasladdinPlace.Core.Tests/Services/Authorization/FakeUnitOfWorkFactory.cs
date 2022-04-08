using System.Collections.Concurrent;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.Models;

namespace NasladdinPlace.Core.Tests.Services.Authorization
{
    public class FakeUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ConcurrentDictionary<AccessGroup, Role> _roleByAccessGroupDictionary =
            new ConcurrentDictionary<AccessGroup, Role>
            {
                [AccessGroup.Admin] = Role.FromName("Role1"),
                [AccessGroup.Logistician] = Role.FromName("Role2")
            };
        
        public IUnitOfWork MakeUnitOfWork()
        {
            return new FakeUnitOfWork(_roleByAccessGroupDictionary);
        }
    }
}