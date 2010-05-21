using System.Runtime.Remoting.Contexts;
using Castle.MonoRail.Framework;

namespace Components.Page
{
    [ViewComponentDetails("Baloon",Sections = "Content")]
    public class Baloon : ViewComponent
    {
        public override void Render()
        {
            if (Context.HasSection("Content"))
            {
                RenderText("<div id=\"liquid-round\"><div id=\"liquid-round-rt\"><div class=\"top_bar\"><span></span></div><div class=\"center-content\">");
                Context.RenderSection("Content");
                RenderText("<br/><span></span></div><div class=\"bottom\"><span></span></div></div></div>");
            }

        }
    }
}