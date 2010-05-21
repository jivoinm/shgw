using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MonoRail.Framework.Helpers;

namespace JQueryFormHelper
{
    public class JQAjaxHelper : FormHelper
    {
        public string LinkToRemote(string innerText,string url,string updateId)
        {
            return LinkToRemote(innerText, url, updateId, "form1");
        }

        public string LinkToRemote(string innerText,string url,string updateId,string formId)
        {
            return string.Format( "<a href=\"javascript:void(0)\" onClick=\"AjaxUpdateCallWithFormSubmit('{0}','{1}','{2}')\">{3}</a>",url, updateId, formId, innerText);
        }
    }
}
