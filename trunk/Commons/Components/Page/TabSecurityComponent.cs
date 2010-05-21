using System;
using System.Collections;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;

namespace Components.Page
{
    public class TabSecurityComponent : BaseSecurityComponent
    {
        private bool shouldRender;
        private string action;
        private string text;
        private string link;
        private string selected;
        private string alwaysEnabled;
        private IDictionary parameters = new Hashtable();
//        private PageViewStates state;
//        private IDFRSSecurity security;
//        private ISystemPropertiesProvider systemPropertiesProvider;


//        public TabSecurityComponent(IDFRSSecurity baseSecurity, ISystemPropertiesProvider baseSystemPropertiesProvider, IDFRSSecurity security, ISystemPropertiesProvider systemPropertiesProvider) : base(baseSecurity, baseSystemPropertiesProvider)
//        {
//            this.security = security;
//            this.systemPropertiesProvider = systemPropertiesProvider;
//        }

        [ViewComponentParam(Required = true)]
        public string Action
        {
            get { return action; }
            set { action = value; }
        }

        [ViewComponentParam()]
        public bool ShouldRender
        {
            get { return shouldRender; }
            set { shouldRender = value; }
        }
        [ViewComponentParam(Required = true)]
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        [ViewComponentParam(Required=true)]
        public string Link
        {
            get { return link; }
            set { link = value; }
        }
        
        [ViewComponentParam()]
        public string Selected
        {
            get { return selected; }
            set { selected = value; }
        }
        [ViewComponentParam()]
        public string AlwaysEnabled
        {
            get { return alwaysEnabled; }
            set { alwaysEnabled = value; }
        }
        [ViewComponentParam()]
        public IDictionary Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        /// <summary>
        /// Called by the framework once the component instance
        /// is initialized
        /// </summary>
        public override void Initialize()
        {
            if (action == "*")
            {
                shouldRender = true;
            }
//            state = PageViewStates.IsEdit;
            //auto select the tab
            if (EngineContext.Request.Url.Contains(link))
                selected = "true";
            
//            if (!shouldRender)
//            {
//                string[] roles = CachedRoles(EngineContext.CurrentControllerContext.Name, action);
//
//                if (roles != null)
//                {
//                    foreach (string itRole in roles)
//                    {
//                        if (EngineContext.CurrentUser.IsInRole(itRole.Trim()))
//                        {
//                            shouldRender = true;
//                            break;
//                        }
//                    }
//                }
//            }
        }

        /// <summary>
        /// Called by the framework so the component can
        /// render its content
        /// </summary>
        public override void Render()
        {
            UrlHelper help = (UrlHelper) EngineContext.CurrentControllerContext.Helpers["UrlHelper"];
            if (shouldRender)
            {
                string cssclass = "divTabButtonSelected";
//                bool isDisabled = false;
//                if (alwaysEnabled != null)
//                    cssclass = "class=\"divTabButtonSelected\"";
//                else
//                {
//                    cssclass = selected != null
//                        ? "class=\"divTabButtonSelected\"":"";
//
//                    if (IsNewObject() && selected == null)
//                    {
//                        cssclass = "class=\"divTabButtonDisabled\"";
//                        isDisabled = true;
//                    }
//                }

//                if(state == PageViewStates.IsEdit)
//                    parameters.Add("id",CurrentContactID);

                string qstring = help.BuildQueryString(parameters);
//                if(isDisabled)
//                    RenderText(string.Format("<a id=\"{0}\" {1} href=\"#\">{0}</a>", text, cssclass));
//                else
                RenderText(help.Link(text, DictHelper.Create("action=" + link,"querystring="+qstring), DictHelper.Create("class=" + cssclass)));
                //RenderText(string.Format("<a id=\"{1}\" {2} href=\"{0}{3}\">{1}</a>", link, text, cssclass, qstring!=""? "?"+qstring:""));
            }
        }

        private bool IsNewObject()
        {
//            if ( state == PageViewStates.IsEdit)
//                return false;
//            else
                return true;
        }

        public int CurrentContactID
        {
            get
            {
//                if (Request["id"] != null)
//                    return Utility.Int(SecureUrl.DecodeId(Request["id"]));
//                else
                    return 0;
            }
        }
    }
}
