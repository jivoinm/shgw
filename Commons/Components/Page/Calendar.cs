using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Castle.MonoRail.Framework;
using Rhino.Commons;
using StoneHaven.Models;
using StoneHaven.Models.Services;

namespace Components.Page
{
    public enum GCalendarEnum
    {
        Month,Week,Day
    }

    [ViewComponentDetails("Calendar")]
    public class Calendar : ViewComponent
    {
        [ViewComponentParam]
        public GCalendarEnum Type { get; set; }
        
        [ViewComponentParam]
        public DateTime? Date { get; set; }

        public CalendarDay[,] calendarDays;
        public IList<CalendarEvent> events = new List<CalendarEvent>();
        public ICalendar CalendarService { get; set; }
        

        public override void Initialize()
        {
            if(CalendarService==null)
                throw new Exception("Setup the ICalendar service first.");
            Date = Date ?? DateTime.Now;
            
            if (Type == GCalendarEnum.Month)
                events = CalendarService.GetEventsFor(new DateTime(Date.Value.Year, Date.Value.Month, 1),
                                                      new DateTime(Date.Value.Year, Date.Value.Month, 1).AddMonths(1).AddSeconds(-1));
        }

        public override void Render()
        {

            IoC.Resolve<IViewSourceLoader>().AddAssemblySource(new AssemblySourceInfo("Components", "Components.Views"));
            var monthFirstDay = new DateTime(Date.Value.Year, Date.Value.Month, 1);
            var monthLastDay = monthFirstDay.AddMonths(1).AddDays(-1);
            var firstDay = (int) monthFirstDay.DayOfWeek;
            var daysInMonth = DateTime.DaysInMonth(Date.Value.Year, Date.Value.Month);
            var days = 0;
            var weeks = Math.Ceiling((double)(firstDay+daysInMonth)/7);
            calendarDays = new CalendarDay[(int)weeks,7];

            for (var i = 0; i < weeks; i++)
            {
                var isWeekStart = true;
                for (var j = 0; j < 7; j++)
                {
                    days++;
                    calendarDays[i, j] = new CalendarDay(days, isWeekStart,null); 
                    calendarDays[i, j].Day -= firstDay;
                    calendarDays[i, j].CalendarEvents = GetEvents(calendarDays[i, j].Day);

                    calendarDays[i, j].IsWeekEnd = j==6;
                    if ((calendarDays[i, j].Day < 1) || (calendarDays[i, j].Day > daysInMonth))
                    {
                        calendarDays[i,j].IsNonMonthday = true;
                    }
                    //calendarDays[i, j].Day++;
                    isWeekStart = false;
                }
            }
            PropertyBag["calendar"] = calendarDays;
            PropertyBag["calendarPeriod"] = string.Format("[{0}] - [{1}]", monthFirstDay.ToString("MMMM, dd yyyy"), monthLastDay.ToString("MMMM, dd yyyy"));
            PropertyBag["calCellPercent"] = weeks < 5 ?"25": weeks == 5? "19" : "15";
            RenderView("default");
        }

        private IList<CalendarEvent> GetEvents(int day)
        {
            var list = new List<CalendarEvent>();
            foreach (var calendarEvent in events)
            {
                if (calendarEvent.Range.Start.Day == day) 
                    list.Add(calendarEvent);
            }
            return list;
        }
        
    }

    public class CalendarDay
    {
        public int Day { get; set; }
        public bool IsWeekStart { get; set; }
        public bool IsWeekEnd { get; set; }
        public bool IsNonMonthday { get; set; }
        public IList<CalendarEvent> CalendarEvents { get; set; }

        public CalendarDay(int day,bool isWeekStart, IList<CalendarEvent> calendarEvents)
        {
            Day = day;
            CalendarEvents = calendarEvents;
            IsWeekStart = isWeekStart;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (CalendarDay)) return false;
            return Equals((CalendarDay) obj);
        }

        public bool Equals(CalendarDay obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.Day == Day && Equals(obj.CalendarEvents, CalendarEvents);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Day*397) ^ (CalendarEvents != null ? CalendarEvents.GetHashCode() : 0);
            }
        }
    }

    public class CalendarEvent
    {
        public DateRange Range { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Color { get; set; }

    }

    public interface ICalendar
    {
        IList<CalendarEvent> GetEventsFor(DateTime start, DateTime end);
    }

    public class SHGWCalendar : ICalendar
    {
        public IList<CalendarEvent> GetEventsFor(DateTime start, DateTime end)
        {
            var templates = IoC.Resolve<IJobServices>().FindTemplateJobs(start, end);
            var installations = IoC.Resolve<IJobServices>().FindInstallationJobs(start, end);
            var services = IoC.Resolve<IJobServices>().FindServiceJobs(start, end);
            var events = new List<CalendarEvent>();
            AddTemplateJobs(templates, events);
            AddInstallJobs(installations, events);
            AddServiceJobs(services, events);

            return events;
        }

        private static void AddTemplateJobs(IEnumerable<Job> templates, ICollection<CalendarEvent> events)
        {
            foreach (var job in templates)
            {
                events.Add(new CalendarEvent
                               {
                                   Color = "Green",
                                   Description = string.Format("#{0} {1} {2}", job.JobNr, job.Quotation.Customer.CustomerName, job.Quotation.Company.Name),
                                   Title = job.Status.ToString(),
                                   Range = new DateRange(job.TemplateDate.Value, job.TemplateDate.Value.AddHours(1))
                               });
            }
        }

        private static void AddInstallJobs(IEnumerable<Job> templates, ICollection<CalendarEvent> events)
        {
            foreach (var job in templates)
            {
                events.Add(new CalendarEvent
                               {
                                   Color = "Red",
                                   Description = string.Format("#{0} {1} {2}",job.JobNr,job.Quotation.Company.Name,job.Quotation.Customer.CustomerName),
                                   Title = job.Status.ToString(),
                                   Range = new DateRange(job.InstallationDate.Value, job.InstallationDate.Value.AddHours(1))
                               });
            }
        }

        private static void AddServiceJobs(IEnumerable<Job> templates, ICollection<CalendarEvent> events)
        {
            foreach (var job in templates)
            {
                events.Add(new CalendarEvent
                               {
                                   Color = "Yellow;color:black;",
                                   Description = string.Format("#{0} {1} {2}",job.JobNr,job.Quotation.Company.Name,job.Quotation.Customer.CustomerName),
                                   Title = job.Status.ToString(),
                                   Range = new DateRange(job.ServiceDate.Value, job.ServiceDate.Value.AddHours(1))
                               });
            }
        }
    }
}