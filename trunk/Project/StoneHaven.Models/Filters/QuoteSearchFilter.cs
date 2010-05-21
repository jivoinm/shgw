using NHibernate.Criterion;

namespace StoneHaven.Models.Filters
{
    public class QuoteSearchFilter : AbstractSearchFilter
    {
        private long quoteNr;
        private string customerName;
        private string customerPhone;
        private int company;

        public long QuoteNr
        {
            get { return quoteNr; }
            set
            {
                quoteNr = value;
                actions.Add(dc =>
                                {
                                    if (quoteNr==0)
                                        return;

                                    dc.Add(Restrictions.Eq("QuoteNr",quoteNr));
                                });

            }
        }

        public string CustomerName
        {
            get { return customerName; }
            set
            {
                customerName = value;
                actions.Add((dc =>
                                 {
                                     if(customerName==null) return;
                                     dc.CreateCriteria("Customer","cust").Add(Restrictions.Like("CustomerName", customerName,MatchMode.Anywhere));
                                 }
                    
                            ));
            }
        }

        public string CustomerPhone
        {
            get { return customerPhone; }
            set
            {
                customerPhone = value;
                actions.Add((dc =>
                                 {
                                     if (customerPhone == null) return;
                                     dc.CreateCriteria("Customer", "cust").Add(Restrictions.Like("Phone", customerPhone, MatchMode.Anywhere));
                                 }
                            ));
                
            }
        }

        public int Company
        {
            get { return company; }
            set
            {
                company = value;
                actions.Add((dc =>
                                 {
                                     if(company==0) return;
                                     dc.CreateCriteria("Company", "comp").Add(Restrictions.Eq("Id", company));
                                 }));
            }
        }
    }
}