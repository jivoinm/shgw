using System;
using Castle.MonoRail.Framework;
using Rhino.Commons;
using StoneHaven.Models.Services;

namespace StoneHaven.Web.Controllers
{
    public class CalendarManagementController : BaseSecuredController
    {
        public void Index(DateTime? date)
        {
            PropertyBag["currentDate"] = date ?? DateTime.Now;
            RenderView("Index");
        }

        public void GoToDate(DateTime date)
        {
            CancelLayout();
            Index(date);
        }

        [Layout("simple")]
        public void RenderCalendarDay(DateTime date)
        {
            var start = new DateTime(date.Year, date.Month, date.Day);
            var end = start.AddDays(1).AddSeconds(-1);

            var templates = IoC.Resolve<IJobServices>().FindTemplateJobs(start,end);
            var installations = IoC.Resolve<IJobServices>().FindInstallationJobs(start,end);
            var services = IoC.Resolve<IJobServices>().FindServiceJobs(start,end);

            PropertyBag["templates"] = templates;
            PropertyBag["installations"] = installations;
            PropertyBag["services"] = services;
        }
    }
}