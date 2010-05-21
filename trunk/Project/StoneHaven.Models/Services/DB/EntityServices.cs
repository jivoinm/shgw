using System;
using System.Collections;
using System.Collections.Generic;
using Castle.Components.Pagination;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Query;
using Rhino.Commons;
using StoneHaven.Models.Filters;

namespace StoneHaven.Models.Services.DB
{
    public class EntityServices : IEntityServices
    {
        public T SaveEntity<T>(T instance)
        {
            return Repository<T>.SaveOrUpdate(instance);
        }

        public T GetInstance<T>(int id)
        {
            return Repository<T>.Get(id);
        }

        public IList<T> FindAll<T>()
        {
            return (IList<T>) Repository<T>.FindAll();
        }

        public IList<T> FindAll<T>(params Order[] orderBy)
        {
            return (IList<T>) Repository<T>.FindAll(orderBy);
        }

        public IPaginatedPage<T> FindAll<T>(AbstractSearchFilter searchFilter, int firstResult, int maxResults,params Order[] orders)
        {
            var dc = DetachedCriteria.For(typeof (T));
            searchFilter.Apply(dc);

            if (searchFilter.OrderBy != null && searchFilter.OrderBy!="undefined")
                dc.AddOrder(new Order(searchFilter.OrderBy, searchFilter.Asc));
            
            IPaginatedPage<T> page = null;
            firstResult = firstResult > 0 ? firstResult*maxResults : firstResult;
            new CriteriaBatch(UnitOfWork.CurrentSession).Add(dc).Paging(firstResult, maxResults)
                .OnRead(delegate(ICollection<T> res, int count)
                            {
                                page = new GenericCustomPage<T>(res, firstResult, maxResults, count);
                            })
                .Execute();
            return page;
        }

        public IList<T> FindAll<T>(DetachedCriteria queryBuilder, OrderByClause @param)
        {
            return (IList<T>) Repository<T>.FindAll(queryBuilder,@param);
        }

        public IList<T> FindAll<T>(DetachedCriteria queryBuilder)
        {
            return (IList<T>) Repository<T>.FindAll(queryBuilder);
        }

        public void Flush()
        {
            UnitOfWork.CurrentSession.Flush();
        }

        public void Delete<T>(T item)
        {
            Repository<T>.Delete(item);
        }

        public IList FindPricesBy(Type type,PriceFilter filter)
        {
            var prices = UnitOfWork.CurrentSession.CreateCriteria(type)
                .CreateAlias("Company", "comp")
                .CreateAlias("Project", "prj")
                .CreateAlias("Entity", "entity")
                .CreateAlias("entity.Parent", "parent", JoinType.LeftOuterJoin)
                .Add(Restrictions.Eq("comp.Id", filter.CompanyId))
                .Add(Restrictions.Eq("prj.Id", filter.ProjectId))
                .AddOrder(Order.Asc("entity.Name"));
                
            return prices.List();
        }

        public IList FindPricesBy(Type type,PriceFilter filter,object entity)
        {
            var prices = UnitOfWork.CurrentSession.CreateCriteria(type)
                .CreateAlias("Company", "comp")
                .CreateAlias("Project", "prj")
                .CreateAlias("Entity", "entity")
                .CreateAlias("entity.Parent", "parent", JoinType.LeftOuterJoin)
                .Add(Restrictions.Eq("comp.Id", filter.CompanyId))
                .Add(Restrictions.Eq("prj.Id", filter.ProjectId))
                .Add(Restrictions.Eq("Entity", entity))
                .AddOrder(Order.Asc("entity.Name"));
                
            return prices.List();
        }
    }
}