using System;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework;
using Query;
using Rhino.Commons;
using StoneHaven.Models;
using StoneHaven.Models.Filters;
using StoneHaven.Models.Services;
using StoneHaven.Web.Forms;

namespace StoneHaven.Web.Controllers
{
    public class JobManagementController : BaseSecuredController
    {
        
        public void Index()
        {
            PrepareJobSearch();
        }

        private void PrepareJobSearch()
        {
            PropertyBag["jobStatuses"] = typeof(JobStatus);
            PropertyBag["companies"] = IoC.Resolve<IEntityServices>().FindAll<Company>();
        }

        public void AdvancedSearch()
        {
            PrepareJobSearch();
        }
        

        [AjaxAction]
        public JSONPageResult List([DataBind("filter")]JobSearchFilter filter, string sortname, string sortorder, int page, int rp)
        {
            filter.OrderBy = sortname;
            filter.Asc = sortorder == "asc";

            var jobs = IoC.Resolve<IEntityServices>().FindAll<Job>(filter, page > 0 ? page - 1 : 0, rp);
            var jsonPageResult = new JSONPageResult { page = 1, total = jobs.TotalItems };
            foreach (Job job in jobs)
            {
                jsonPageResult.rows.Add(new Row
                {
                    cell = new string[]
                                                           {
                                                               string.Format("{0}",RenderLinkToAction("Edit","EditJob","jobId="+job.Id))+
                                                               string.Format("{0}",RenderLinkToAction("Delete","DeleteJob","jobId="+job.Id)),
                                                               job.JobNr.ToString(),
                                                               job.Quotation.Customer.CustomerName,
                                                               job.Status.ToString(),
                                                               job.CreateDate.HasValue?job.CreateDate.Value.ToString("d"):"",
                                                               job.TemplateDate.HasValue?job.TemplateDate.Value.ToString("d"):"",
                                                               job.InstallationDate.HasValue?job.InstallationDate.Value.ToString("d"):"",
                                                               job.ServiceDate.HasValue?job.ServiceDate.Value.ToString("d"):"",
                                                          },
                    id = job.Id.ToString()
                });
            }

            return jsonPageResult;
        }

        public void CreateNewJobFromQuote(int quoteId)
        {
            if(quoteId>0)
            {
                var job = IoC.Resolve<IJobServices>().CreateJob(quoteId);
                if(job!=null)
                {
                    RedirectToAction("editJob", "jobId=" + job.Id);
                    return;
                }
            }
            PropertyBag["error"] = "Error creating the job";
            RedirectToAction("index");
        }
        
        public void SaveJob([DataBind("job", Validate = true)]Job job)
        {
            if (HasValidationError(job))
            {
                SetErrorMessages(GetErrorSummary(job));
                RenderView("_form");
                return;
            }
            
            //job.JobNotes = IoC.Resolve<IEntityServices>().FindAll<Note>(Where.Note.Job.Id == job.Id);
            IoC.Resolve<IJobServices>().SaveJob(job);
            UnitOfWork.CurrentSession.Flush();
            job = IoC.Resolve<IEntityServices>().GetInstance<Job>(job.Id);
            PropertyBag["job"] = job;
            PrepareJobPage();
            CancelLayout();
            RenderView("_form");
        }

        public void EditJob(int jobId)
        {
            Job job = null;
            if(jobId>0)
                job = IoC.Resolve<IEntityServices>().GetInstance<Job>(jobId);

            PropertyBag["job"] = job;
            PrepareJobPage();
            RenderView("jobForm");
        }

        public void AddDeposit(int jobId)
        {
            CancelLayout();
            if(jobId>0)
            {
                var job = IoC.Resolve<IEntityServices>().GetInstance<Job>(jobId);
                var deposit = new Deposit{Job = job,Amount = job.Balance};

                IoC.Resolve<IEntityServices>().SaveEntity(deposit);
                job.Deposits.Add(deposit);
                UnitOfWork.CurrentSession.Flush();
                job = IoC.Resolve<IEntityServices>().GetInstance<Job>(jobId);
                PropertyBag["deposits"] = job.Deposits;
                PrepareJobPage();
                PropertyBag["job"] = job;

            }
            RenderView("_deposits");
        }

        private void PrepareJobPage()
        {
            PropertyBag["paymentTypes"] = typeof(PaymentMethod);
            PropertyBag["jobStatusTypes"] = typeof(JobStatus);
            PropertyBag["jobtype"] = typeof(Job);
        }

        public void DeleteDeposit(int depositId)
        {
            CancelLayout();
            if(depositId>0)
            {
                var deposit = IoC.Resolve<IEntityServices>().GetInstance<Deposit>(depositId);
                IoC.Resolve<IEntityServices>().Delete(deposit);
                var job = IoC.Resolve<IEntityServices>().GetInstance<Job>(deposit.Job.Id);
                UnitOfWork.CurrentSession.Flush();
                PropertyBag["deposits"] = job.Deposits;
                PrepareJobPage();
                PropertyBag["job"] = job;
            }
            RenderView("_deposits");
        }

        public void DeleteJob(int jobId)
        {
            if (jobId > 0)
            {
                var job = IoC.Resolve<IEntityServices>().GetInstance<Job>(jobId);
                IoC.Resolve<IEntityServices>().Delete(job);
                UnitOfWork.CurrentSession.Flush();
                SetMessage("Job was successully deleted.");
            }

            RedirectToAction("index");
        }


        public void SaveNote([ARDataBind("note",Validate = true,AutoLoad = AutoLoadBehavior.NewInstanceIfInvalidKey)]Note note)
        {
            CancelLayout();
            if(HasValidationError(note))
            {
                SetErrorMessage("Invalid Note!");
                RedirectToReferrer();
                return;
            }
            IoC.Resolve<IEntityServices>().SaveEntity(note);
            UnitOfWork.CurrentSession.Flush();
            PropertyBag["job"] = note.Job;
            RenderView("_notes");
        }

        public void DeleteNote(int noteId)
        {
            CancelLayout();
            if (noteId > 0)
            {
                var note = IoC.Resolve<IEntityServices>().GetInstance<Note>(noteId);
                IoC.Resolve<IEntityServices>().Delete(note);
                var job = IoC.Resolve<IEntityServices>().GetInstance<Job>(note.Job.Id);
                UnitOfWork.CurrentSession.Flush();
                PropertyBag["job"] = job;
            }
           RenderView("_notes");
        }

        [Layout("print")]
        public void PrintPreview(int jobId,bool withPrice)
        {
            
            if (jobId > 0)
            {
                PropertyBag["withPrice"] = withPrice;
                PropertyBag["job"] = IoC.Resolve<IEntityServices>().GetInstance<Job>(jobId);
                PrepareJobPage();
                RenderView("print/_printPreview");
                return;
            }

            SetErrorMessage("Invalid Job");
            RedirectToReferrer();
        }
    }
}