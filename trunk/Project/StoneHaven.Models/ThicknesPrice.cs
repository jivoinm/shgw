using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.ThicknesPrices", DiscriminatorValue = "Thicknes")]
    public class ThicknesPrice : PriceBase
    {
        [BelongsTo()]
        public virtual Thicknes Entity { get; set; }
    }
}