using System.Collections;
using Rhino.Commons;
using StoneHaven.Models.Filters;
using StoneHaven.Models.Services.DB;
using System.Collections.Generic;

namespace StoneHaven.Models.Services
{
    public interface IReportServices
    {
        IList<SalesReportModel> RegionalSaleReport(DateRange range);
        IList<SalesReportModel> ColorGroupSaleReport(DateRange range);
        IList<SalesReportModel> CompanySaleReport(DateRange range);
    }
}