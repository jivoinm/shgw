using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.MonoRail.Framework.Helpers;
using Rhino.Commons;
using StoneHaven.Models;

namespace StoneHaven.Web.Helpers
{
    public class AppHelper : FormHelper
    {
        public string DateField(string target,IDictionary properties)
        {
            var val = ObtainValue(target);
            var htmlId = CreateHtmlId(target);
            string date = null;
            if (val != null)
            {
                date = ((DateTime?) val).Value.ToShortDateString();
            }

            return TextFieldValue(target, date, properties) + ScriptBlock("$(document).ready(function() {$(\"#" + htmlId + "\").datepicker();});");
        }
        public string DateTimeField(string target,IDictionary properties)
        {
            var val = ObtainValue(target);
            string dateValue = null;
            if (val != null)
            {
                dateValue = ((DateTime?)val).Value.ToShortDateString(); 
                val = ((DateTime?) val).Value.ToString("hh:mm tt");
            }
            var dateTime = TextFieldValue(target+"_date", dateValue, properties);
            var timeA = GenerateTimeArray(6,20);
            return string.Format("{0}&nbsp;@&nbsp;{1}{2}{3}", dateTime, Select(target + "_time", timeA, DictHelper.Create("style=width:80px","firstoption=")),
                ScriptBlock("$(\"[name*='" + target + "']\").change(function(){$(\"[name='" + target + "']\").val($(\"[name='" + target + "_date']\").val()+' '+$(\"[name='" + target + "_time']\").val())});$(\"[name='"+target+"_date']\").datepicker();" + string.Format("$(\"[name='{0}']\").val('{1}')", target + "_time", val ?? "")), HiddenField(target));
        }

        private static IList<string> GenerateTimeArray(int from, int to)
        {
            var times = new List<string>();
            for (var hour = from; hour < to; hour++)
              for (var minute = 0; minute < 60; minute++)
                if (minute % 15 == 0)
                   times.Add(new DateTime(TimeSpan.Parse(string.Format("{0:00}:{1:00}", hour, minute)).Ticks).ToString("hh:mm tt"));
            return times;
        }


        public string NumericField(string target,IDictionary properties)
        {
            var val = ObtainValue(target);
            return NumericField(target, val, properties);
        }

        public string NumericField(string target,object val,IDictionary properties)
        {
            try
            {
                if (val != null && properties.Contains("round"))
                {
                    var round = properties["round"];
                    val = Math.Round((decimal) val,(int)round);
                }
            }
            catch {val = 0; }
            return TextFieldValue(target,val, properties) + ScriptBlock(string.Format("$(\"[name='{0}']\").numeric();", target));
        }

//        public string TreeSelectBox(IList<Project> list)
//        {
//            var result = new StringBuilder();
//            if(list!=null)
//            {
//                foreach (var item in list)
//                {
//                    result.AppendFormat("\"{0}\":{1}", item.Name, "{\"" + item.Id);
//                }
//            }
//            return result.ToString();
//        }
//
//        private string RenderRecursiveProjects(IList<Project> list,StringBuilder options)
//        {
//            foreach (var project in list)
//            {
//                options.AppendFormat("\"{0}\":{1}", project.Name, "{\"" + project.Id);
//            }
//        }
    }
}