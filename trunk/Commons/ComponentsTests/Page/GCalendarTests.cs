using System;
using System.Collections.Generic;
using Castle.MonoRail.TestSupport;
using Components.Page;
using NUnit.Framework;
using Rhino.Commons;
using Rhino.Mocks;

namespace ComponentsTests.Page
{
    [TestFixture]
    public class GCalendarTests : BaseViewComponentTest
    {
        private Calendar calendar;
        private MockRepository mocks;
        private ICalendar calendarServices;


        [SetUp]
        public void Init()
        {
            calendar = new Calendar();
            mocks = new MockRepository();
            calendarServices = mocks.StrictMock<ICalendar>();
            calendar.CalendarService = calendarServices;
        }

        [Test]
        public void RenderDecember2008CalendarMatrix()
        {
            calendar.Type = GCalendarEnum.Month;
            calendar.Date = DateTime.Today;
            PrepareViewComponent(calendar);
            calendar.Render();
          
            Assert.IsNotNull(calendar.calendarDays);
            Assert.AreEqual(0,calendar.calendarDays[0,0]);
            Assert.AreEqual(1,calendar.calendarDays[0,1]);
            Assert.AreEqual(2,calendar.calendarDays[0,2]);
            Assert.AreEqual(3,calendar.calendarDays[0,3]);
            Assert.AreEqual(4,calendar.calendarDays[0,4]);
            Assert.AreEqual(5,calendar.calendarDays[0,5]);
            Assert.AreEqual(6,calendar.calendarDays[0,6]);
            Assert.AreEqual(7,calendar.calendarDays[1,0]);
            Assert.AreEqual(8,calendar.calendarDays[1,1]);
            Assert.AreEqual(31,calendar.calendarDays[4,3]);
            Assert.AreEqual(0,calendar.calendarDays[4,4]);
            Assert.AreEqual(0,calendar.calendarDays[4,6]);
        }

        [Test]
        public void RenderDecember2008CalendarWithEventOnFirstAndDay15()
        {
            var calendarEvent1 = new CalendarEvent { Title = "Job1", Range = new DateRange(new DateTime(2008, 12, 1, 10, 0, 0), new DateTime(2008, 12, 1, 11, 0, 0)) };
            var calendarEvent2 = new CalendarEvent { Title = "Job1", Range = new DateRange(new DateTime(2008, 12, 15, 10, 0, 0), new DateTime(2008, 12, 15, 11, 0, 0)) };
            var events = new CalendarEvent[] {calendarEvent1, calendarEvent2};
            
            using (mocks.Record())
            {
                Expect.Call(calendarServices.GetEventsFor(new DateTime(2008, 12, 1), new DateTime(2008, 12, 31))).Return(events);
            }
            using (mocks.Playback())
            {
                PrepareViewComponent(calendar);
                calendar.Render();
                Assert.IsNotNull(calendar.calendarDays);
                Assert.AreEqual(null,calendar.calendarDays[0,0]);
                Assert.AreEqual(new CalendarDay(1,true, new CalendarEvent[] { calendarEvent1 }).Day, calendar.calendarDays[0, 1].Day);
                Assert.AreEqual(new CalendarDay(1,true, new CalendarEvent[] { calendarEvent1 }).CalendarEvents.Count, calendar.calendarDays[0, 1].CalendarEvents.Count);
                Assert.AreEqual(0, calendar.calendarDays[0, 2].CalendarEvents.Count);
                Assert.AreEqual(new CalendarDay(15,true, new CalendarEvent[] { calendarEvent1 }).CalendarEvents.Count, calendar.calendarDays[2, 1].CalendarEvents.Count);
                Assert.AreEqual("components\\Calendar\\default",calendar.Context.ViewToRender);
            }
        }
    }
}