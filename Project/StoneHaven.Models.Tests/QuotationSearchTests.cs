using NUnit.Framework;
using Rhino.Commons;
using StoneHaven.Models.Services;

namespace StoneHaven.Models.Tests
{
    [TestFixture]
    public class QuotationSearchTests : QuotationHelper
    {
        [Test]
        public void SearchQuoteByQuoteNr()
        {
            var company1 = CreateCompany("Company2");
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
    }
}