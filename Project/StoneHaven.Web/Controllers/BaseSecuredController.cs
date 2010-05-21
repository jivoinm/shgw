using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Castle.Components.Validator;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using JQueryFormHelper;
using MSoftLibrary.Filters;
using StoneHaven.Web.Helpers;

namespace StoneHaven.Web.Controllers
{
    [Layout("default")]
    [Helper(typeof(AppHelper))]
    [Helper(typeof(JQAjaxHelper), "JQAjax")]
    [Filter(ExecuteWhen.BeforeAction, typeof(Authentication))]
//    [Rescue("error")]
    public class BaseSecuredController: SmartDispatcherController
    {
        private const string ERROR_MSG_KEY = "error";
        private const string ERRORS_MSG_KEY = "errors";
        private const string MSG_KEY = "message";

        protected override object InvokeMethod(MethodInfo method, IRequest request, IDictionary<string, object> extraArgs)
        {
            var parameters = method.GetParameters();
            var methodArgs = BuildMethodArguments(parameters, request, extraArgs);

            var result = method.Invoke(this, methodArgs);

            var isAjaxAction = method.GetCustomAttributes(typeof(AjaxActionAttribute), false).Length > 0;

            if (result != null)
            {
                if (isAjaxAction)
                {
                    if (typeof(bool).IsAssignableFrom(result.GetType()))
                    {
                        RenderText(
                            (bool)result
                                ? Newtonsoft.Json.JavaScriptConvert.True
                                : Newtonsoft.Json.JavaScriptConvert.False
                        );
                    }
                    else
                    {
                        RenderText(Newtonsoft.Json.JavaScriptConvert.SerializeObject(result));
                    }
                }
                else
                {
                    RenderText(result.ToString());
                }
            }
           
            return result;
        }
     
        public string RenderLinkToAction(string linkText, string action,string queryString)
        {
            var helper = GetUrlHelper;
            if(linkText.ToLower().IndexOf("delete")>-1)
                return helper != null ? helper.Link("<img src='" + Context.ApplicationPath + "/Content/icons/delete.png' title='Delete'>", DictHelper.Create("action=" + action, "querystring=" + queryString), DictHelper.Create("onclick=return confirm('Do you want to permanently delete this record?')")) : null;
            if(linkText.ToLower().IndexOf("edit")>-1)
                return helper != null ? helper.Link("<img src='"+Context.ApplicationPath+"/Content/icons/image_edit.png' title='Edit'>", DictHelper.Create("action=" + action,"querystring="+ queryString)) : null;
            
            return helper != null ? helper.Link(linkText, DictHelper.Create("action=" + action,"querystring="+ queryString)) : null;
        }


        public void SetErrorMessage(string message)
        {
            Flash[ERROR_MSG_KEY] = message;
        }

        public void SetErrorMessages(ErrorSummary errors)
        {
            Flash[ERRORS_MSG_KEY] = errors;
        }

        public void SetMessage(string message)
        {
            Flash[MSG_KEY] = message;
        }

        public UrlHelper GetUrlHelper
        {
            get { return Helpers["UrlHelper"] as UrlHelper; }
        }

        public AppHelper GetAppHelper
        {
            get { return Helpers["AppHelper"] as AppHelper; }
        }

        protected FormHelper GetFormHelper
        {
            get { 
                    if (ControllerContext != null) 
                        return ControllerContext.Helpers["FormHelper"] as FormHelper;
                    
                    return new FormHelper(Context);
                }
        }
    }
}