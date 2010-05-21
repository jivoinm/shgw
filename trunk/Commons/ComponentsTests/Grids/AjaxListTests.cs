using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.TestSupport;
using Components.Grids;
using NUnit.Framework;

namespace ComponentsTests.Grids
{
    [TestFixture]
    public class AjaxListTests : BaseViewComponentTest
    {
        private AjaxList ajaxList;
        [SetUp]
        public void Setup()
        {
            ajaxList = new AjaxList();
        }

        [Test]
        [ExpectedException(typeof(ViewComponentException), ExpectedMessage = "The parameter 'List' is required by the ViewComponent AjaxList but was not passed or had a null value")]
        public void TestRequiredListProperty()
        {
            ajaxList.List = null;
            PrepareViewComponent(ajaxList);
            ajaxList.Render();
        }

        [Test]
        [ExpectedException(typeof(ViewComponentException), ExpectedMessage = "The parameter 'AddAction' is required by the ViewComponent AjaxList but was not passed or had a null value")]
        public void TestRequiredAddActionProperty()
        {
            ajaxList.List = new object[]{};
            PrepareViewComponent(ajaxList);
            ajaxList.Render();
        }
        
        [Test]
        [ExpectedException(typeof(ViewComponentException), ExpectedMessage = "The parameter 'RemoveAction' is required by the ViewComponent AjaxList but was not passed or had a null value")]
        public void TestRequiredRemoveActionProperty()
        {
            ajaxList.List = new object[]{};
            ajaxList.AddAction = "Add action";
            ajaxList.RemoveAction = null;
            PrepareViewComponent(ajaxList);
            ajaxList.Render();
        } 

        [Test]
        public void TestRenderAjaxListComponent()
        {
            ajaxList.List = new object[]{"Item1","Item2"};
            ajaxList.AddAction = "Add action";
            ajaxList.RemoveAction = "Remove action";

            SectionRender["List"] = ((context, writer) => writer.WriteLine(""));

            PrepareViewComponent(ajaxList);
            Assert.IsTrue(ajaxList.SupportsSection("List"));
            ajaxList.Render();
            Assert.AreEqual("", Output);
        }
    }
}