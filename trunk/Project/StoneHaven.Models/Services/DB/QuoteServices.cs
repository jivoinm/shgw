using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Criterion;
using Query;
using Rhino.Commons;
using Castle.Core.Logging;
using NHibernate.SqlCommand;

namespace StoneHaven.Models.Services.DB
{
    public class QuoteServices : IQuoteServices
    {
        public Quotation FindQuote(int id)
        {
            return Repository<Quotation>.Get(id);
        }

        public decimal CalculateQuoteTotal(Quotation quote)
        {
            decimal total = 0;
            if(quote==null) return total;
            foreach (var quoteItem in quote.QuoteItems)
            {
                QuoteItemTotal itemTotal = CalculateProjectTotal(quoteItem);
                total += itemTotal.Total;
            }

            total = Math.Round(total, 2);
            return total;
        }

        public IDictionary<string, string> CalculatePricesForAllColorGroups(QuoteItem quoteItem, ColorGroupTypeEnum type)
        {
            var colorGroups = IoC.Resolve<IEntityServices>().FindAll<ColorGroup>(Where.ColorGroup.Parent == null && Where.ColorGroup.ColorGroupType==type, OrderBy.ColorGroup.Name.Asc);
            var totalList =  new SortedList<string,string>();
            foreach (var colorGroup in colorGroups)
            {
                quoteItem.ColorGroup = colorGroup;
                var total = CalculateProjectTotal(quoteItem);
                totalList.Add(colorGroup.Name,total.Total.ToString("c"));
            }
            return totalList;
        }

        public QuoteItemTotal CalculateProjectTotal(QuoteItem quoteItem)
        {
                decimal total = 0;
                var itemTotal = new QuoteItemTotal();
                if (quoteItem == null || quoteItem.Quotation==null) return itemTotal;
                //calculate mesasurements
                var totalCounterSqF = GetTotalSqFt(quoteItem.CounterMeasurements);
                var totalBackSplashSqF = GetTotalSqFt(quoteItem.BackSplashMeasurements); ;
                var totalEdgeSqF = GetTotalSqFt(quoteItem.EdgeMeasurements); ;

                itemTotal.TotalSqFt =  totalBackSplashSqF + totalCounterSqF;

                if (quoteItem.ColorGroup!=null)
                {
                    var priceBase = GetProjectBasePrice<ColorGroup>(quoteItem.Project,quoteItem.Quotation.Company,quoteItem.ColorGroup);
                    if (priceBase!=null && priceBase.Length == 3)
                    {
                        itemTotal.SqFt_ColorgroupPrice = (decimal)priceBase[0];
                        itemTotal.SqFt_ColorgroupTotal = (decimal)totalCounterSqF;
                        itemTotal.Total_Colorgroup = Math.Round(itemTotal.SqFt_ColorgroupTotal * itemTotal.SqFt_ColorgroupPrice, 2);
                        total += itemTotal.Total_Colorgroup;
                        itemTotal.Total_SuplayAndInstall = quoteItem.SupplyType == 0 ? (decimal) priceBase[1]: (decimal) priceBase[2];
                        total += itemTotal.Total_SuplayAndInstall;    
                    }
                }

                if (totalBackSplashSqF > 0)
                {
                    itemTotal.SqFt_BackSplashTotal = (decimal) totalBackSplashSqF;
                    itemTotal.Total_Backsplash = Math.Round(itemTotal.SqFt_BackSplashTotal * itemTotal.SqFt_ColorgroupPrice,2);
                    total += itemTotal.Total_Backsplash;
                }

                if (quoteItem.Edge != null)
                {
                    var edgePrice = GetProjectBasePrice<Edge>(quoteItem.Project, quoteItem.Quotation.Company, quoteItem.Edge);
                    if (edgePrice != null && edgePrice.Length == 3)
                        itemTotal.SqFt_EdgePrice = (decimal) edgePrice[0];
                    itemTotal.SqFt_EdgeTotal = (decimal)totalEdgeSqF;
                    itemTotal.Total_Edge = Math.Round(itemTotal.SqFt_EdgeTotal * itemTotal.SqFt_EdgePrice,2);
                    total += itemTotal.Total_Edge; 
                }

                if (quoteItem.Stove!=null)
                {
                    var stovePrice = GetProjectBasePrice<Stove>(quoteItem.Project, quoteItem.Quotation.Company, quoteItem.Stove);
                    if (stovePrice != null && stovePrice.Length == 3)
                        itemTotal.Total_Stove = (decimal) stovePrice[0];
                    total += itemTotal.Total_Stove;
                }

                if (quoteItem.Sink != null)
                {
                    var sinkPrice = GetProjectBasePrice<Sink>(quoteItem.Project, quoteItem.Quotation.Company, quoteItem.Sink);
                    if (sinkPrice != null && sinkPrice.Length == 3)
                        itemTotal.SinkPrice = (decimal) sinkPrice[0];
                    itemTotal.Total_NrOfSinks = quoteItem.NumberOfSinks;
                    itemTotal.Total_Sink = Math.Round(quoteItem.NumberOfSinks*itemTotal.SinkPrice,2);
                    total += itemTotal.Total_Sink;
                }

                if (quoteItem.Project.BumpoutPrice != null)
                {
                    itemTotal.Total_NrOfBumpouts = quoteItem.Bumpouts;
                    itemTotal.BumpoutPrice = Math.Round(quoteItem.Project.BumpoutPrice.Price,2);
                    itemTotal.Total_Bumpouts = Math.Round(itemTotal.BumpoutPrice*itemTotal.Total_NrOfBumpouts,2);
                    total += itemTotal.Total_Bumpouts;
                }

                if (quoteItem.Project.ArchePrice != null)
                {
                    itemTotal.Total_NrOfArches = quoteItem.Arches;
                    itemTotal.ArchePrice = quoteItem.Project.ArchePrice.Price;
                    itemTotal.Total_Arches = Math.Round(itemTotal.Total_NrOfArches*itemTotal.ArchePrice,2);
                    total += itemTotal.Total_Arches;
                }

//                if(quoteItem.Thicknes!=null)
//                {
//                    itemTotal.ThicknesPrice = GetProjectPrice<Thicknes>(quoteItem.Project, quoteItem.Quotation.Company, quoteItem.Thicknes);
//                    itemTotal.Total_Thicknes = Math.Round(itemTotal.ThicknesPrice, 2);
//                    total += itemTotal.Total_Thicknes;
//                }


                
                itemTotal.Total_Adjustment = quoteItem.Adjustment;
                total -= quoteItem.Adjustment;

            total = Math.Round(total, 2);
            itemTotal.Total = total;
            return itemTotal;
        }

        private static double GetTotalSqFt<T>(IList<T> item)
        {
            double total = 0;
            foreach (var measurement in item)
            {
                if (Equals(measurement, default(T))) continue;
                var res = measurement.GetType().InvokeMember("GetTotalSqFt", BindingFlags.InvokeMethod, null, measurement,null);
                if (res != null)
                    total += (double) res;
            }
            return total;
        }

        private static object[] GetProjectBasePrice<T>(Project prj,Company company,object entity)
        {
            object[] result = null;
            try
            {
                if (prj != null && prj.Id > 0)
                {
                    var identifier = entity as ParentIdentifier<T>;
                    if(identifier==null) return null;

                    var priceQuery = UnitOfWork.CurrentSession.CreateSQLQuery(string.Format("select Price, SupplyPrice, SupplyAndInstallPrice From CompanyPrices Where Type='{0}' and ProjectId={1} and CompanyId={2} and Entity={3}",typeof (T).Name, prj.Id, company.Id, identifier.Id));
                    var price = priceQuery.List();
                    if (price != null && price.Count > 0)
                        result = (object[]) price[0];
                    if(prj.Parent!=null)
                     result =  GetProjectBasePrice<T>(prj.Parent, company, identifier);
                    if (!Equals(identifier.Parent, default(T)))
                     result =  GetProjectBasePrice<T>(prj, company, identifier.Parent);


                }
            }
            catch (Exception e)
            {
                IoC.Resolve<ILogger>().Error("Error getting the price ",e);
            }
            return result;
        }
//        private static decimal GetProjectPrice<T>(Project prj,Company company,object entity)
//        {
//            var priceBase = GetProjectBasePrice<T>(prj, company, entity);
//            return priceBase != null ? (decimal) priceBase[0] : 0; 
//        }

        public void SaveQoute(Quotation quotation,int[] colorIds)
        {
            quotation.Date = DateTime.Now;
            if (quotation.Customer.Id > 0)
                Repository<Customer>.SaveOrUpdate(quotation.Customer);
            else
                Repository<Customer>.Save(quotation.Customer);

            for (int i = 0; i < quotation.QuoteItems.Count; i++)
            {
                quotation.QuoteItems[i].Quotation = quotation;
                if (colorIds.Length>=i && colorIds[i]>0)
                    quotation.QuoteItems[i].ColorGroup = Repository<ColorGroup>.Get(colorIds[i]);

                if (quotation.QuoteItems[i].Id > 0)
                {
                    Repository<QuoteItem>.SaveOrUpdate(quotation.QuoteItems[i]);
                }
                else
                    Repository<QuoteItem>.Save(quotation.QuoteItems[i]);
            }
            foreach (var qouteItem in quotation.QuoteItems)
            {
                qouteItem.Quotation = quotation;
                if(qouteItem.Id>0)
                {
                    Repository<QuoteItem>.SaveOrUpdate(qouteItem);
                }
                else
                    Repository<QuoteItem>.Save(qouteItem);
            }
            quotation.QuoteNr = quotation.QuoteNr == 0 ? CreateQuoteNumber() : quotation.QuoteNr;
            if(quotation.Id>0)
                Repository<Quotation>.SaveOrUpdate(quotation);
            else
                Repository<Quotation>.Save(quotation);
        }

     
        private static long CreateQuoteNumber()
        {
            long maxNr = UnitOfWork.CurrentSession.CreateCriteria(typeof (Quotation))
                .SetProjection(Projections.Max("QuoteNr")).UniqueResult<long>();
            return maxNr + 1;
        }
    }
}