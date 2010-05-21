using System.Collections.Generic;
using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.Stoves")]
    public class Stove : ParentIdentifier<Stove>
    {
        private IList<Stove> childrens = new List<Stove>();
        IList<StovePrice> prices = new List<StovePrice>();

        [HasMany(typeof(Stove), ColumnKey = "ParentId", Inverse = true, Lazy = true)]
        public virtual IList<Stove> Childrens
        {
            get { return childrens; }
            set { childrens = value; }
        }
        [HasMany(typeof(StovePrice), Inverse = true, Lazy = true, Where = "Type='Stove'")]
        public virtual IList<StovePrice> Prices
        {
            get { return prices; }
            set { prices = value; }
        }

        public virtual decimal GetPrice(Project prj)
        {
            foreach (var price in prices)
            {
                if (price.Project != prj) continue;
                return price.Price;
            }

            return 0;
        }

    }
}