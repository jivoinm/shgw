using System.Collections.Generic;

namespace StoneHaven.Models.Services
{
    public interface IQuoteServices
    {
        void SaveQoute(Quotation quotation,int[] colorIds);
        Quotation FindQuote(int id);
        QuoteItemTotal CalculateProjectTotal(QuoteItem quoteItem);
        decimal CalculateQuoteTotal(Quotation quote);
        IDictionary<string, string> CalculatePricesForAllColorGroups(QuoteItem quoteItem,ColorGroupTypeEnum type);
//        IList<CounterMeasurement> GetColorMeasurements(QuoteItem quoteItem);
//        IList<BackSplashMeasurement> GetBSMeasurements(QuoteItem quoteItem);
//        IList<EdgeMeasurement> GetEdgeMeasurements(QuoteItem quoteItem);

    }
}