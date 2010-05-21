using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using NHibernate.Criterion;
using Query;
using Rhino.Commons;

namespace StoneHaven.Models.Services.DB
{
    public class JobServices : IJobServices
    {
        public Job CreateJob(int quoteId)
        {
            using (var tran = UnitOfWork.Current.BeginTransaction())
            {
                try
                {
                    var quote = IoC.Resolve<IEntityServices>().GetInstance<Quotation>(quoteId); 

                    var job = Repository<Job>.FindOne(Where.Job.Quotation == quote);
                    if (job==null)
                    {
                        job = new Job {Quotation = quote};
                        job.JobNr = job.JobNr == 0 ? CreateJobNumber() : job.JobNr;
                        job.CreateDate = DateTime.Now;
                        job.Total = quote.Total;
                        Repository<Job>.Save(job);
                        foreach (var quoteItem in quote.QuoteItems)
                        {
                            job.JobItems.Add(Repository<JobItem>.Save(
                                new JobItem
                                    {
                                        Job = job,
                                        FinalColorGroup = quoteItem.ColorGroup,
                                        FinalEdge = quoteItem.Edge
                                    }
                                ));
                        }
                
                    }
                    tran.Commit();
                    return job;
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    IoC.Resolve<ILogger>().Error("Error Creating a job", e);
                    return null;
                }
            }
        }

        public void SaveJob(Job job)
        {
            foreach (var deposit in job.Deposits)
            {
                deposit.Job = job;
                Repository<Deposit>.SaveOrUpdate(deposit);
            }
            foreach (var item in job.JobItems)
            {
                item.Job = job;
                Repository<JobItem>.SaveOrUpdate(item);
            }
            job.Quotation = Repository<Quotation>.Get(job.Quotation.Id);
            Repository<Job>.SaveOrUpdate(job);
        }

        private static long CreateJobNumber()
        {
            var maxNr = UnitOfWork.CurrentSession.CreateCriteria(typeof(Job)).SetProjection(Projections.Max("JobNr")).UniqueResult<long>();
            return maxNr + 1;
        }

        public IList<Job> FindTemplateJobs(DateTime start, DateTime end)
        {
            return (IList<Job>) Repository<Job>.FindAll(Where.Job.TemplateDate.Between(start, end));
        }

        public IList<Job> FindInstallationJobs(DateTime start, DateTime end)
        {
            return (IList<Job>)Repository<Job>.FindAll(Where.Job.InstallationDate.Between(start, end));
        }

        public IList<Job> FindServiceJobs(DateTime start, DateTime end)
        {
            return (IList<Job>)Repository<Job>.FindAll(Where.Job.ServiceDate.Between(start, end));
        }

    }
}
