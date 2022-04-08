using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Utils;
using NasladdinPlace.DAL.Utils.EntityFilter;
using PagedList;

namespace NasladdinPlace.DAL.Repositories
{
    /// <summary>
    /// Универсальный репозиторий, для работы с любыми типами
    /// </summary>
    public class ReferenceRepository : IReferenceRepository
    {
        public ApplicationDbContext Context { get; }
        private bool _disposed;

        public ReferenceRepository(ApplicationDbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Получить все элементы
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<Entity> GetAll(Type type)
        {
            return GetQueryByEntityType(type).ToList();
        }

        /// <summary>
        /// Получить все элементы по динамическому фильтру
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filter"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public IQueryable<Entity> Get(Type type, List<BaseFilterItemModel> filter = null, PaginationInfo pagination = null)
        {
            var query = GetQueryByEntityType(type);

            var provider = new FilterProvider(type);
            if (filter != null && filter.Any())
            {
                var filterObject = provider.GetFilter(filter);
                if (filterObject.HasFilter)
                    query = query.Where(filterObject.Predicate, filterObject.Params);

                if (filterObject.HasSort)
                    query = query.OrderBy(filterObject.Sort);                
            }

            var includeProperties = GetIncludeProperties(type).ToList();
            if (includeProperties.Count > 0)
            {
                foreach (var property in includeProperties)
                {
                    query = query.Include(property);
                }
            }

            if (pagination != null)
                query = query.ToPagedQuery(pagination.Page, pagination.PageSize);

            return query;
        }

        private IQueryable<Entity> GetQueryByEntityType(Type type)
        {
            var dbSetMethodInfo = typeof(DbContext).GetMethod("Set");
            var query = (IQueryable<Entity>)dbSetMethodInfo.MakeGenericMethod(type).Invoke(Context, null);

            return query;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                Context.Dispose();

            _disposed = true;
        }

        ~ReferenceRepository()
        {
            Dispose(false);
        }

        /// <summary>
        /// Возвращает список названий свойств помеченных атрибутом <see cref="IncludeAttribute"/>
        /// </summary>
        /// <returns></returns>
        private static  IEnumerable<string> GetIncludeProperties(Type type)
        {
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<IncludeAttribute>();
                if (attr != null) yield return property.Name;
            }
        }
    }
}
