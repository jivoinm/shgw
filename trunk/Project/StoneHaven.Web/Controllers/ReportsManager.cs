using System;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using NHibernate.Criterion;
using Query;
using Rhino.Commons;
using StoneHaven.Models;
using StoneHaven.Models.Filters;
using StoneHaven.Models.Services;
using StoneHaven.Models.Services.DB;

namespace StoneHaven.Web.Controllers
{
    public enum ReportTypeEnum
    {
        RegionalSaleReport,
        ColorGroupSaleReport,
        CustomerSaleReport
    }
    public class ReportsManagementController : BaseSecuredController
    {
        public void Index()
        {
            PropertyBag["reportTypes"] = typeof (ReportTypeEnum);
            RenderView("Index");
        }

        public void DoReport(ReportTypeEnum reportType,DateTime start,DateTime end)
        {
            CancelLayout();
            IList<SalesReportModel> models = null;
            switch (reportType)
            {
                case ReportTypeEnum.RegionalSaleReport:
                    models = IoC.Resolve<IReportServices>().RegionalSaleReport(new DateRange(start, end));
                    break;
                case ReportTypeEnum.ColorGroupSaleReport:
                    models = IoC.Resolve<IReportServices>().ColorGroupSaleReport(new DateRange(start, end));
                    break;
                case ReportTypeEnum.CustomerSaleReport:
                    models = IoC.Resolve<IReportServices>().CompanySaleReport(new DateRange(start, end));
                    break;
            }
            
            PropertyBag["models"] = models;
            RenderView("Report");
        }
    }
}