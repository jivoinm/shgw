namespace StoneHaven.Models.Services
{
    public interface ISinkServices
    {
        Sink SaveSink(Sink sink);
        SinkPrice SaveSinkPrice(SinkPrice price);
    }
}