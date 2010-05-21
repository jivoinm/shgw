using System.Text;
using Castle.MonoRail.Framework;

namespace Components.Grids
{
    [ViewComponentDetails("Flexgrid", Sections = "ColumnModel,Buttons,Search")]
    public class FlexgridComponent : ViewComponent
    {
        private bool useRemotePaging = false;
        private int recPerPage = 20;
        private int width = 700;
        private int height = 200;

        [ViewComponentParam]
        public string Sortname { get; set; }
 
        [ViewComponentParam]
        public string Title { get; set; }
        
        [ViewComponentParam]
        public string Sortorder { get; set; }

        [ViewComponentParam(Required=true)]
        public string Url{ get; set; }

        [ViewComponentParam]
        public string Id { get; set; }

        [ViewComponentParam]
        public bool UseRemotePaging
        {
            get { return useRemotePaging; }
            set { useRemotePaging = value; }
        }
        
        [ViewComponentParam]
        public int RecPerPage
        {
            get { return recPerPage; }
            set { recPerPage = value; }
        }
        [ViewComponentParam]
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        [ViewComponentParam]
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        [ViewComponentParam]
        public string Query { get; set; }

        public override void Render()
        {
            var init = InitFlexGrid();
            RenderText(init);
            if(HasSection("ColumnModel")) AddSection("ColumnModel","colModel");
            if(HasSection("Buttons")) AddSection("Buttons","buttons");
            if(HasSection("Search")) AddSection("Search","searchitems");
            
            if(Sortname!=null) AddProperty("sortname",Sortname);
            if(Sortorder!=null) AddProperty("sortorder",Sortorder);
            if(Title!=null) AddProperty("title",Title);
            if(UseRemotePaging)
            {
                RenderText(string.Format("{0} : {1},", "usepager", "true"));
                RenderText(string.Format("{0} : {1},", "useRp", "true"));
                RenderText(string.Format("{0} : {1},", "rp", recPerPage));
            }
            
            RenderText(string.Format("width : {0},height : {1}{2}",width,height,"});"));
            if (Query != null)
                RenderText("$(\"#" + Id + "\").flexOptions({params: " + Query + "}).flexReload(); ");
            RenderText("</script>");
        }

        private void AddProperty(string propertyName,string propertyValue)
        {
            RenderText(string.Format("{0} : \"{1}\",", propertyName,propertyValue));
        }

        private void AddSection(string sectionName,string jsSectionName)
        {
            RenderText(jsSectionName + " : ");
            RenderSection(sectionName);
            RenderText(",");
        }

        public string InitFlexGrid()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("<table id=\"{0}\" style=\"display:none\"></table>",Id);
            stringBuilder.Append("<script>");
            stringBuilder.Append("$(\"#" + Id + "\").flexigrid({url: '" + Url + "'");
            if (Query != null)
                stringBuilder.Append(",params: " + Query);
            stringBuilder.Append(",dataType: 'json',");
            return stringBuilder.ToString();
        }
    }
}