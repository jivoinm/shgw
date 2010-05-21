using System.Collections;
using Rhino.Commons;
using StoneHaven.Models.Filters;
using StoneHaven.Models.Services;

namespace StoneHaven.Models.Tests
{
    public class ReportsTests : QuotationHelper
    {
        public void TestRegionalSaleReport()
        {
            var filter = new ReportFilter {CityId = 1};

            var reports = (IList) IoC.Resolve<IReportServices>().RegionalSaleReport(new DateRange());

        }
    }
}