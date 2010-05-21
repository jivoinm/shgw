/*
 * Created by: Zivojin, Mirca
 * Created: December 11, 2008
 */

using MemberSuite.Core;

namespace MemberSuite.Web.Core.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Castle.MonoRail.Framework;
    using Castle.MonoRail.Framework.Helpers;
    using MemberSuite.Core.Models;
    using MemberSuite.Core.Services;
    using NHibernate;
    using NHibernate.SqlCommand;
    using NHibernate.Transform;
    using Rhino.Commons;
    using Query;
    using NHibernate.Criterion;

    public class JQAutocompleteActionProvider :IDynamicActionProvider
    {
        public void IncludeActions(IEngineContext engineContext, IController controller,
                                   IControllerContext controllerContext)
        {
            controllerContext.DynamicActions["JQOrgSearch"] = new JQOrgSearch();
            controllerContext.DynamicActions["JQOrgByBusinessTypeSearch"] = new JQOrgByBusinessTypeSearch();
            controllerContext.DynamicActions["BusinessCategorySearch"] = new BusinessCategorySearch();
            controllerContext.DynamicActions["RegionalPartnershipSearch"] = new RegionalPartnershipSearch();
            controllerContext.DynamicActions["RolesAndResponsibilitiesSearch"] = new RolesAndResponsibilitiesSearch();
            controllerContext.DynamicActions["ContactCategoriesSearch"] = new ContactCategoriesSearch();
            controllerContext.DynamicActions["ContactExpertiseSearch"] = new ContactExpertiseSearch();
            controllerContext.DynamicActions["CitySearch"] = new CitySearch();
            controllerContext.DynamicActions["JQAccountSearch"] = new JQAccountSearch();
            controllerContext.DynamicActions["JQExpenseAccountSearch"] = new JQExpenseAccountSearch();
            controllerContext.DynamicActions["JSONLoadMembershipInfo"] = new JSONLoadMembershipInfo();
            controllerContext.DynamicActions["JSONLoadEventsByDepartment"] = new JSONLoadEventsByDepartment();
            controllerContext.DynamicActions["CalculateTaxes"] = new CalculateTaxes();
        }
    }


    public class CalculateTaxes : IDynamicAction
    {
        public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
        {
            int gstTaxId = engineContext.Request.QueryString["gstTaxId"] != null ? Convert.ToInt32(engineContext.Request.QueryString["gstTaxId"]) : 0;
            int pstTaxId = engineContext.Request.QueryString["pstTaxId"] != null ? Convert.ToInt32(engineContext.Request.QueryString["pstTaxId"]) : 0;

            decimal amount = engineContext.Request.QueryString["amount"] != null ? Convert.ToDecimal(engineContext.Request.QueryString["amount"]) : 0;
            AppEnumeration.TaxType type = engineContext.Request.QueryString["type"] != null ? (AppEnumeration.TaxType)Enum.Parse(typeof(AppEnumeration.TaxType), engineContext.Request.QueryString["type"]) : AppEnumeration.TaxType.NoTax;

            JSONHelper jsonHelper = controllerContext.Helpers["JSONHelper"] as JSONHelper;
            IDictionary<string, string> jsonRes;
            if (type > 0 && amount > 0)
            {

                jsonRes = IoC.Resolve<IBillingServices>().CalculateTaxes(amount, type, gstTaxId, pstTaxId);
            }
            else
            {
                jsonRes = GetDictTax(0, 0, 0);
            }
            BaseController baseCtrl = (BaseController)controller;

            baseCtrl.Response.ContentType = "application/json; charset=utf-8";
            baseCtrl.RenderText(jsonHelper != null ? jsonHelper.ToJSON(jsonRes) : "");

            return null;
        }

        private static IDictionary<string, string> GetDictTax(decimal gst, decimal pst, decimal pretax)
        {
            IDictionary<string, string> jsonRes = new Dictionary<string, string>();
            jsonRes.Add("gst", gst.ToString());
            jsonRes.Add("pst", pst.ToString());
            jsonRes.Add("pretax", pretax.ToString());
            return jsonRes;
        }

    }


    /// <summary>
    /// Dynamic Actions implementation for Autocomplete searches
    /// </summary>
   
    public class JQOrgSearch : IDynamicAction
    {
        [SkipFilter]
        public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
        {
            string keyStrig = engineContext.Request.QueryString["q"];
            int limit = Convert.ToInt32(engineContext.Request.QueryString["limit"]);
            BaseController baseCtrl = (BaseController)controller;
            JSONHelper jsonHelper = controllerContext.Helpers["JSONHelper"] as JSONHelper;

            ICriteria criteria = UnitOfWork.CurrentSession.CreateCriteria(typeof(Membership))
                .SetProjection(
                    Projections.ProjectionList()
                    .Add(Projections.Property("MembershipID"),"Id")
                    .Add(Projections.Property("CompanyName"),"Name")
                )
                .Add(Restrictions.Like("CompanyName",keyStrig,MatchMode.Anywhere))
                .Add(Restrictions.Eq("Deleted",false))
                .SetMaxResults(limit);

            criteria.SetResultTransformer(Transformers.AliasToBean(typeof(AutoCompleteModel)));
            IList list = criteria.List();

            if (jsonHelper != null)
            {
                string json = jsonHelper.ToJSON(list);
                baseCtrl.RenderText(json);
            }
            baseCtrl.Response.ContentType = "application/json; charset=utf-8";

            return null;
        }

    }

    public class JQOrgByBusinessTypeSearch : IDynamicAction
    {
        public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
        {
            string keyStrig = engineContext.Request.QueryString["q"];
            int dependsOn =Convert.ToInt32(engineContext.Request.QueryString["dependsOn"]);
            int limit = Convert.ToInt32(engineContext.Request.QueryString["limit"]);
            BaseController baseCtrl = (BaseController)controller;
            JSONHelper jsonHelper = controllerContext.Helpers["JSONHelper"] as JSONHelper;

            ICriteria criteria = UnitOfWork.CurrentSession.CreateCriteria(typeof(Membership))
                .CreateAlias("BusinessType","bt")
                .SetProjection(
                    Projections.ProjectionList()
                    .Add(Projections.Property("MembershipID"),"Id")
                    .Add(Projections.Property("CompanyName"),"Name")
                )
                .Add(Restrictions.Like("CompanyName",keyStrig,MatchMode.Anywhere))
                .Add(Restrictions.Eq("Deleted",false))
                .Add(Restrictions.Eq("bt.BusinessTypeID",dependsOn))
                .SetMaxResults(limit);

            criteria.SetResultTransformer(Transformers.AliasToBean(typeof(AutoCompleteModel)));
            IList list = criteria.List();

            if (jsonHelper != null)
            {
                string json = jsonHelper.ToJSON(list);
                baseCtrl.RenderText(json);
            }
            baseCtrl.Response.ContentType = "application/json; charset=utf-8";

            return null;
        }

    }

    public class BusinessCategorySearch : IDynamicAction
    {

        public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
        {
            string keyStrig = engineContext.Request.QueryString["q"];
            int limit = Convert.ToInt32(engineContext.Request.QueryString["limit"]);
            int currentLID = Thread.CurrentThread.CurrentUICulture.LCID;
            BaseController baseCtrl = (BaseController)controller;
            JSONHelper jsonHelper = controllerContext.Helpers["JSONHelper"] as JSONHelper;

            IList<LocalizedEntity> res = IoC.Resolve<ILocalizeBaseLookUp>().FindAllLocalized<BusinessCategory>("Name", "BusinessCategoryName", currentLID,"Name like '%" + keyStrig + "%'");

            if (jsonHelper != null)
            {
                string json = jsonHelper.ToJSON(res);
                baseCtrl.RenderText(json);
            }
            baseCtrl.Response.ContentType = "application/json; charset=utf-8";
            return null;
        }
    }

    public class RegionalPartnershipSearch : IDynamicAction
    {
        public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
        {
            string keyStrig = engineContext.Request.QueryString["q"];
            keyStrig = keyStrig != null ? keyStrig.Replace("'", "\'") : string.Empty;
            int limit = Convert.ToInt32(engineContext.Request.QueryString["limit"]);
            int currentLID = Thread.CurrentThread.CurrentUICulture.LCID;
            BaseController baseCtrl = (BaseController)controller;
            JSONHelper jsonHelper = controllerContext.Helpers["JSONHelper"] as JSONHelper;

            IList<LocalizedEntity> res = IoC.Resolve<ILocalizeBaseLookUp>().FindAllLocalized<RegionalPartnership>("Name", "RegionalPartnershipName", currentLID, "Name like '%" + keyStrig + "%'");

            if (jsonHelper != null)
            {
                string json = jsonHelper.ToJSON(res);
                baseCtrl.RenderText(json);
            }
            baseCtrl.Response.ContentType = "application/json; charset=utf-8";
            return null;
        }
    }


    public class RolesAndResponsibilitiesSearch : IDynamicAction
    {
        public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
        {
            string keyStrig = engineContext.Request.QueryString["q"];
            int limit = Convert.ToInt32(engineContext.Request.QueryString["limit"]);
            int currentLID = Thread.CurrentThread.CurrentUICulture.LCID;
            BaseController baseCtrl = (BaseController)controller;
            JSONHelper jsonHelper = controllerContext.Helpers["JSONHelper"] as JSONHelper;

            IList<LocalizedEntity> res = IoC.Resolve<ILocalizeBaseLookUp>().FindAllLocalized<RoleAndresponsibility>("Name", "RoleAndresponsibilityName", currentLID, "Name like '%" + keyStrig + "%'");

            if (jsonHelper != null)
            {
                string json = jsonHelper.ToJSON(res);
                baseCtrl.RenderText(json);
            }
            baseCtrl.Response.ContentType = "application/json; charset=utf-8";
            return null;
        }
    }
    public class ContactCategoriesSearch : IDynamicAction
    {
        public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
        {
            string keyStrig = engineContext.Request.QueryString["q"];
            int limit = Convert.ToInt32(engineContext.Request.QueryString["limit"]);
            int currentLID = Thread.CurrentThread.CurrentUICulture.LCID;
            BaseController baseCtrl = (BaseController)controller;
            JSONHelper jsonHelper = controllerContext.Helpers["JSONHelper"] as JSONHelper;

            IList<LocalizedEntity> res = IoC.Resolve<ILocalizeBaseLookUp>().FindAllLocalized<ContactCategory>("Name", "ContactCategoryName", currentLID, "Name like '%" + keyStrig + "%'");

            if (jsonHelper != null)
            {
                string json = jsonHelper.ToJSON(res);
                baseCtrl.RenderText(json);
            }
            baseCtrl.Response.ContentType = "application/json; charset=utf-8";
            return null;

        }
    }
    public class ContactExpertiseSearch : IDynamicAction
    {
        public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
        {
            string keyStrig = engineContext.Request.QueryString["q"];
            int limit = Convert.ToInt32(engineContext.Request.QueryString["limit"]);
            int currentLID = Thread.CurrentThread.CurrentUICulture.LCID;
            BaseController baseCtrl = (BaseController)controller;
            JSONHelper jsonHelper = controllerContext.Helpers["JSONHelper"] as JSONHelper;

            IList<LocalizedEntity> res = IoC.Resolve<ILocalizeBaseLookUp>().FindAllLocalized<ContactExpertise>("Name", "ContactExpertiseName", currentLID, "Name like '%" + keyStrig + "%'");

            if (jsonHelper != null)
            {
                string json = jsonHelper.ToJSON(res);
                baseCtrl.RenderText(json);
            }
            baseCtrl.Response.ContentType = "application/json; charset=utf-8";
            return null;

        }
    }

    public class CitySearch : IDynamicAction
    {
        public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
        {
            string keyStrig = engineContext.Request.QueryString["q"];
            string provinceId = engineContext.Request.QueryString["dependsOn"];
            int currentLID = Thread.CurrentThread.CurrentUICulture.LCID;
            BaseController baseCtrl = (BaseController)controller;
            JSONHelper jsonHelper = controllerContext.Helpers["JSONHelper"] as JSONHelper;
            string query = string.Format("Name like '%{0}%'",keyStrig);
            if (provinceId != null) query += string.Format(" and entity.ProvinceID = {0}",provinceId);

            IList<LocalizedEntity> res = IoC.Resolve<ILocalizeBaseLookUp>()
                .FindAllLocalized<City>("Name", "CityName", currentLID, query);

            if (jsonHelper != null)
            {
                string json = jsonHelper.ToJSON(res);
                baseCtrl.RenderText(json);
            }
            baseCtrl.Response.ContentType = "application/json; charset=utf-8";
            return null;
        }
    }
    
    public class JQAccountSearch : IDynamicAction
    {
        public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
        {
            string keyStrig = engineContext.Request.QueryString["q"];
            int currentLID = Thread.CurrentThread.CurrentUICulture.LCID;
            BaseController baseCtrl = (BaseController)controller;
            JSONHelper jsonHelper = controllerContext.Helpers["JSONHelper"] as JSONHelper;
            string query = string.Format("Description like '%{0}%'", keyStrig);
            

            IList<LocalizedEntity> res = IoC.Resolve<ILocalizeBaseLookUp>()
                .FindAllLocalized<Account>("Description", "AccountName", currentLID, query);

            if (jsonHelper != null)
            {
                string json = jsonHelper.ToJSON(res);
                baseCtrl.RenderText(json);
            }
            baseCtrl.Response.ContentType = "application/json; charset=utf-8";
            return null;
        }
    }

    public class JQExpenseAccountSearch : IDynamicAction
    {
        public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
        {
            string keyStrig = engineContext.Request.QueryString["q"];
            int currentLID = Thread.CurrentThread.CurrentUICulture.LCID;
            BaseController baseCtrl = (BaseController)controller;
            JSONHelper jsonHelper = controllerContext.Helpers["JSONHelper"] as JSONHelper;
            string query = string.Format("Name like '%{0}%'", keyStrig);
            

            IList<LocalizedEntity> res = IoC.Resolve<ILocalizeBaseLookUp>()
                .FindAllLocalized<ExpenseAccount>("Name", "ExpenseAccountName", currentLID, query);

            if (jsonHelper != null)
            {
                string json = jsonHelper.ToJSON(res);
                baseCtrl.RenderText(json);
            }
            baseCtrl.Response.ContentType = "application/json; charset=utf-8";
            return null;
        }
    }

    public class JSONLoadMembershipInfo : IDynamicAction
    {

        #region IDynamicAction Members

        public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
        {
            int memberId = Convert.ToInt32(engineContext.Request.QueryString["id"]);
            BaseController baseCtrl = (BaseController)controller;
            JSONHelper jsonHelper = controllerContext.Helpers["JSONHelper"] as JSONHelper;

            ICriteria criteria = UnitOfWork.CurrentSession.CreateCriteria(typeof(Membership))
                .CreateAlias("PhysicalAddress","address")
                .CreateAlias("address.City","city")
                .CreateAlias("city.Province","province")
                .CreateAlias("province.Country","country")
                .CreateAlias("PrimaryPhone", "phone", JoinType.LeftOuterJoin)
                .CreateAlias("SecondaryPhone", "secondPhone",JoinType.LeftOuterJoin)
                .CreateAlias("Toll", "toll", JoinType.LeftOuterJoin)
                .CreateAlias("Fax", "fax", JoinType.LeftOuterJoin)

                .SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Property("MembershipID"))
                    .Add(Projections.Property("CompanyName"))
                    .Add(Projections.Property("address.Address1"))
                    .Add(Projections.Property("address.Address2"))
                    .Add(Projections.Property("address.PostalZip"))
                    .Add(Projections.Property("city.CityID"))
                    .Add(Projections.Property("city.Name"))
                    .Add(Projections.Property("province.ProvinceID"))
                    .Add(Projections.Property("province.Name"))
                    .Add(Projections.Property("country.CountryID"))
                    .Add(Projections.Property("country.Name"))
                    .Add(Projections.Property("phone.PhoneNumber"))
                    .Add(Projections.Property("phone.Extension"))
                    .Add(Projections.Property("secondPhone.PhoneNumber"))
                    .Add(Projections.Property("secondPhone.Extension"))
                    .Add(Projections.Property("toll.PhoneNumber"))
                    .Add(Projections.Property("fax.PhoneNumber"))
                    .Add(Projections.Property("Email"))
                    .Add(Projections.Property("Website"))
                )
                .Add(Restrictions.Eq("MembershipID", memberId))
                .Add(Restrictions.Eq("Deleted", false));

            IList list = criteria.List();

            if (jsonHelper != null && list.Count>0)
            {
                string json = jsonHelper.ToJSON(list[0]);
                baseCtrl.RenderText(json);
            }else
                baseCtrl.RenderText("alert('No Results found')");
            
            baseCtrl.Response.ContentType = "application/json; charset=utf-8";

            return null;
        }

        #endregion
    }

    public class JSONLoadEventsByDepartment : IDynamicAction
    {
        public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
        {
            string keyStrig = engineContext.Request.QueryString["q"];
            int dependsOn = Convert.ToInt32(engineContext.Request.QueryString["dependsOn"]);
            int limit = Convert.ToInt32(engineContext.Request.QueryString["limit"]);
            BaseController baseCtrl = (BaseController)controller;
            JSONHelper jsonHelper = controllerContext.Helpers["JSONHelper"] as JSONHelper;

            ICriteria criteria = UnitOfWork.CurrentSession.CreateCriteria(typeof(Event))
                .CreateAlias("EventGroup", "egroup")
                .CreateAlias("egroup.Department", "dep")
                .SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Property("EventID"), "Id")
                    .Add(Projections.Property("Name"), "Name")
                )
                .Add(Restrictions.Like("Name", keyStrig, MatchMode.Anywhere))
                .Add(Restrictions.Eq("Deleted", false));

            if (dependsOn > 0)
                criteria.Add(Restrictions.Eq("dep.DepartmentID", dependsOn));
            
            criteria.SetMaxResults(limit);

            criteria.SetResultTransformer(Transformers.AliasToBean(typeof(AutoCompleteModel)));
            IList list = criteria.List();

            if (jsonHelper != null)
            {
                string json = jsonHelper.ToJSON(list);
                baseCtrl.RenderText(json);
            }
            baseCtrl.Response.ContentType = "application/json; charset=utf-8";

            return null;
        }
    }

    public class AutoCompleteModel
    {
        private int id;
        private string name;


        public AutoCompleteModel()
        {
        }

        public AutoCompleteModel(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set
            {
                //name = value!=null?value.Replace(",","\\,"):null;
                name = value;
            }
        }
    }
}