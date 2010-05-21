using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NUnit.Framework;
using Query;
using Rhino.Commons;
using StoneHaven.Models.Filters;
using StoneHaven.Models.Services;

namespace StoneHaven.Models.Tests
{
    [TestFixture]
    public class QuotationModelTest : QuotationHelper
    {
        [Test]
        public void CreateQuotationModel()
        {
            var company1 = CreateCompany("Company1");
            var quotation = GetQuotation(company1);
            var savedQoutation = IoC.Resolve<IQuoteServices>().FindQuote(1);

            AssertQuotationWasSaved(quotation, savedQoutation);
            //Verify Color group
            AssertColorGroupWasSaved(quotation, savedQoutation);

            //Verify Edge
            AssertEdgeWasSaved(quotation, savedQoutation);

            //Virify Thicknes
            AssertThicknesWasSaved(quotation, savedQoutation);

            //Virify Sink
            AssertSinkWasSaved(quotation, savedQoutation);
        }

        [Test]
        public void SearchQuoteByNr()
        {
            CreateQuotationModel();
            var comp = Repository<Company>.FindAll(Where.Company.Name.Like("Company", MatchMode.Anywhere));
            Assert.IsNotNull(comp);
            var res = IoC.Resolve<IEntityServices>().FindAll<Quotation>(new QuoteSearchFilter
                                                                                         {
                                                                                            CustomerName = "Cust"
                                                                                         }
                                                                                         ,0,10,null);
            Assert.AreEqual(1,res.TotalItems);
        }

        [Test]
        public void TestTheQoutNumberWasGeneratedCorrectlyAndIsUnique()
        {
            var company1 = CreateCompany("Company1");
            var company2 = CreateCompany("Company2");


            var quotation1 = GetQuotation(company1);
            var quotation2 = GetQuotation(company2);
            var savedQoutation1 = IoC.Resolve<IQuoteServices>().FindQuote(1);
            var savedQoutation2 = IoC.Resolve<IQuoteServices>().FindQuote(2);
            Assert.AreEqual(1,savedQoutation1.QuoteNr);
            Assert.AreEqual(2,savedQoutation2.QuoteNr);
        }

       
        [Test]
        public void CalculateQuoteTotalForCompany1()
        {
            var company1 = CreateCompany("Company1");
            var quote = GetQuotation(company1);
            Assert.IsNotNull(quote.Company);
            Assert.IsNotNull(quote.Customer);
            Assert.IsNotNull(quote.QuoteItems[0].Project);
            Assert.IsNotNull(quote.QuoteItems[0].ColorGroup);
            Assert.IsNotNull(quote.QuoteItems[0].Edge);
            Assert.IsNotNull(quote.QuoteItems[0].Sink);
            Assert.IsNotNull(quote.QuoteItems[0].Thicknes);

            //Set Base Project Prices
            CreateProjectPriceFor(quote, "BumpoutPrice", new Bumpout { Price = 10 });
            CreateProjectPriceFor(quote, "ArchePrice", new Arche { Price = 10 });
            CreateProjectPriceFor(quote, "SupplyPrice", new Supply { Price = 10 });
            CreateProjectPriceFor(quote, "SupplyAndInstallPrice", new SupplyAndInstall { Price = 10 });

            quote.QuoteItems[0].NumberOfSinks = 1;
            quote.QuoteItems[0].Bumpouts = 1;
            quote.QuoteItems[0].Arches = 1;
            //quote.QuoteItems[0].SupplyType = SupplyEnum.Supply;
            quote.QuoteItems[0].SupplyType = 0;

            quote.QuoteItems[0].CounterMeasurements.Add(new CounterMeasurement{Length = 100,Width = 100});
            quote.QuoteItems[0].BackSplashMeasurements.Add(new BackSplashMeasurement { Length = 100, Width = 100 });
            quote.QuoteItems[0].EdgeMeasurements.Add(new EdgeMeasurement { Length = 100 });
            foreach (var item in quote.QuoteItems)
            {
                IoC.Resolve<IEntityServices>().SaveEntity(item);
                if (item.CounterMeasurements != null)
                {
                    foreach (var measurement in item.CounterMeasurements)
                    {
                        IoC.Resolve<IEntityServices>().SaveEntity(measurement);
                    }
                }

                if (item.BackSplashMeasurements != null)
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
            IoC.Resolve<IQuoteServices>().SaveQoute(quote, new[] { 1 });
            UnitOfWork.CurrentSession.Flush();
            var savedQuotation = IoC.Resolve<IQuoteServices>().FindQuote(1);
            var total = IoC.Resolve<IQuoteServices>().CalculateQuoteTotal(savedQuotation);
            Assert.AreEqual(1, savedQuotation.QuoteItems[0].CounterMeasurements.Count);
            Assert.AreEqual(new decimal(448.75), total);
            quote.QuoteItems[0].Adjustment = 100;
            IoC.Resolve<IQuoteServices>().SaveQoute(quote, new[] { 1 });
            savedQuotation = IoC.Resolve<IQuoteServices>().FindQuote(1);
            total = IoC.Resolve<IQuoteServices>().CalculateQuoteTotal(savedQuotation);
            Assert.AreEqual(1,savedQuotation.Id);
            Assert.AreEqual(new decimal(348.75), quote.Total);
        }

        [Test]
        public void TestCreateJobFromQuote()
        {
            CalculateQuoteTotalForCompany1();
            var quote = IoC.Resolve<IQuoteServices>().FindQuote(1);
            var job = IoC.Resolve<IJobServices>().CreateJob(1);
            Assert.AreEqual(1,job.JobNr);
            Assert.IsNotNull(job.Quotation);
            Assert.AreEqual(new decimal(348.75), quote.Total);
            job.Tax = 10;
            Assert.AreEqual(new decimal(358.75), job.Total);
            Assert.IsNotNull(job.JobItems);
            Assert.IsNotNull(job.Deposits);
            Assert.AreEqual(new decimal(358.75),job.Balance);
            Assert.AreEqual(quote.QuoteItems[0].ColorGroup,job.JobItems[0].FinalColorGroup);
            Assert.AreEqual(quote.QuoteItems[0].Edge,job.JobItems[0].FinalEdge);
            Assert.IsNull(job.JobItems[0].Backsplash);

            var savedJob = IoC.Resolve<IEntityServices>().GetInstance<Job>(1);
            Assert.AreEqual(1,savedJob.JobNr);
            savedJob.Status = JobStatus.InHold;
            savedJob.TemplateDate = DateTime.Today.AddDays(2);
            savedJob.JobNotes.Add(new Note {Name = "Test", Description = "Tesing site"});
            savedJob.JobItems[0].FinalColorGroup = savedJob.Quotation.QuoteItems[0].ColorGroup.Childrens[0];
            savedJob.JobItems[0].FinalEdge = savedJob.Quotation.QuoteItems[0].Edge.Childrens[1];

            IoC.Resolve<IJobServices>().SaveJob(savedJob);

            var savedJob1 = IoC.Resolve<IEntityServices>().GetInstance<Job>(1);
            Assert.AreEqual(1, savedJob1.JobNr);
            Assert.AreEqual(1, savedJob1.JobNotes.Count);
            Assert.AreEqual(JobStatus.InHold, savedJob1.Status);
            Assert.AreEqual(DateTime.Today.AddDays(2), savedJob1.TemplateDate);
            Assert.AreEqual("Color 0", savedJob1.JobItems[0].FinalColorGroup.Name);
            Assert.AreEqual("Edge child 1", savedJob1.JobItems[0].FinalEdge.Name);
        }

    }

}
