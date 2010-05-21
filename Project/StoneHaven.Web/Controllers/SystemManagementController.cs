using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Components.Binder;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using NHibernate.Criterion;
using Query;
using Rhino.Commons;
using StoneHaven.Models;
using StoneHaven.Models.Services;
using StoneHaven.Models.Filters;
using StoneHaven.Web.Forms;

namespace StoneHaven.Web.Controllers
{
    public class SystemManagementController : BaseSecuredController
    {
        public void Index(string itemTip)
        {
            switch (itemTip)
            {
                case "Edge":
                    SetPropertyBagsFor(new Edge(), "Edge", IoC.Resolve<IEntityServices>().FindAll<Edge>(new Order[]{OrderBy.Edge.Name}));
                    break;
                case "Thicknes":
                    SetPropertyBagsFor(new Thicknes(), "Thicknes", IoC.Resolve<IEntityServices>().FindAll<Thicknes>(new Order[] { OrderBy.Thicknes.Name }));
                    break;
                case "Stove":
                    SetPropertyBagsFor(new Stove(), "Stove", IoC.Resolve<IEntityServices>().FindAll<Stove>(new Order[] { OrderBy.Stove.Name }));
                    break;
                case "Sink":
                    SetPropertyBagsFor(new Sink(), "Sink", IoC.Resolve<IEntityServices>().FindAll<Sink>(new Order[] { OrderBy.Sink.Name }));
                    break;
                case "Project":
                    SetPropertyBagsFor(new Project(), "Project", IoC.Resolve<IEntityServices>().FindAll<Project>(new Order[] { OrderBy.Project.Name }));
                    RenderView("project");
                    break;
                case "Company":
                    PropertyBag["item"] = new Company();
                    PropertyBag["itemTip"] = "Company";
                    RenderView("company");
                    break;
                case "Prices":
                    PropertyBag["pricesFor"] = new string[] {"ColorGroup", "Edge", "Sink", "Stove"/*, "Thicknes"*/};
                    RenderView("Prices");
                    break;
                case "User":
                    PropertyBag["item"] = new User();
                    PropertyBag["itemTip"] = "User";
                    RenderView("user");
                    break;
                default:
                    PropertyBag["colorGroupTypes"] = typeof (ColorGroupTypeEnum);
                    SetPropertyBagsFor(new ColorGroup(), "ColorGroup", IoC.Resolve<IEntityServices>().FindAll<ColorGroup>(new Order[] { OrderBy.ColorGroup.Name }));
                    break;
            }
            PrepareList();
        }

        private void SetPropertyBagsFor<T>(object item,string itemTip,IList<T> parentItems)
        {
            PropertyBag["item"] = item;
            PropertyBag["itemTip"] = itemTip;
            PropertyBag["parentItems"] = parentItems;
        }

        private void PrepareList()
        {
            PropertyBag["colorItems"] = IoC.Resolve<IEntityServices>().FindAll<ColorGroup>(Where.ColorGroup.Parent.IsNull, OrderBy.ColorGroup.Name.Asc);
            PropertyBag["edgeItems"] = IoC.Resolve<IEntityServices>().FindAll<Edge>(Where.Edge.Parent.IsNull, OrderBy.Edge.Name.Asc);
            PropertyBag["stoveItems"] = IoC.Resolve<IEntityServices>().FindAll<Stove>(Where.Stove.Parent.IsNull, OrderBy.Stove.Name.Asc);
            PropertyBag["sinkItems"] = IoC.Resolve<IEntityServices>().FindAll<Sink>(Where.Sink.Parent.IsNull, OrderBy.Sink.Name.Asc);
            PropertyBag["thicknesItems"] = IoC.Resolve<IEntityServices>().FindAll<Thicknes>(Where.Thicknes.Parent.IsNull, OrderBy.Thicknes.Name.Asc);
            PropertyBag["projectItems"] = IoC.Resolve<IEntityServices>().FindAll<Project>(Where.Project.Parent.IsNull, OrderBy.Project.Name);
            PropertyBag["companies"] = IoC.Resolve<IEntityServices>().FindAll<Company>(OrderBy.Company.Name);

            PropertyBag["users"] = IoC.Resolve<IEntityServices>().FindAll<User>(OrderBy.User.Name);
            PropertyBag["roles"] = IoC.Resolve<IEntityServices>().FindAll<Role>(OrderBy.Role.Name);
        }

        public void Edit(int id,string itemTip)
        {
            CancelLayout();
            if(itemTip!=null && id>0)
            {
                object item = null;
                object parentItems = null;

                item = GetItem(itemTip, id, item, ref parentItems);

                PropertyBag["item"] = item;
                PropertyBag["itemTip"] = itemTip;
                PropertyBag["parentItems"] = parentItems;
                PropertyBag["colorGroupTypes"] = typeof(ColorGroupTypeEnum);
            }
            PrepareList();
        }
        public void Delete(int id, string itemTip)
        {
            CancelLayout();
            if (itemTip != null && id > 0)
            {
                object item = null;
                object parentItems = null;

                try
                {
                    item = GetItem(itemTip, id, item, ref parentItems);
                    IoC.Resolve<IEntityServices>().Delete(item);
                    UnitOfWork.CurrentSession.Flush();
                    SetMessage(string.Format("{0} was deleted with success.",item.GetType().GetProperty("Name").GetValue(item,null)));
                }
                catch (Exception e)
                {
                    Flash["error"] = string.Format("{0} couldn't be deleted since is in use by other entities", item!=null?item.GetType().GetProperty("Name").GetValue(item, null):"This");
                    Logger.Error("Error deleting entity",e);
                }
            }
            RedirectToAction("index");
        }

        private object GetItem(string itemTip, int id, object item, ref object parentItems)
        {
            switch (itemTip)
            {
                case "Edge":
                    item = IoC.Resolve<IEntityServices>().GetInstance<Edge>(id);
                    parentItems = IoC.Resolve<IEntityServices>().FindAll<Edge>(OrderBy.Edge.Name);
                    RenderView("_itemContent");
                    break;
                case "Thicknes":
                    item = IoC.Resolve<IEntityServices>().GetInstance<Thicknes>(id);
                    parentItems = IoC.Resolve<IEntityServices>().FindAll<Thicknes>(OrderBy.Thicknes.Name);
                    RenderView("_itemContent");
                    break;
                case "Sink":
                    item = IoC.Resolve<IEntityServices>().GetInstance<Sink>(id);
                    parentItems = IoC.Resolve<IEntityServices>().FindAll<Sink>(OrderBy.Sink.Name);
                    RenderView("_itemContent");
                    break;
                case "Stove":
                    item = IoC.Resolve<IEntityServices>().GetInstance<Stove>(id);
                    parentItems = IoC.Resolve<IEntityServices>().FindAll<Stove>(OrderBy.Stove.Name);
                    RenderView("_itemContent");
                    break;
                case "ColorGroup":
                    item = IoC.Resolve<IEntityServices>().GetInstance<ColorGroup>(id);
                    parentItems = IoC.Resolve<IEntityServices>().FindAll<ColorGroup>(OrderBy.ColorGroup.Name);
                    RenderView("_itemContent");
                    break;
                case "Project":
                    item = IoC.Resolve<IEntityServices>().GetInstance<Project>(id);
                    parentItems = IoC.Resolve<IEntityServices>().FindAll<Project>(OrderBy.Project.Name);
                    RenderView("project");
                    break;
                case "Company":
                    item = IoC.Resolve<IEntityServices>().GetInstance<Company>(id);
                    parentItems = IoC.Resolve<IEntityServices>().FindAll<Company>(OrderBy.Company.Name);
                    RenderView("company");
                    break;
                case "Users":
                    item = IoC.Resolve<IEntityServices>().GetInstance<User>(id);
                    parentItems = IoC.Resolve<IEntityServices>().FindAll<User>(OrderBy.User.Name);
                    RenderView("user");
                    break;
            }
            return item;
        }

        public void AddNewPrice(int id,string itemTip)
        {
            CancelLayout();
            if (itemTip != null && id > 0)
            {
                object item;
                object price;
                switch (itemTip)
                {
                    case "ColorGroup":
                        var coloGroup = IoC.Resolve<IEntityServices>().GetInstance<ColorGroup>(id);
                        price = new ColorGroupPrice { Company = Repository<Company>.FindFirst(), Entity = coloGroup, Project = Repository<Project>.FindFirst() };
                        IoC.Resolve<IEntityServices>().SaveEntity(price);
                        UnitOfWork.CurrentSession.Flush();

                        PropertyBag["item"] = IoC.Resolve<IEntityServices>().GetInstance<ColorGroup>(id);
                        PropertyBag["projectItems"] = IoC.Resolve<IEntityServices>().FindAll<Project>(Where.ColorGroupPrice.Project.IsNull, OrderBy.ColorGroup.Name.Asc);
                        PropertyBag["companies"] = IoC.Resolve<IEntityServices>().FindAll<Company>();

                        break;
                    case "Edge":
                        var edge = IoC.Resolve<IEntityServices>().GetInstance<Edge>(id);
                        price = new EdgePrice { Company = Repository<Company>.FindFirst(), Entity = edge, Project = Repository<Project>.FindFirst() };
                        IoC.Resolve<IEntityServices>().SaveEntity(price);
                        UnitOfWork.CurrentSession.Flush();

                        PropertyBag["item"] = IoC.Resolve<IEntityServices>().GetInstance<Edge>(id);
                        break;
                    case "Sink":
                        var sink = IoC.Resolve<IEntityServices>().GetInstance<Sink>(id);
                        price = new SinkPrice { Company = Repository<Company>.FindFirst(), Entity = sink, Project = Repository<Project>.FindFirst() };
                        IoC.Resolve<IEntityServices>().SaveEntity(price);
                        UnitOfWork.CurrentSession.Flush();

                        PropertyBag["item"] = IoC.Resolve<IEntityServices>().GetInstance<Sink>(id);
                        break;
                    case "Thicknes":
                        var thicknes = IoC.Resolve<IEntityServices>().GetInstance<Thicknes>(id);
                        price = new ThicknesPrice() { Company = Repository<Company>.FindFirst(), Entity = thicknes, Project = Repository<Project>.FindFirst() };
                        IoC.Resolve<IEntityServices>().SaveEntity(price);
                        UnitOfWork.CurrentSession.Flush();

                        PropertyBag["item"] = IoC.Resolve<IEntityServices>().GetInstance<Thicknes>(id);
                        break;
                    case "Stove":
                        var stove = IoC.Resolve<IEntityServices>().GetInstance<Stove>(id);
                        price = new StovePrice() { Company = Repository<Company>.FindFirst(), Entity = stove, Project = Repository<Project>.FindFirst() };
                        IoC.Resolve<IEntityServices>().SaveEntity(price);
                        UnitOfWork.CurrentSession.Flush();

                        PropertyBag["item"] = IoC.Resolve<IEntityServices>().GetInstance<Stove>(id);
                        break;
                    case "Project":
                        var project = IoC.Resolve<IEntityServices>().GetInstance<Project>(id);
                        price = new ProjectPrice() {};
                        IoC.Resolve<IEntityServices>().SaveEntity(price);
                        UnitOfWork.CurrentSession.Flush();

                        PropertyBag["item"] = IoC.Resolve<IEntityServices>().GetInstance<Project>(id);
                        break;
                }
                
            }
            PrepareList();
            RenderView("_priceList");
        }

        public void DeletePrices(string ids)
        {
            if(ids!=null)
            {
                var queryString = new StringBuilder();
                ids = ids.EndsWith(",") ? ids.Remove(ids.LastIndexOf(","), 1) : ids;
                queryString.AppendFormat("DELETE CompanyPrices WHERE Id in ({0})",ids);
                if (queryString.Length > 0)
                {
                    var query = UnitOfWork.CurrentSession.CreateSQLQuery(queryString.ToString());
                    var result = query.ExecuteUpdate();
                }
            }
            RenderText("Deleted with success...");
        }
        public void DeletePrice(int id,string itemTip)
        {
            CancelLayout();
            if (itemTip != null && id > 0)
            {
                switch (itemTip)
                {
                    case "ColorGroup":
                        var colorGroupPrice = IoC.Resolve<IEntityServices>().GetInstance<ColorGroupPrice>(id);
                        IoC.Resolve<IEntityServices>().Delete(colorGroupPrice);
                        UnitOfWork.CurrentSession.Flush();

                        PropertyBag["item"] = IoC.Resolve<IEntityServices>().GetInstance<ColorGroup>(colorGroupPrice.Entity.Id);
                        break;
                    case "Edge":
                        var edgePrice = IoC.Resolve<IEntityServices>().GetInstance<EdgePrice>(id);
                        IoC.Resolve<IEntityServices>().Delete(edgePrice);
                        UnitOfWork.CurrentSession.Flush();

                        PropertyBag["item"] = IoC.Resolve<IEntityServices>().GetInstance<Edge>(edgePrice.Entity.Id);
                        break;
                    case "Sink":
                        var sinkPrice = IoC.Resolve<IEntityServices>().GetInstance<SinkPrice>(id);
                        IoC.Resolve<IEntityServices>().Delete(sinkPrice);
                        UnitOfWork.CurrentSession.Flush();

                        PropertyBag["item"] = IoC.Resolve<IEntityServices>().GetInstance<Sink>(sinkPrice.Entity.Id);
                        break;
                    case "Thicknes":
                        var thicknes = IoC.Resolve<IEntityServices>().GetInstance<ThicknesPrice>(id);
                        IoC.Resolve<IEntityServices>().Delete(thicknes);
                        UnitOfWork.CurrentSession.Flush();

                        PropertyBag["item"] = IoC.Resolve<IEntityServices>().GetInstance<Thicknes>(thicknes.Entity.Id);
                        break;
                    case "Stove":
                        var stovePrice = IoC.Resolve<IEntityServices>().GetInstance<StovePrice>(id);
                        IoC.Resolve<IEntityServices>().Delete(stovePrice);
                        UnitOfWork.CurrentSession.Flush();

                        PropertyBag["item"] = IoC.Resolve<IEntityServices>().GetInstance<Stove>(stovePrice.Entity.Id);
                        break;
                }
            }
            PrepareList();
            RenderView("_priceList");
        }

        public void SaveItem(string itemTip)
        {
            CancelLayout();
            if (itemTip != null)
            {

                switch (itemTip)
                {
                    case "ColorGroup":
                        var item = DataBindFromForm<ColorGroup>("item");
                        
                        if(item.Parent!=null && item.Parent.Id==0) item.Parent = null;
                       
//                        foreach (var price in item.Prices)
//                        {
//                            price.Entity = item;
//                            if (price.Id > 0)
//                                Repository<ColorGroupPrice>.SaveOrUpdate(price);
//                            else
//                                Repository<ColorGroupPrice>.Save(price);
//                        }
                        if(item.Id>0)
                            Repository<ColorGroup>.SaveOrUpdate(item);
                        else
                            Repository<ColorGroup>.Save(item);
                        UnitOfWork.CurrentSession.Flush();
                        PropertyBag["parentItems"] = IoC.Resolve<IEntityServices>().FindAll<ColorGroup>();
                        PropertyBag["item"] = item;
                        PropertyBag["colorGroupTypes"] = typeof(ColorGroupTypeEnum);
                        break;
                    case "Edge":
                        var edgeitem = DataBindFromForm<Edge>("item");

                        if (edgeitem.Parent != null && edgeitem.Parent.Id == 0) edgeitem.Parent = null;

                        foreach (var price in edgeitem.Prices)
                        {
                            price.Entity = edgeitem;
                            if(price.Id>0)
                                Repository<EdgePrice>.SaveOrUpdate(price);
                            else
                                Repository<EdgePrice>.Save(price);
                        }
                        if (edgeitem.Id > 0)
                            Repository<Edge>.SaveOrUpdate(edgeitem);
                        else
                            Repository<Edge>.Save(edgeitem);
                        UnitOfWork.CurrentSession.Flush();
                        PropertyBag["parentItems"] = IoC.Resolve<IEntityServices>().FindAll<Edge>();
                        PropertyBag["item"] = edgeitem;
                        break;
                    case "Stove":
                        var stoveitem = DataBindFromForm<Stove>("item");

                        if (stoveitem.Parent != null && stoveitem.Parent.Id == 0) stoveitem.Parent = null;

                        foreach (var price in stoveitem.Prices)
                        {
                            price.Entity = stoveitem;
                            if(price.Id>0)
                                Repository<StovePrice>.SaveOrUpdate(price);
                            else
                                Repository<StovePrice>.Save(price);
                        }
                        if (stoveitem.Id > 0)
                            Repository<Stove>.SaveOrUpdate(stoveitem);
                        else
                            Repository<Stove>.Save(stoveitem);
                        UnitOfWork.CurrentSession.Flush();
                        PropertyBag["parentItems"] = IoC.Resolve<IEntityServices>().FindAll<Stove>();
                        PropertyBag["item"] = stoveitem;
                        break;
                    case "Sink":
                        var sinkitem = DataBindFromForm<Sink>("item");

                        if (sinkitem.Parent != null && sinkitem.Parent.Id == 0) sinkitem.Parent = null;

                        foreach (var price in sinkitem.Prices)
                        {
                            price.Entity = sinkitem;
                            if(price.Id>0)
                                Repository<SinkPrice>.SaveOrUpdate(price);
                            else
                                Repository<SinkPrice>.Save(price);
                        }
                        if (sinkitem.Id > 0)
                            Repository<Sink>.SaveOrUpdate(sinkitem);
                        else
                            Repository<Sink>.Save(sinkitem);
                        UnitOfWork.CurrentSession.Flush();
                        PropertyBag["parentItems"] = IoC.Resolve<IEntityServices>().FindAll<Sink>();
                        PropertyBag["item"] = sinkitem;
                        break;
                    case "Thicknes":
                        var thicknes = DataBindFromForm<Thicknes>("item");

                        if (thicknes.Parent != null && thicknes.Parent.Id == 0) thicknes.Parent = null;

                        foreach (var price in thicknes.Prices)
                        {
                            price.Entity = thicknes;
                            if(price.Id>0)
                                Repository<ThicknesPrice>.SaveOrUpdate(price);
                            else
                                Repository<ThicknesPrice>.Save(price);
                        }
                        if (thicknes.Id > 0)
                            Repository<Thicknes>.SaveOrUpdate(thicknes);
                        else
                            Repository<Thicknes>.Save(thicknes);
                        UnitOfWork.CurrentSession.Flush();
                        PropertyBag["parentItems"] = IoC.Resolve<IEntityServices>().FindAll<Thicknes>();
                        PropertyBag["item"] = thicknes;
                        break;
                    case "Project":
                        var project = DataBindFromForm<Project>("item");

                        if (project.Parent != null && project.Parent.Id == 0) project.Parent = null;


                        if (project.Id > 0)
                            Repository<Project>.SaveOrUpdate(project);
                        else
                            Repository<Project>.Save(project);
                        UnitOfWork.CurrentSession.Flush();
                        PropertyBag["parentItems"] = IoC.Resolve<IEntityServices>().FindAll<Project>();
                        PropertyBag["item"] = project;
                        break;
                }
                SetMessage("Saved successully.");
            }
            else
            {
                SetErrorMessage("Can't find the data type.");
            }
            
            PrepareList();
            RenderView("_itemContent");
        }

        public void SaveProject([DataBind("item")]Project project)
        {
            CancelLayout();
            if(project!=null)
            {
                try
                {
                    if (project.Parent != null && project.Parent.Id == 0) project.Parent = null;
                    if (project.Id > 0)
                        Repository<Project>.SaveOrUpdate(project);
                    else
                        Repository<Project>.Save(project);
                    UnitOfWork.CurrentSession.Flush();
                    PropertyBag["parentItems"] = IoC.Resolve<IEntityServices>().FindAll<Project>();
                    PropertyBag["item"] = project;
                    SetMessage(string.Format("{0} was successfully saved",project.Name));
                }
                catch (Exception e)
                {
                    SetErrorMessage(e.Message);
                }
            }
            else
            {
                SetErrorMessage("Couldn't load project");
            }
            PrepareList();
            RenderView("_projectForm");
        }

        public void SaveCompany([DataBind("item")]Company company)
        {
            CancelLayout();
            if (company != null)
            {
                try
                {
                    if (company.Id > 0)
                        Repository<Company>.SaveOrUpdate(company);
                    else
                        Repository<Company>.Save(company);
                    UnitOfWork.CurrentSession.Flush();
                    PropertyBag["item"] = company;
                    SetMessage(string.Format("{0} was successfully saved",company.Name));
                }
                catch (Exception e)
                {
                    SetErrorMessage(e.Message);
                }
            }
            else
            {
                SetErrorMessage("Couldn't load Company");
            }
            PrepareList();
            RenderView("_companyForm");
        }
        
        
        public void SaveUser([DataBind("item")]User user)
        {
            CancelLayout();
            if (user != null)
            {
                try
                {
                    if (user.Id > 0)
                        Repository<User>.SaveOrUpdate(user);
                    else
                        Repository<User>.Save(user);
                    UnitOfWork.CurrentSession.Flush();
                    PropertyBag["item"] = user;
                    SetMessage(string.Format("{0} was successfully saved",user.UserName));
                }
                catch (Exception e)
                {
                    SetErrorMessage(e.Message);
                }
            }
            else
            {
                SetErrorMessage("Couldn't load User");
            }
            PrepareList();
            RenderView("_userForm");
        }
        
        private T DataBindFromForm<T>(string prefix)
        {
            var binder = new DataBinder();
            //binder.AutoLoad = AutoLoadBehavior.NewInstanceIfInvalidKey;
            return (T)binder.BindObject(typeof(T), prefix, Request.FormNode);
        }
        
        [AjaxAction]
        public JSONPageResult PriceSearch([DataBind("filter")]PriceFilter filter)
        {
            CancelLayout();
            if(filter==null ||filter.PricesFor==null) return new JSONPageResult();
            Type type = null;
            IList pricesFor = null;
            if (filter.PricesFor == "ColorGroup")
            {
                type = typeof(ColorGroupPrice);
                pricesFor = (IList) IoC.Resolve<IEntityServices>().FindAll<ColorGroup>(Where.ColorGroup.Parent.IsNull,OrderBy.ColorGroup.Name.Asc);
            }
            if (filter.PricesFor == "Sink")
            {
                type = typeof(SinkPrice);
                pricesFor = (IList)IoC.Resolve<IEntityServices>().FindAll<Sink>(Where.Sink.Parent.IsNull, OrderBy.Sink.Name.Asc);
            }
            if (filter.PricesFor == "Edge")
            {
                type = typeof(EdgePrice);
                pricesFor = (IList)IoC.Resolve<IEntityServices>().FindAll<Edge>(Where.Edge.Parent.IsNull, OrderBy.Edge.Name.Asc);
            }
            if (filter.PricesFor == "Stove")
            {
                type = typeof(StovePrice);
                pricesFor = (IList)IoC.Resolve<IEntityServices>().FindAll<Stove>(Where.Stove.Parent.IsNull, OrderBy.Stove.Name.Asc);
            }
            if (filter.PricesFor == "Thicknes")
            {
                type = typeof(ThicknesPrice);
                pricesFor = (IList)IoC.Resolve<IEntityServices>().FindAll<Thicknes>(Where.Thicknes.Parent.IsNull, OrderBy.Thicknes.Name.Asc);
            }

            if (type == null) return null;
            
            
            var jsonPageResult = new JSONPageResult { page = 1, total = pricesFor.Count };
            var i = 0;

            foreach (var priceFor in pricesFor)
            {
                var price = IoC.Resolve<IEntityServices>().FindPricesBy(type, filter,priceFor);
                PriceBase priceBase = null;
                if(price==null || price.Count==0)
                {
                    priceBase = new PriceBase();
                }
                else
                {
                    priceBase = price[0] as PriceBase;
                }

                AddRecord(filter, priceFor, priceBase, i, jsonPageResult);
                i++;
            }

            return jsonPageResult;
        }

        private void AddRecord(PriceFilter filter, object priceFor, PriceBase priceBase, int i, JSONPageResult jsonPageResult)
        {
            var entityName = priceFor.GetType().GetProperty("Name").GetValue(priceFor, null);
            var entityId = priceFor.GetType().GetProperty("Id").GetValue(priceFor, null);
            var supplyPricesProperties = DictHelper.Create("style=width:60px");
            if(filter.PricesFor!="ColorGroup") supplyPricesProperties.Add("disabled","true");
            if(priceBase==null)return;
            var pricePrefix = string.Format("price[{0}]",i);
            jsonPageResult.rows.Add(new Row
                                        {
                                            cell = new []{
                                                             string.Format("{0}",GetFormHelper.HiddenField(pricePrefix+".Id",priceBase.Id)) +
                                                             string.Format("{0}",GetFormHelper.HiddenField(pricePrefix+".EntityId",entityId)) +
                                                             string.Format("{0}",GetAppHelper.TextFieldValue(pricePrefix+".Price",Math.Round(priceBase.Price,2),DictHelper.Create("style=width:60px"))),
                                                             string.Format("{0}",GetAppHelper.TextFieldValue(pricePrefix+".SupplyPrice",Math.Round(priceBase.SupplyPrice,2),supplyPricesProperties)),
                                                             string.Format("{0}",GetAppHelper.TextFieldValue(pricePrefix+".SupplyAndInstallPrice",Math.Round(priceBase.SupplyAndInstallPrice,2),supplyPricesProperties)),
                                                             string.Format("<label>{0}</label>",entityName)
                                                         },
                                            id = priceBase.Id.ToString()
                                        });
            
        }


        public void PriceSave([DataBind("price")]PriceBase[] priceBase,[DataBind("filter")]PriceFilter filter)
        {
            CancelLayout();
            if(priceBase!=null)
            {
                var queryString = new StringBuilder();
                foreach (var price in priceBase)
                {
                    if(price.Id>0)
                        queryString.AppendFormat("UPDATE CompanyPrices Set Price={0},SupplyPrice={1}, SupplyAndInstallPrice={2} Where Id={3};",price.Price, price.SupplyPrice, price.SupplyAndInstallPrice, price.Id);
                    else
                        queryString.AppendFormat("INSERT INTO CompanyPrices (Price,SupplyPrice, SupplyAndInstallPrice,CompanyId,ProjectId,Type,Entity) VALUES ({0},{1},{2},{3},{4},'{5}',{6});",price.Price, price.SupplyPrice, price.SupplyAndInstallPrice,filter.CompanyId,filter.ProjectId,filter.PricesFor,price.EntityId);
                }
                if (queryString.Length > 0)
                {
                    var query = UnitOfWork.CurrentSession.CreateSQLQuery(queryString.ToString());
                    var result = query.ExecuteUpdate();
                }
            }
            RenderText("ok");
        }

        public void SaveNewPrice([DataBind("price")]PriceBase priceBase, string type, int entityId)
        {
            CancelLayout();
            if (priceBase == null || priceBase.Company==null || priceBase.Project==null || type==null || entityId==0) return;
            var queryString = new StringBuilder();
            queryString.AppendFormat("INSERT INTO CompanyPrices (Type,Price,SupplyPrice,SupplyAndInstallPrice,CompanyId,ProjectId,Entity) VALUES ('{0}',{1},{2},{3},{4},{5},{6})",type,priceBase.Price,priceBase.SupplyPrice,priceBase.SupplyAndInstallPrice,priceBase.Company.Id,priceBase.Project.Id,entityId);
            if (queryString.Length > 0)
            {
                var query = UnitOfWork.CurrentSession.CreateSQLQuery(queryString.ToString());
                var result = query.ExecuteUpdate();
            }
            RenderText(string.Format("New Price was added for selected {0}", type));
        }
        
        public void AddNewPrice([DataBind("filter")]PriceFilter filter)
        {
            if (filter != null)
            {
                var table = "";
                if (filter.PricesFor == "ColorGroup") table = "ColorGroups";
                if (filter.PricesFor == "Sink") table = "Sinks";
                if (filter.PricesFor == "Edge") table = "Edges";
                if (filter.PricesFor == "Stove") table = "Stoves";
                if (filter.PricesFor == "Thicknes") table = "Thickneses";

                var sql = string.Format("select color.* from {0} color where ParentId is null group by color.id,color.Name,color.Description,color.ParentId,color.CreatedBy,color.CreatedOn,color.LastUpdatedBy,color.Message,color.LastUpdatedOn having Id not in (select Entity from CompanyPrices where Type='{1}' and ProjectId={2} and CompanyId={3})", table, filter.PricesFor, filter.ProjectId, filter.CompanyId);
                var sqlQuery = UnitOfWork.CurrentSession.CreateSQLQuery(sql,"color",typeof(ColorGroup));
                var entityList = sqlQuery.List();
                if (entityList != null && entityList.Count > 0)
                {
                    PropertyBag["entityList"] = entityList;
                    PropertyBag["company"] = IoC.Resolve<IEntityServices>().GetInstance<Company>(filter.CompanyId);
                    PropertyBag["project"] = IoC.Resolve<IEntityServices>().GetInstance<Project>(filter.ProjectId);
                    PropertyBag["priceFor"] = filter.PricesFor;
                    RenderView("_newPriceForm",true);
                    return;
                }
            }
            CancelLayout();
            RenderText("<div style=\"width:350px\"><div class=\"solid-green\"><strong><div id=\"message\">There are no more prices to be setup for the selected combination.<br>Change your search and try again.</div></strong></div></div>");
        }
    }
}