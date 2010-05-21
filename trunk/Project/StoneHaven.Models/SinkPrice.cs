using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.SinkPrices", DiscriminatorValue = "Sink")]
    public class SinkPrice : PriceBase
    {
        [BelongsTo()]
        public virtual Sink Entity { get; set; }
    }
}