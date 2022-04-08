using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.DAL.Utils.EntityFilter;
using PagedList;

namespace NasladdinPlace.DAL.Utils
{
    public static class DynamicLinqExt
    {
        /// <summary>
        /// Диначеская фильтрация, сортировка и пагинация
        /// </summary>
        public static IQueryable<TEntity> DynamicFilter<TEntity>(this IQueryable<TEntity> query,
            List<BaseFilterItemModel> filter, PaginationInfo pagination = null, bool sort = true)
        {
            var type = typeof(TEntity);
            var provider = new FilterProvider(type);

            if (filter != null && filter.Any())
                query = query.Filtration(filter, ref sort, provider);
            else
                sort = false;

            if (pagination != null)
                query = query.Pagination(pagination, sort, provider);

            return query;
        }

        /// <summary>
        /// Филтрация и сортировка
        /// </summary>
        public static IQueryable<TEntity> Filtration<TEntity>(this IQueryable<TEntity> query,
            List<BaseFilterItemModel> filter, ref bool sort, FilterProvider provider = null)
        {
            if (provider == null) provider = new FilterProvider(typeof(TEntity));

            var filterObject = provider.GetFilter(filter);
            if (filterObject.HasFilter)
                query = query.Where(filterObject.Predicate, filterObject.Params);

            if (sort)
            {
                sort = filterObject.HasSort;
                if (sort) query = query.OrderBy(filterObject.Sort);
            }

            return query;
        }

        /// <summary>
        /// Проверка условия и фильтрация
        /// </summary>
        public static IQueryable<TEntity> CheckAndFilter<TEntity>(this IQueryable<TEntity> query, bool checkCondition, Expression<Func<TEntity, bool>> filterExpression)
        {
            if (checkCondition)
                query = query.Where(filterExpression);
            return query;
        }

        /// <summary>
        /// Пагинация
        /// </summary>
        public static IQueryable<TEntity> Pagination<TEntity>(this IQueryable<TEntity> query, PaginationInfo pagination, bool ordered, FilterProvider provider = null)
        {
            return query.Pagination(pagination);
        }

        /// <summary>
        /// Пагинация
        /// </summary>
        public static IQueryable<TEntity> Pagination<TEntity>(this IQueryable<TEntity> query, PaginationInfo pagination)
        {
            return query.ToPagedQuery(pagination.Page, pagination.PageSize);
        }
        
        /// <summary>
        /// Сортировка
        /// </summary>
        public static IQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> query, string sortField, int sortType)
        {
            return query.OrderBy($"{sortField} {(sortType == 1 ? FilterProvider.ConstAsc : FilterProvider.ConstDesc)}");
        }

        public static void Remove<TSource>(this ICollection<TSource> source, Func<TSource, bool> predicate)
        {
            var remove = source.Where(predicate).ToList();
            remove.ForEach(x => source.Remove(x));
        }
    }
}