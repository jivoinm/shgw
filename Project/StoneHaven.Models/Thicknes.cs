using System.Collections.Generic;
using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.Thickneses")]
    public class Thicknes : ParentIdentifier<Thicknes>
    {
        private IList<Thicknes> childrens = new List<Thicknes>();
        IList<ThicknesPrice> prices = new List<ThicknesPrice>();
        
        [HasMany(typeof(Thicknes), ColumnKey = "ParentId", Inverse = true, Lazy = true)]
        public virtual IList<Thicknes> Childrens
        {
            get { return childrens; }
            set { childrens = value; }
        }

        [HasMany(typeof(ThicknesPrice), Inverse = true, Lazy = true, Where = "Type='Thicknes'")]
        public virtual IList<ThicknesPrice> Prices
        {
            get { return prices; }
            set { prices = value; }
        }

        public virtual decimal GetPrice(Project prj)
        {
            foreach (var groupPrice in prices)
            {
                if (groupPrice.Project != prj) continue;
                return groupPrice.Price;
            }

            return 0;
        }

    }
}