using System.Collections.Generic;
using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.Sinks")]
    public class Sink : ParentIdentifier<Sink>
    {
        private IList<Sink> childrens = new List<Sink>();
        IList<SinkPrice> prices = new List<SinkPrice>();

        [HasMany(typeof(Sink), ColumnKey = "ParentId", Inverse = true, Lazy = true)]
        public virtual IList<Sink> Childrens
        {
            get { return childrens; }
            set { childrens = value; }
        }

        [HasMany(typeof(SinkPrice), Inverse = true, Lazy = true, Where = "Type='Sink'")]
        public virtual IList<SinkPrice> Prices
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