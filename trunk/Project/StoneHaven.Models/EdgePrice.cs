using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.EdgePrices", DiscriminatorValue = "Edge")]
    public class EdgePrice : PriceBase
    {
        [BelongsTo()]
        public virtual Edge Entity { get; set; }
    }
}