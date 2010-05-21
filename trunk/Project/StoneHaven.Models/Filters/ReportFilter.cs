using NHibernate.Criterion;

namespace StoneHaven.Models.Filters
{
    public class ReportFilter : AbstractSearchFilter
    {
        public ReportFilter()
        {
            actions.Add(dc =>
                            {
                                dc.CreateCriteria("Quotation", "quote");
                                dc.CreateCriteria("quote.Customer", "cust");
                                dc.CreateCriteria("cust.City", "city");
                            });
        }

        public int ColorGroupId { get; set; }
        private int cityId;
        public int CityId
        {
            get { return cityId; }
            set
            {
                cityId = value;
                if(cityId==0) return;
                actions.Add(dc => dc.Add(Restrictions.Eq("city.Id", cityId)));
            }
        }

        public int Company { get; set; }
    }
}