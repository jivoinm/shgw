using System;
using System.Collections;
using System.Collections.Generic;
using Castle.Components.Pagination;
using NHibernate.Criterion;
using Query;
using StoneHaven.Models.Filters;

namespace StoneHaven.Models.Services
{
    public interface IEntityServices
    {
        T SaveEntity<T>(T instance);
        T GetInstance<T>(int id);
        IList<T> FindAll<T>();
        IPaginatedPage<T> FindAll<T>(AbstractSearchFilter searchFilter, int firstResult, int maxResults, params Order[] orders);
        void Flush();

        IList<T> FindAll<T>(DetachedCriteria queryBuilder, OrderByClause param);
        IList<T> FindAll<T>(DetachedCriteria queryBuilder);
        void Delete<T>(T item);
        IList<T> FindAll<T>(params Order[] orderBy);
        IList FindPricesBy(Type type, PriceFilter filter);
        IList FindPricesBy(Type type, PriceFilter filter, object entity);
    }
}