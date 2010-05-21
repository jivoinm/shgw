using System;
using System.Collections;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;

namespace Components.Grids
{
    [ViewComponentDetails("AjaxList", Sections = "List,Edit")]
    public class AjaxList : ViewComponent
    {
        private long compId = DateTime.Now.Ticks;

        [ViewComponentParam(Required = true)]
        public ICollection List { get; set; }
        [ViewComponentParam(Required = true)]
        public string AddAction { get; set; }
        [ViewComponentParam(Required = true)]
        public string RemoveAction { get; set; }
        [ViewComponentParam]
        public string Width { get; set; }
        [ViewComponentParam]
        public string Id { get; set; }
        [ViewComponentParam]
        public string ItemPropertyName { get; set; }
        
        public override void Render()
        {
            var style = Width != null ? string.Format("style='width:{0}'", Width) : null;
            Id = Id ?? "ajaxList_" + compId;
            RenderText(string.Format("<div id=\"{0}\" class=\"basic\" {1}>", Id, style ?? ""));
            var index = 0;
            var count = 1;
            foreach (var item in List)
            {
                PropertyBag[ItemPropertyName ?? "item"] = item;
                PropertyBag["index"] = index;
                PropertyBag["count"] = count;
                
                if (HasSection("List"))
                {
                    RenderSection("List");
                }
                index++;
                count++;    
            }
          
            RenderText("</div>");
            RenderText(AbstractHelper.ScriptBlock("jQuery().ready(function(){ jQuery('#" + Id + "').accordion({header: 'a.title'});})"));
        }
    }
}