using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.StovePrices", DiscriminatorValue = "Stove")]
    public class StovePrice : PriceBase
    {
        [BelongsTo()]
        public virtual Stove Entity { get; set; }
    }
}