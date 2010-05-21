using System;
using System.Collections.Generic;

namespace StoneHaven.Models.Services
{
    public interface IJobServices
    {
        Job CreateJob(int quoteId);
        void SaveJob(Job job);
        IList<Job> FindTemplateJobs(DateTime start, DateTime end);
        IList<Job> FindInstallationJobs(DateTime start, DateTime end);
        IList<Job> FindServiceJobs(DateTime start, DateTime end);
    }
}