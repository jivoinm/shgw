using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Castle.ActiveRecord;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using NHibernate.Criterion;
using Query;
using Rhino.Commons;
using StoneHaven.Models;
using StoneHaven.Models.Filters;
using StoneHaven.Models.Services;
using StoneHaven.Web.Forms;
using Winnovative.WnvHtmlConvert;

namespace StoneHaven.Web.Controllers
{
    public class QuoteManagementController : BaseSecuredController
    {
        public void Index()
        {
            PropertyBag["companies"] = IoC.Resolve<IEntityServices>().FindAll<Company>(new Order[] { OrderBy.Company.Name.Asc });
            PropertyBag["cities"] = IoC.Resolve<IEntityServices>().FindAll<City>(new Order[] { OrderBy.City.Name.Asc });
        }

        [AjaxAction]
        public JSONPageResult List([DataBind("filter")]QuoteSearchFilter filter, string sortname, string sortorder, int page, int rp)
        {
            filter.OrderBy = sortname;
            filter.Asc = sortorder == "asc";

            var quotations = IoC.Resolve<IEntityServices>().FindAll<Quotation>(filter, page > 0 ? page - 1 : 0, rp);
            var jsonPageResult = new JSONPageResult { page = page, total = quotations.TotalItems };
            foreach (var quotation in quotations)
            {
                jsonPageResult.rows.Add(new Row
                                            {
                                                cell = new[]
                                                           {
                                                               string.Format("{0}",RenderLinkToAction("Edit","EditQuote","Id="+quotation.Id)) +
                                                               string.Format("{0}",RenderLinkToAction("Delete","DeleteQuote","Id="+quotation.Id)),
                                                               quotation.QuoteNr.ToString(),
                                                               quotation.Company.Name,
                                                               quotation.Customer.CustomerName,
                                                               
                                                          },
                                                id = quotation.Id.ToString()
                                            });
            }

            return jsonPageResult;
        }
        
        [AjaxAction]
        public Company GetCompanyInfo(int companyId)
        {
            Company company = null;
            if (companyId > 0) company = IoC.Resolve<IEntityServices>().GetInstance<Company>(companyId);

            return company;
        }

        public void AddNew()
        {
            PropertyBag["quote"] = new Quotation();

            QuoteSetup();

            RenderView("quotationForm");
        }

        public void AddNewCity([DataBind("city",Validate = true)]City city)
        {
            if(HasValidationError(city))
            {
                RenderText("Error SAving City");
                return;
            }

            Repository<City>.SaveOrUpdate(city);
            UnitOfWork.CurrentSession.Flush();
            var cities = Repository<City>.FindAll(OrderBy.City.Name);
            var select = GetFormHelper.Select("quote.Customer.City.Id", cities,DictHelper.Create("text=Name", "value=Id", "style=width:250px"));
           RenderText(select);
        }

        private void QuoteSetup()
        {
            PropertyBag["companies"] = IoC.Resolve<IEntityServices>().FindAll<Company>(new Order[] {OrderBy.Company.Name.Asc});
            PropertyBag["cities"] = IoC.Resolve<IEntityServices>().FindAll<City>(new Order[] { OrderBy.City.Name.Asc });
            PropertyBag["projects"] = IoC.Resolve<IEntityServices>().FindAll<Project>(new Order[]{OrderBy.Project.Name.Asc});
            PropertyBag["colorGroups"] = IoC.Resolve<IEntityServices>().FindAll<ColorGroup>(Where.ColorGroup.Parent==null,OrderBy.ColorGroup.Name.Asc);
            PropertyBag["edges"] = IoC.Resolve<IEntityServices>().FindAll<Edge>(Where.Edge.Parent==null, OrderBy.ColorGroup.Name.Asc);
            PropertyBag["thickneses"] = IoC.Resolve<IEntityServices>().FindAll<Thicknes>(Where.Thicknes.Parent==null, OrderBy.ColorGroup.Name.Asc);
            PropertyBag["sinks"] = IoC.Resolve<IEntityServices>().FindAll<Sink>(Where.Sink.Parent==null, OrderBy.ColorGroup.Name.Asc);
            PropertyBag["stoves"] = IoC.Resolve<IEntityServices>().FindAll<Stove>(Where.Stove.Parent==null, OrderBy.ColorGroup.Name.Asc);
            PropertyBag["webSite"] = string.Format("http://{0}:{1}",Request.Params["SERVER_NAME"], Request.Params["SERVER_PORT"]);
        }

//        public void SaveQuote([DataBind("quote", Validate = true)]Quotation quotation)
//        {
//            if (HasValidationError(quotation))
//            {
//                RenderText("Error validating the form");
//                return;
//            }
//            
//            if(quotation.Company.Id==0)
//            {
//                RenderText("Please select the company");
//                return;
//            }
//
//            Repository<Quotation>.SaveOrUpdate(quotation);
//            UnitOfWork.CurrentSession.Flush();
//            PropertyBag["quote"] = quotation;
//            //QuoteSetup();
//            CancelLayout();
//            RenderText(quotation.Id.ToString());
//        }

        public void SaveQuoteItems([DataBind("quote.QuoteItems")]QuoteItem[] items)
        {
            int quoteId = 0;

            if(items!=null)
            {
                foreach (var item in items)
                {
                    if (item.Quotation!=null && item.Quotation.Id>0)
                        quoteId = item.Quotation.Id;
                    
                    Repository<QuoteItem>.SaveOrUpdate(item);
                }
                UnitOfWork.Current.Flush();
            }
            if (quoteId > 0)
                PropertyBag["quote"] = Repository<Quotation>.Get(quoteId);
            QuoteSetup();
            RenderView("_projectList",true);
        }

        public void SaveQuote([DataBind("quote")]Quotation quotation, int[] colorGroupId)
        {
            if (HasValidationError(quotation))
            {
                //SetErrorMessages(GetErrorSummary(quotation));
                RenderText("Error validating the form");
                return;
            }

            if(quotation.Company.Id==0)
            {
                RenderText("Please select the company");
                return;
            }
            
            IoC.Resolve<IQuoteServices>().SaveQoute(quotation,colorGroupId);
            UnitOfWork.CurrentSession.Flush();
            PropertyBag["quote"] = quotation;
            QuoteSetup();
            CancelLayout();
            RenderText(quotation.Id.ToString());
        }
        
        public void EditQuote(int id)
        {
            if (id == 0)
            {
                SetErrorMessage("Invalid Quote");
                RedirectToAction("Index");
                return;
            }
            var quotation = IoC.Resolve<IEntityServices>().GetInstance<Quotation>(id);
            PropertyBag["quote"] = quotation;
            QuoteSetup();
            PropertyBag["tab"] = !string.IsNullOrEmpty(Request["tab"]) ? Request["tab"] : "0";
            RenderView("quotationForm");
        }

        [AjaxAction]
        public void AddProjectToQuote([ARFetch("quoteId",Create = false)]Quotation quotation)
        {
            CancelLayout();
            using(new SessionScope(FlushAction.Never))
            {
                if (quotation != null)
                {
                    quotation.QuoteItems.Insert(0, new QuoteItem { Project = new Project { Name = "New Entry" }, ColorGroup = new ColorGroup(), Edge = new Edge(), Sink = new Sink(), Thicknes = new Thicknes() });
                    PropertyBag["quote"] = quotation;
                }
                QuoteSetup();
                RenderView("_projectList");
            }
        }
   
        public void DeleteQuoteItem(int id)
        {
           int quoteationId=0;
           if(id>0)
            {
                var item = IoC.Resolve<IEntityServices>().GetInstance<QuoteItem>(id);
                
                quoteationId = item.Quotation.Id;
                IoC.Resolve<IEntityServices>().Delete(item);
                UnitOfWork.CurrentSession.Flush();
            }
           RedirectToAction("EditQuote", "id=" + quoteationId);
        }

        public void DeleteQuote(int id)
        {
           if(id>0)
            {
               var item = IoC.Resolve<IEntityServices>().GetInstance<Quotation>(id);
               var job = IoC.Resolve<IEntityServices>().FindAll<Job>(Where.Job.Quotation == item);
               if (job != null && job.Count==1)
               {
                   job[0].Deleted = true;
                   IoC.Resolve<IEntityServices>().Delete(job[0]);
               }
               IoC.Resolve<IEntityServices>().Delete(item);
               UnitOfWork.CurrentSession.Flush();
            }
           RedirectToAction("index");
        }

        //[Layout("print")]
        public void PrintPreview(int quoteId)
        {
            CancelLayout();
            CancelView();
            QuoteSetup();
            if(quoteId>0)
            {
                var quote = IoC.Resolve<IEntityServices>().GetInstance<Quotation>(quoteId);
                PropertyBag["quote"] = quote;
                PropertyBag["dateNow"] = DateTime.Now.ToString("MMMM, dd, yyyy");
            }
            byte[] pdf;
            using (StringWriter sw = new StringWriter())
            {
                InPlaceRenderSharedView(sw, "QuoteManagement/print/_printPreview");
                PdfConverter pdfc = new PdfConverter();
                //pdfc.PageWidth = 11222;
                pdfc.PdfDocumentOptions.AutoSizePdfPage = true;
                pdfc.PdfDocumentOptions.PdfPageSize = PdfPageSize.Custom;
                pdfc.PdfDocumentOptions.CustomPdfPageSize = new System.Drawing.SizeF(UnitsConverter.PixelsToPoints(1250), UnitsConverter.PixelsToPoints(1200));
                pdfc.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
                pdfc.PdfDocumentOptions.PdfPageOrientation = PDFPageOrientation.Landscape;
                pdfc.PdfDocumentOptions.FitWidth = true;
                pdfc.PdfDocumentOptions.LeftMargin = 40;
                //pdfc.PdfDocumentOptions.SinglePage = true;
                pdfc.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.NoCompression;
                //pdfc.PdfDocumentOptions.GenerateSelectablePdf = true;
                
                pdfc.LicenseKey = "IwgRAxIDERoDFw0TAxASDRIRDRoaGho=";
                
                pdf = pdfc.GetPdfBytesFromHtmlString(sw.ToString());
            }


            Response.ContentType = "application/pdf";
            Response.BinaryWrite(pdf);
        }

       
        public void LoadProjectMeasurement(int quoteItemId,int index)
        {
            var item = Repository<QuoteItem>.Get(quoteItemId);
            PropertyBag["item"] = item;
            PropertyBag["index"] = index;
            RenderView("_projectMeasurements",true);
            
        }

        
        public void CheckIfCustomerExist([DataBind("quote.Customer")]Customer customer)
        {
            var jsonResp = new StringBuilder();
            jsonResp.Append("{");
            if(customer!=null)
            {
                var customers = Repository<Customer>.FindAll(
                    (Where.Customer.CustomerName.Like(customer.CustomerName, MatchMode.Anywhere) &&
                    Where.Customer.Address.Like(customer.Address,MatchMode.Anywhere) &&
                    Where.Customer.PostalCode.Like(customer.PostalCode,MatchMode.Anywhere)) ||
                    Where.Customer.Phone.Like(customer.Phone));
                
                if(customers.Count>0)
                    jsonResp.AppendFormat("exists:true");
                else
                    jsonResp.AppendFormat("exists:false");

            }
            jsonResp.Append("}");
            RenderText(jsonResp.ToString());
        }
    }
}