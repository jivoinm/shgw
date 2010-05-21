using System;
using System.Collections;
using System.Collections.Generic;
using Castle.ActiveRecord.Queries;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using NHibernate.Type;
using Rhino.Commons;
using StoneHaven.Models.Filters;

namespace StoneHaven.Models.Services.DB
{
    public class ReportServices : IReportServices
    {

        public IList<SalesReportModel> RegionalSaleReport(DateRange range)
        {
            var queryString = @"SELECT distinct city3_.Name, sum(this_.Total) as Total,((sum(this_.Total) / (SELECT sum(Total) FROM Jobs WHERE CreateDate BETWEEN :fromDate AND :toDate)) * 100) as Perc FROM Jobs this_ 
                inner join dbo.Quotations quote1_ on this_.QuoteId=quote1_.Id inner 
                join dbo.Customers cust2_ on quote1_.CustomerId=cust2_.Id inner 
                join dbo.Cities city3_ on cust2_.CityId=city3_.Id WHERE CreateDate BETWEEN :fromDate AND :toDate
                GROUP By city3_.Name ORDER BY city3_.Name";
            IQuery query = UnitOfWork.CurrentSession.CreateSQLQuery(queryString);
            query.SetParameter("fromDate",range.Start);
            query.SetParameter("toDate",range.End);
            query.SetResultTransformer(new AliasToBeanResultTransformer(typeof (SalesReportModel)));
            return query.List<SalesReportModel>();
        }

        public IList<SalesReportModel> ColorGroupSaleReport(DateRange range)
        {
            var queryString = @"select col.Name, (select sum(Total) from Jobs job inner join JobItems jobItems on jobItems.JobId=job.Id where jobItems.ColorGroupId=col.Id and job.CreateDate BETWEEN :fromDate AND :toDate) as Total,
                ((select sum(Total) from Jobs job inner join JobItems jobItems on jobItems.JobId=job.Id where jobItems.ColorGroupId=col.Id and job.CreateDate BETWEEN :fromDate AND :toDate)/(select sum(Total) from Jobs where CreateDate BETWEEN :fromDate AND :toDate) )* 100 as Perc
                from ColorGroups as col where parentId is not null
                ";
            IQuery query = UnitOfWork.CurrentSession.CreateSQLQuery(queryString);
            query.SetParameter("fromDate",range.Start);
            query.SetParameter("toDate",range.End);
            query.SetResultTransformer(new AliasToBeanResultTransformer(typeof (SalesReportModel)));
            return query.List<SalesReportModel>();
        }
        
        public IList<SalesReportModel> CompanySaleReport(DateRange range)
        {
            var queryString = @"SELECT distinct comp.Name, sum(this_.Total) as Total,
                ((sum(this_.Total) / (SELECT sum(Total) FROM Jobs WHERE CreateDate BETWEEN :fromDate AND :toDate)) * 100) as Perc FROM Jobs this_ 
                inner join dbo.Quotations quote1_ on this_.QuoteId=quote1_.Id 
                inner join dbo.Companies comp on quote1_.CompanyId=comp.Id 
                WHERE CreateDate BETWEEN :fromDate AND :toDate
                GROUP By comp.Name ORDER BY comp.Name
                ";
            IQuery query = UnitOfWork.CurrentSession.CreateSQLQuery(queryString);
            query.SetParameter("fromDate",range.Start);
            query.SetParameter("toDate",range.End);
            query.SetResultTransformer(new AliasToBeanResultTransformer(typeof (SalesReportModel)));
            return query.List<SalesReportModel>();
        }
    
    }

    public class SalesReportModel
    {

        public string Name { get; set; }
        private decimal total;

        public decimal Total
        {
            get
            {
                return total>0?Math.Round(total,2):total;
            }
            set { total = value; }
        }

        private decimal perc;
        public decimal Perc
        {
            get { return perc>0?Math.Round(perc,2):perc; }
            set { perc = value; }
        }
    }
}