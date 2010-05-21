using Rhino.Commons;

namespace StoneHaven.Models.Services.DB
{
    public class SinkServices : ISinkServices
    {
        public Sink SaveSink(Sink sink)
        {
            Repository<Sink>.Save(sink);
            return sink;
        }

        public SinkPrice SaveSinkPrice(SinkPrice price)
        {
            Repository<SinkPrice>.Save(price);
            return price;

        }
    }
}