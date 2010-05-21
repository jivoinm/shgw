/*
 * Created by: Zivojin, Mirca
 * Created: December 11, 2008
 */

using System;
using System.Collections;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using Rhino.Commons;

namespace Components.Page
{
    [ViewComponentDetails("JQAutocomplete")]
    public class JQueryAutocomplete : ViewComponent
    {
        private string target;
        private string source;
        private bool isMultiSelect;
        private bool sendSelectedId;
        private string dependsOn;
        private object targetValue;
        private bool isRequired;
        private string onSelect;
        private bool mustMatch=true;
        private string onNoSelect;
        private bool cache = true;

        #region Properties

        [ViewComponentParam(Required = true)]
        public string Target
        {
            get { return target; }
            set { target = value; }
        }
        
        [ViewComponentParam(Required = true)]
        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        [ViewComponentParam()]
        public bool IsMultiSelect
        {
            get { return isMultiSelect; }
            set { isMultiSelect = value; }
        }

        [ViewComponentParam()]
        public bool SendSelectedId
        {
            get { return sendSelectedId; }
            set { sendSelectedId = value; }
        }

        [ViewComponentParam()]
        public string DependsOn
        {
            get { return dependsOn; }
            set { dependsOn = value; }
        }

        [ViewComponentParam()]
        public object TargetValue
        {
            get { return targetValue; }
            set { targetValue = value; }
        }

        [ViewComponentParam()]
        public bool IsRequired
        {
            get { return isRequired; }
            set { isRequired = value; }
        }

        [ViewComponentParam()]
        public string OnSelect
        {
            get { return onSelect; }
            set { onSelect = value; }
        }

        [ViewComponentParam()]
        public bool MustMatch
        {
            get { return mustMatch; }
            set { mustMatch = value; }
        }

        [ViewComponentParam()]
        public string OnNoSelect
        {
            get { return onNoSelect; }
            set { onNoSelect = value; }
        }

        [ViewComponentParam()]
        public bool Cache
        {
            get { return cache; }
            set { cache = value; }
        }

        #endregion

        public override void Render()
        {

            IoC.Resolve<IViewSourceLoader>().AddAssemblySource(new AssemblySourceInfo("Components", "Components.Views"));

            var helper = EngineContext.CurrentControllerContext.Helpers["SuiteHtmlHelper"] as AbstractFormRelatedHelper;
            
            if(helper!=null)
            {
                string targetAC = target;
                if (sendSelectedId) targetAC += "_AC";
                if (isMultiSelect)
                {
                    sendSelectedId = false;
                    PropertyBag["targetAC"] = targetAC;
                    //targetValue = targetValue ?? helper.ObtainValue(target);
                    PropertyBag["targetValue"] = targetValue;
                }else
                {
                    PropertyBag["targetAC"] = targetValue ?? targetAC;
                    PropertyBag["targetValue"] = targetValue;
                }

            }

            PropertyBag["isMultiSelect"] = IsMultiSelect;
            IDictionary properties = new Hashtable();
            if(IsRequired) properties.Add("class","required");
            else properties.Add("disablevalidation", "true");
            
            PropertyBag["properties"] = properties;
            PropertyBag["targetACID"] = target;
            PropertyBag["source"] = source;
            PropertyBag["sendSelectedId"] = sendSelectedId;
            PropertyBag["dependsOn"] = dependsOn;
            PropertyBag["onSelect"] = onSelect;
            PropertyBag["onNoSelect"] = onNoSelect;
            PropertyBag["mustMatch"] = mustMatch?"true":"false";
            PropertyBag["cache"] = cache?"1":"0";
            PropertyBag["componentID"] = new Random().Next(10000);
            RenderView("default");
        }
    }
}