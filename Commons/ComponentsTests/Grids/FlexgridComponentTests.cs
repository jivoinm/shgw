using System.Collections;
using System.IO;
using Castle.MonoRail.Framework;
using Castle.MonoRail.TestSupport;
using Components.Grids;

using NUnit.Framework;

namespace ComponentsTests.Grids
{
    [TestFixture]
    public class FlexgridComponentTests : BaseViewComponentTest
    {
        private FlexgridComponent component;

        [SetUp]
        public void Init()
        {
            component = new FlexgridComponent();
        }

        [Test]
        [ExpectedException(typeof(ViewComponentException))]
        public void ThrowExceptionWhenNoUrl()
        {
            component.Url = null;
            PrepareViewComponent(component);
            component.Render();
        }

        [Test]
        public void RenderColumModel()
        {
            component.Url = "test.aspx";
            component.Sortname = "iso";
            component.Sortorder = "asc";
            component.Title = "Test Grid";
            component.UseRemotePaging = true;
            
            var table = "<table id=\""+component.Id+"\" style=\"display:none\"></table>";
            var initScript = "<script>$(\"#" + component.Id + "\").flexigrid({url: '"+component.Url+"',dataType: 'json',";
            var columnModelSection = "colModel :[{display: 'ISO', name : 'iso', width : 40, sortable : true, align: 'center'},{display: 'Name', name : 'name', width : 180, sortable : true, align: 'left'}]";
            var buttonsSection = "[{name: 'Add', bclass: 'add', onpress : test},{name: 'Delete', bclass: 'delete', onpress : test},{separator: true}]";
            var searchSection = "[{display: 'ISO', name : 'iso'},{display: 'Name', name : 'name', isdefault: true}]";
            
            SectionRender["ColumnModel"] = ((context, writer) =>writer.WriteLine(columnModelSection));
            SectionRender["Buttons"] = ((context,writer) => writer.WriteLine(buttonsSection));
            SectionRender["Search"] = ((context,writer) => writer.WriteLine(searchSection));
            
            PrepareViewComponent(component);
            Assert.IsTrue(component.SupportsSection("ColumnModel"));
            Assert.IsTrue(component.SupportsSection("Buttons"));
            Assert.IsTrue(component.SupportsSection("Search"));
            component.Render();

            var buildScript = table + initScript + 
                              "colModel : " + columnModelSection+System.Environment.NewLine + ","+
                              "buttons : " + buttonsSection+System.Environment.NewLine + "," +
                              "searchitems : " + searchSection+System.Environment.NewLine + ","+
                              "sortname : \"iso\","+
                              "sortorder : \"asc\","+
                              "title : \"Test Grid\","+
                              "usepager : true," +
                              "rp : 20," +
                              "width : 700,"+
                              "height : 200"+
                              "})</script>"
                ;
            Assert.AreEqual(buildScript,Output);
        }
    }
}