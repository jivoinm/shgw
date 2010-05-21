using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Commons;
using StoneHaven.Models.Services;

namespace StoneHaven.Models.Tests
{
    public class QuotationHelper : BaseTests
    {
        protected Company CreateCompany(string name)
        {
            var company1 = new Company { Name = name };
            IoC.Resolve<IEntityServices>().SaveEntity(company1);
            IoC.Resolve<IEntityServices>().Flush();
            return company1;
        }

        protected void CreateProjectPriceFor(Quotation quote, string propName, object value)
        {
            quote.QuoteItems[0].Project.GetType().GetProperty(propName).SetValue(quote.QuoteItems[0].Project, value, null);
            IoC.Resolve<IEntityServices>().SaveEntity(quote.QuoteItems[0].Project);
            var project = IoC.Resolve<IEntityServices>().GetInstance<Project>(1);
            Assert.AreEqual(value, project.GetType().GetProperty(propName).GetValue(project, null));
        }

        #region Asserts

        protected void AssertSinkWasSaved(Quotation quotation, Quotation savedQoutation)
        {
            Assert.IsNotNull(savedQoutation.QuoteItems[0].Sink);
            Assert.IsNotNull(savedQoutation.QuoteItems[0].Sink.Name);
            Assert.AreEqual(quotation.QuoteItems[0].Sink.Prices.Count, savedQoutation.QuoteItems[0].Sink.Prices.Count);
            Assert.AreEqual(quotation.QuoteItems[0].Sink.Prices, savedQoutation.QuoteItems[0].Sink.Prices);
        }

        protected void AssertThicknesWasSaved(Quotation quotation, Quotation savedQoutation)
        {
            Assert.AreEqual(quotation.QuoteItems[0].Thicknes.Childrens.Count, savedQoutation.QuoteItems[0].Thicknes.Childrens.Count);
            Assert.AreEqual(quotation.QuoteItems[0].Thicknes.Prices.Count, savedQoutation.QuoteItems[0].Thicknes.Prices.Count);
            Assert.AreEqual(quotation.QuoteItems[0].Thicknes.Prices[0].Price, savedQoutation.QuoteItems[0].Thicknes.Prices[0].Price);
        }

        protected void AssertEdgeWasSaved(Quotation quotation, Quotation savedQoutation)
        {
            Assert.AreEqual(quotation.QuoteItems[0].Edge.Childrens.Count, savedQoutation.QuoteItems[0].Edge.Childrens.Count);
            Assert.AreEqual(quotation.QuoteItems[0].Edge.Prices.Count, savedQoutation.QuoteItems[0].Edge.Prices.Count);
            Assert.AreEqual(quotation.QuoteItems[0].Edge.Prices[1].Id, savedQoutation.QuoteItems[0].Edge.Prices[1].Id);
            Assert.AreEqual(quotation.QuoteItems[0].Edge.Prices[0].Price, savedQoutation.QuoteItems[0].Edge.Prices[0].Price);
        }

        protected void AssertColorGroupWasSaved(Quotation quotation, Quotation savedQoutation)
        {
            Assert.IsNotNull(savedQoutation.QuoteItems[0].ColorGroup);
            Assert.AreEqual(quotation.QuoteItems[0].ColorGroup.Childrens.Count, savedQoutation.QuoteItems[0].ColorGroup.Childrens.Count);
            //Assert.AreEqual(quotation.QuoteItems[0].ColorGroup.Prices[1].Price, savedQoutation.QuoteItems[0].ColorGroup.Prices[1].Price);
        }

        protected void AssertQuotationWasSaved(Quotation quotation, Quotation savedQoutation)
        {
            Assert.AreEqual(quotation.QuoteNr, savedQoutation.QuoteNr);
            Assert.AreEqual(quotation.QuoteItems.Count, savedQoutation.QuoteItems.Count);
        }

        #endregion

        #region Quotes
        protected Quotation GetQuotation(Company company)
        {
            var quotation = new Quotation
            {
                Company = company,
                Customer = new Customer
                {
                    CustomerName = "Customer1",
                    Address = "Address1",
                    Phone = "1234567890",
                    Fax = "1234567890"
                }
            };


            quotation.QuoteItems.Add(CreateProjectQuoteItemFor(company));

            IoC.Resolve<IQuoteServices>().SaveQoute(quotation,new []{1});
            UnitOfWork.CurrentSession.Flush();
            return quotation;
        }

        protected QuoteItem CreateProjectQuoteItemFor(Company company)
        {
            var project = new Project { Name = "Kitchen" };
            var colorGroup = new ColorGroup { Name = "Color1" };
            var edgeGroup = new Edge { Name = "Standard" };
            var thicknes = new Thicknes { Name = "3/4" };
            var sink = new Sink { Name = "Undermont" };
            var stove = new Stove { Name = "Unknown" };

            IoC.Resolve<IEntityServices>().SaveEntity(project);
            IoC.Resolve<IEntityServices>().SaveEntity(colorGroup);
            IoC.Resolve<IEntityServices>().SaveEntity(edgeGroup);
            IoC.Resolve<IEntityServices>().SaveEntity(thicknes);
            IoC.Resolve<IEntityServices>().SaveEntity(sink);
            IoC.Resolve<IEntityServices>().SaveEntity(stove);


            edgeGroup.Childrens = CreateTenEdgeChildrens(edgeGroup);
            colorGroup.Childrens = CreateFiveColorNamesForGroup(colorGroup);
            thicknes.Childrens = CreateFiveThickensNames(thicknes);

//            colorGroup.Prices.Add(CreatePrice(new ColorGroupPrice { Project = project, Entity = colorGroup, Price = 2, Company = company }));
//            colorGroup.Prices.Add(CreatePrice(new ColorGroupPrice { Project = project, Entity = colorGroup, Price = 200, Company = company }));

            edgeGroup.Prices.Add(CreatePrice(new EdgePrice { Project = project, Entity = edgeGroup, Price = 3, Company = company }));
            edgeGroup.Prices.Add(CreatePrice(new EdgePrice { Project = project, Entity = edgeGroup, Price = 20, Company = company }));

            thicknes.Prices.Add(CreatePrice(new ThicknesPrice { Project = project, Entity = thicknes, Price = 100, Company = company }));
            sink.Prices.Add(CreatePrice(new SinkPrice { Project = project, Entity = sink, Price = 6, Company = company }));

            stove.Prices.Add(CreatePrice(new StovePrice() { Project = project, Entity = stove, Price = 10, Company = company }));


            return new QuoteItem
            {
                Project = project,
                ColorGroup = colorGroup,
                Edge = edgeGroup,
                Thicknes = thicknes,
                Sink = sink,
                Stove = stove
            };
        }

        protected Price CreatePrice<Price>(Price entity)
        {
            return IoC.Resolve<IEntityServices>().SaveEntity(entity);
        }


        #endregion

        #region Color Group


        protected IList<ColorGroup> CreateFiveColorNamesForGroup(ColorGroup group)
        {
            IList<ColorGroup> colorNames = new Boo.Lang.List<ColorGroup>();
            for (var i = 0; i < 5; i++)
            {
                var name = new ColorGroup { Name = ("Color " + i), Parent = group };
                IoC.Resolve<IEntityServices>().SaveEntity(name);
                colorNames.Add(name);
            }
            return colorNames;
        }

        #endregion

        #region Edge

        protected IList<Edge> CreateTenEdgeChildrens(Edge edge)
        {
            var childrens = new List<Edge>();
            for (var i = 0; i < 10; i++)
            {
                var child = new Edge { Name = "Edge child " + i, Parent = edge };
                childrens.Add(IoC.Resolve<IEntityServices>().SaveEntity(child));
            }
            return childrens;
        }

        #endregion

        #region Thicknes

        protected IList<Thicknes> CreateFiveThickensNames(Thicknes thicknes)
        {
            IList<Thicknes> colorNames = new Boo.Lang.List<Thicknes>();
            for (var i = 0; i < 5; i++)
            {
                var name = new Thicknes { Name = ("Color " + i), Parent = thicknes };
                IoC.Resolve<IEntityServices>().SaveEntity(name);
                colorNames.Add(name);
            }
            return colorNames;
        }

        #endregion

    }
}