using System;
using NHibernate.Criterion;

namespace StoneHaven.Models.Filters
{
    public class JobSearchFilter : AbstractSearchFilter
    {
        private JobStatus? status;
        private long jobNr;
        private int companyId;
        private string customerName;
        private string customerPhone;
        private int projectId;
        private DateTime? scheduledStartDate;
        private DateTime? scheduledEndDate;

        public JobSearchFilter()
        {
            actions.Add(dc=>
                            {
                                dc.CreateCriteria("Quotation", "quote");
                                dc.CreateCriteria("quote.Customer", "cust");
                                dc.CreateCriteria("quote.Company", "comp");
                            });
        }

        public JobStatus? Status
        {
            get { return status; }
            set
            {
                status = value;
                if(!status.HasValue) return;
                actions.Add(dc => dc.Add(Restrictions.Eq("Status", status)));
            }
        }

        public long JobNr
        {
            get { return jobNr; }
            set
            {
                jobNr = value;
                if (jobNr == 0) return;
                actions.Add(dc => dc.Add(Restrictions.Like("JobNr",jobNr)));
            }
        }

        public int CompanyId
        {
            get { return companyId; }
            set
            {
                companyId = value;
                if(companyId==0)return;
                actions.Add(dc=> dc.Add(Restrictions.Eq("comp.Id", companyId)));
            }
        }

        public string CustomerName
        {
            get { return customerName; }
            set
            {
                customerName = value;
                if (customerName == null) return;
                actions.Add(dc => dc.Add(Restrictions.Like("cust.CustomerName", customerName, MatchMode.Anywhere)));
            }
        }

        public string CustomerPhone
        {
            get { return customerPhone; }
            set
            {
                customerPhone = value;
                if (customerPhone == null) return;
                actions.Add(dc => dc.Add(Restrictions.Like("cust.Phone", customerPhone, MatchMode.Anywhere)));
            }
        }

        public int ProjectId
        {
            get { return projectId; }
            set
            {
                projectId = value;
                if(projectId==0)return;
                actions.Add(dc=>
                                {
                                    dc.CreateCriteria("quote.QuoteItems", "items");
                                    dc.Add(Restrictions.In("items.Project.Id", new object[] {projectId}));
                                });
            }
        }

        public DateTime? ScheduledEndDate
        {
            get { return scheduledEndDate; }
            set
            {
                scheduledEndDate = value;
                if (!scheduledEndDate.HasValue) return;
                actions.Add(dc => 
                        dc.Add(
                            Restrictions.Or(Restrictions.Le("TemplateDate", scheduledEndDate),
                                Restrictions.Or(Restrictions.Le("InstallationDate", scheduledEndDate),
                                    Restrictions.Le("ServiceDate", scheduledEndDate)))
                        )
                    );
            }
        }

        public DateTime? ScheduledStartDate
        {
            get { return scheduledStartDate; }
            set
            {
                scheduledStartDate = value;
                if (!scheduledStartDate.HasValue) return;
                actions.Add(dc => 
                    dc.Add(
                            Restrictions.Or(Restrictions.Ge("TemplateDate", ScheduledStartDate),
                                Restrictions.Or(Restrictions.Ge("InstallationDate", ScheduledStartDate),
                                    Restrictions.Ge("ServiceDate", ScheduledStartDate)))
                        )
                    );

            }
        }
       
    }
}