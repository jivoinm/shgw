using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.ColorGroupPrices",DiscriminatorValue = "ColorGroup")]
    public class ColorGroupPrice : PriceBase
    {
        [BelongsTo()]
        public virtual ColorGroup Entity { get; set; }
    }
}