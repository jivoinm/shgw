using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using Query;
using Rhino.Commons;
using StoneHaven.Models;
using StoneHaven.Models.Services;
using StoneHaven.Web.Forms;

namespace StoneHaven.Web.Controllers
{
    public class QuoteMeasurementsController : BaseSecuredController
    {
        int counter = 0;
        #region Counter


        public void AddNewCounterMeasurement(int quoteItem,int index)
        {
            CancelLayout();
            if (quoteItem > 0)
            {
                var item = IoC.Resolve<IEntityServices>().GetInstance<QuoteItem>(quoteItem);
                var measurement = new CounterMeasurement {QuoteItem = item};
                IoC.Resolve<IEntityServices>().SaveEntity(measurement);
                UnitOfWork.CurrentSession.Flush();
                PropertyBag["index"] = index;
                PropertyBag["qid"] = quoteItem;
            }
            //RenderView("_counterList");
            RenderText("Success");
        }
        
        
        public void AddNewBSMeasurement(int quoteItem,int index)
        {
            CancelLayout();
            if (quoteItem > 0)
            {
                var item = IoC.Resolve<IEntityServices>().GetInstance<QuoteItem>(quoteItem);
                var measurement = new BackSplashMeasurement {QuoteItem = item};
                IoC.Resolve<IEntityServices>().SaveEntity(measurement);
                UnitOfWork.CurrentSession.Flush();
                PropertyBag["index"] = index;
                PropertyBag["qid"] = quoteItem;
            }
            //RenderView("_backsplashList");
            RenderText("Success");
        }
        
        public void AddNewEdgeMeasurement(int quoteItem,int index)
        {
            CancelLayout();
            if (quoteItem > 0)
            {
                var item = IoC.Resolve<IEntityServices>().GetInstance<QuoteItem>(quoteItem);
                var measurement = new EdgeMeasurement {QuoteItem = item};
                IoC.Resolve<IEntityServices>().SaveEntity(measurement);
                UnitOfWork.CurrentSession.Flush();
                PropertyBag["index"] = index;
                PropertyBag["qid"] = quoteItem;
            }
            //RenderView("_backsplashList");
            RenderText("Success");
        }

        public void SaveMeasurements([DataBind("quote")]Quotation quote, int index, int qid, int[] colorGroupId)
        {
            CancelLayout();
            if(quote!=null && quote.QuoteItems!=null)
            {
                var itemIndex = 0;
                foreach (var item in quote.QuoteItems)
                {
                    item.Quotation = quote;
                    if (colorGroupId.Length >= itemIndex && colorGroupId[itemIndex] > 0)
                        item.ColorGroup = Repository<ColorGroup>.Get(colorGroupId[itemIndex]);

                    item.ColorGroup.Id = colorGroupId[itemIndex];
                    IoC.Resolve<IEntityServices>().SaveEntity(item);
                    if(item.CounterMeasurements!=null)
                    {
                        foreach (var measurement in item.CounterMeasurements)
                        {
                            IoC.Resolve<IEntityServices>().SaveEntity(measurement);
                        }
                    }

                    if(item.BackSplashMeasurements!=null)
                    {
                        foreach (var measurement in item.BackSplashMeasurements)
                        {
                            IoC.Resolve<IEntityServices>().SaveEntity(measurement);
                        }
                    }

                    if (item.EdgeMeasurements == null) continue;
                    foreach (var measurement in item.EdgeMeasurements)
                    {
                        IoC.Resolve<IEntityServices>().SaveEntity(measurement);
                    }
                }

            }
            UnitOfWork.CurrentSession.Flush();
            PropertyBag["index"] = index;
            PropertyBag["qid"] = qid;
            RenderView("_counterList");
        }
        
        public void SaveBSMeasurements([DataBind("bs")]BackSplashMeasurement[] measurements)
        {
            CancelLayout();
            if (measurements != null && measurements.Length>0)
            {
                foreach (var measurement in measurements)
                {
                    IoC.Resolve<IEntityServices>().SaveEntity(measurement);
                }
                UnitOfWork.CurrentSession.Flush();
                PropertyBag["index"] = measurements[0].QuoteItem.Id;
            }
            RenderView("_backsplashList");
        }
        
        public void SaveEdgeMeasurements([DataBind("edge")]EdgeMeasurement[] measurements)
        {
            CancelLayout();
            if (measurements != null && measurements.Length>0)
            {
                foreach (var measurement in measurements)
                {
                    IoC.Resolve<IEntityServices>().SaveEntity(measurement);
                }
                UnitOfWork.CurrentSession.Flush();
                PropertyBag["index"] = measurements[0].QuoteItem.Id;
            }
            RenderView("_backsplashList");
        }
        #endregion

        
        public void GetQuoteItemTotal(int itemId,int index)
        {
            CancelLayout();
            var total = new QuoteItemTotal();
            if(itemId>0)
            {
                var item = IoC.Resolve<IEntityServices>().GetInstance<QuoteItem>(itemId);
                PropertyBag["item"] = item;
                PropertyBag["index"] = index;
            }
            
            RenderView("_measurementTotal");
        }

        #region Lists

        [AjaxAction]
        public JSONPageResult ListCounterMeasurements(int quoteId,int index)
        {
            var counterMeasurements = IoC.Resolve<IEntityServices>().FindAll<CounterMeasurement>(Where.CounterMeasurement.QuoteItem.Id == quoteId);
            var jsonPageResult = new JSONPageResult { page = 1, total = counterMeasurements.Count };
            
            IDictionary textBoxProp = DictHelper.Create("style=width:40px");
            foreach (var measurement in counterMeasurements)
            {
                var prefix = string.Format("quote.QuoteItems[{0}].CounterMeasurements[{1}]", index, counter);
                jsonPageResult.rows.Add(new Row
                                            {
                                                cell = new string[]
                                                           {
                                                               GetFormHelper.HiddenField(prefix+".Id",measurement.Id)+
                                                               GetFormHelper.HiddenField(prefix+".QuoteItem.Id",measurement.QuoteItem.Id)+
                                                               GetFormHelper.TextFieldValue(prefix+".Length",measurement.Length,textBoxProp),
                                                               GetFormHelper.TextFieldValue(prefix+".Width",measurement.Width,textBoxProp),
                                                               string.Format("{0}", measurement.GetTotalSqFt())   
                                                           }
                                            }
                    );
                counter++;
            }

            if(jsonPageResult.rows.Count>0)
                jsonPageResult.rows.Add(new Row
                                            {
                                                cell = new string[]
                                                           {
                                                               "", "Total", GetCounterMeasurementTotalSqFt(counterMeasurements)
                                                            }
                                            });
            return jsonPageResult;
        }

      

        [AjaxAction]
        public JSONPageResult ListBacksplashMeasurements(int quoteId,int index)
        {
            var backSplashMeasurements = IoC.Resolve<IEntityServices>().FindAll<BackSplashMeasurement>(Where.BackSplashMeasurement.QuoteItem.Id == quoteId);
            var jsonPageResult = new JSONPageResult { page = 1, total = backSplashMeasurements.Count };
           // var counter = 0;
            IDictionary textBoxProp = DictHelper.Create("style=width:40px");
            foreach (BackSplashMeasurement measurement in backSplashMeasurements)
            {
                var prefix = string.Format("quote.QuoteItems[{0}].BackSplashMeasurements[{1}]", index, counter);
                jsonPageResult.rows.Add(new Row
                                            {
                                                cell = new string[]
                                                           {
                                                               GetFormHelper.HiddenField(prefix+".Id",measurement.Id)+
                                                               GetFormHelper.HiddenField(prefix+".QuoteItem.Id",measurement.QuoteItem.Id)+
                                                               GetFormHelper.TextFieldValue(prefix+".Length",measurement.Length,textBoxProp),
                                                               GetFormHelper.TextFieldValue(prefix+".Width",measurement.Width,textBoxProp),
                                                               string.Format("{0}", measurement.GetTotalSqFt())
                                                           }
                                            });
                counter++;
            }
            if (jsonPageResult.rows.Count > 0)
                jsonPageResult.rows.Add(new Row
                {
                    cell = new string[]
                                                           {
                                                               "", "Total", GetBSMeasurementTotalSqFt(backSplashMeasurements)
                                                            }
                });

            return jsonPageResult;
        }

        [AjaxAction]
        public JSONPageResult ListEdgeMeasurements(int quoteId,int index)
        {
            var edgeMeasurements = IoC.Resolve<IEntityServices>().FindAll<EdgeMeasurement>(Where.EdgeMeasurement.QuoteItem.Id == quoteId);
            var jsonPageResult = new JSONPageResult { page = 1, total = edgeMeasurements.Count };
            //var counter = 0;
            IDictionary textBoxProp = DictHelper.Create("style=width:40px");
            foreach (EdgeMeasurement measurement in edgeMeasurements)
            {
                var prefix = string.Format("quote.QuoteItems[{0}].EdgeMeasurements[{1}]", index, counter);
                jsonPageResult.rows.Add(new Row
                {
                    cell = new string[]
                                                           {
                                                               GetFormHelper.HiddenField(prefix+".Id",measurement.Id)+
                                                               GetFormHelper.HiddenField(prefix+".QuoteItem.Id",measurement.QuoteItem.Id)+
                                                               GetFormHelper.TextFieldValue(prefix+".Length",measurement.Length,textBoxProp),
                                                               string.Format("{0}", measurement.GetTotalSqFt())
                                                           }
                });
                counter++;
            }
            if (jsonPageResult.rows.Count > 0)
                jsonPageResult.rows.Add(new Row
                {
                    cell = new string[]
                                                           {
                                                               "Total", GetEdgeMeasurementTotalSqFt(edgeMeasurements)
                                                            }
                });
            return jsonPageResult;
        }


        private static string GetCounterMeasurementTotalSqFt(IEnumerable<CounterMeasurement> measurements)
        {
            double total = 0;
            foreach (var measurement in measurements)
            {
                total += measurement.GetTotalSqFt();
            }
            return total.ToString();
        }

        private static string GetBSMeasurementTotalSqFt(IEnumerable<BackSplashMeasurement> measurements)
        {
            double total = 0;
            foreach (var measurement in measurements)
            {
                total += measurement.GetTotalSqFt();
            }
            return total.ToString();
        }

        private static string GetEdgeMeasurementTotalSqFt(IEnumerable<EdgeMeasurement> measurements)
        {
            double total = 0;
            foreach (var measurement in measurements)
            {
                total += measurement.GetTotalSqFt();
            }
            return total.ToString();
        }
        #endregion


    }
}