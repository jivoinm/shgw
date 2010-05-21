using System.Runtime.Remoting.Contexts;
using Castle.MonoRail.Framework;

namespace Components.Page
{
    public class PageViewComponent : BaseSecurityComponent
    {
        private const string TabsSection = "tabs";
        private const string HeaderTextSection = "headerText";
        private const string SearchSection = "search";
        private const string MessagesSection = "messages";
        private const string ActionsSection = "actions";
        private const string ContentSection = "content";
        private bool showPage = true;
        private string pageTitle;


        public override void Render()
        {
            if (showPage)
            {
                //RenderText("<div class=\"heading\">" + pageTitle + "</div>");
                //if (Context.HasSection(MessagesSection))
                    Context.RenderSection(MessagesSection);

                //RenderText("<div class=\"formWrap\"><fieldset style=\"background:#F5F5F5;\"><div class=\"formIndent\">&nbsp;");


//                if (Context.HasSection(TabsSection))
//                {
//                    RenderText("<div class=\"divTabButton\">&nbsp;&nbsp;");
//                    Context.RenderSection(TabsSection);
//                    RenderText("</div>");
//                }

                if (Context.HasSection(HeaderTextSection))
                {
                    RenderText("<div id=\"liquid-round\"><div id=\"liquid-round-rt\"><div class=\"top_bar\"><span></span></div><div class=\"center-content\">");
                    Context.RenderSection(HeaderTextSection);
                    RenderText("<br/><span></span></div><div class=\"bottom\"><span></span></div></div></div>");
                }

                if (Context.HasSection(SearchSection))
                {
                    RenderText("<div id=\"liquid-round\"><div id=\"liquid-round-rt\"><div class=\"top_bar\"><span></span></div><div class=\"center-content\">");
                    Context.RenderSection(SearchSection);
                    RenderText("<br/><span></span></div><div class=\"bottom\"><span></span></div></div></div>");
                }
                
                if (Context.HasSection(ActionsSection))
                {
                    RenderText("<div class=\"alignLeft\">");
                    Context.RenderSection(ActionsSection);
                    RenderText("</div>");
                }

                if (Context.HasSection(ContentSection))
                {
                    RenderText("<br/><div id=\"liquid-round\"><div id=\"liquid-round-rt\"><div class=\"top_bar\"><span></span></div><div class=\"center-content\">");
                    PropertyBag["required"] = "<font color='red'>&nbsp;*</font>";
                    Context.RenderSection(ContentSection);
                    RenderText("<br/><br/><br/><span></span></div><div class=\"bottom\"><span></span></div></div></div>");
                }

                if (Context.HasSection(ActionsSection))
                {
                    RenderText("<div class=\"alignLeft\">");
                    Context.RenderSection(ActionsSection);
                    RenderText("</div>");
                }
                RenderText("</fieldset></div>");
            }
            else
            {
                RenderText(string.Format("<div><img src=\"{0}/content/images/bg-errorLarge.gif\" alt=\"error\"/><strong>{1}</strong></div>", EngineContext.ApplicationPath, "No Access!"));
                RenderText(string.Format("<br/><a href=\"javascript:history.back();\" alt=\"back\">{0}</a>", "Back"));
            }
        }

        public override void Initialize()
        {

            pageTitle = (string)ComponentParams["pageTitle"];
            PropertyBag["title"] = pageTitle;

//            if (EngineContext.CurrentUser.Identity.IsAuthenticated &&
//                EngineContext.CurrentControllerContext.Name.ToLower() != "propertyformwizard" &&
//                EngineContext.CurrentControllerContext.Action.ToLower() != "changepassword")
//            {
//                showPage = UserHasAccessTo("Menu Access");
//            }

            base.Initialize();
        }

       


        public override bool SupportsSection(string name)
        {
            return name == SearchSection || name == TabsSection || name == HeaderTextSection || name == ActionsSection || name == ContentSection || name == MessagesSection;
        }
    }

    public class BaseSecurityComponent : ViewComponent
    {
        protected bool UserHasAccessTo(string menuAccess)
        {
            return true;
        }
    }
}