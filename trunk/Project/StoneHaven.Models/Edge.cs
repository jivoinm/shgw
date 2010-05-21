using System.Collections.Generic;
using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.Edges")]
    public class Edge : ParentIdentifier<Edge>
    {
        private IList<Edge> childrens = new List<Edge>();
        IList<EdgePrice> prices = new List<EdgePrice>();
        
        [HasMany(typeof(Edge),ColumnKey = "ParentId",Inverse = true,Lazy = true)]
        public virtual IList<Edge> Childrens
        {
            get { return childrens; }
            set { childrens = value; }
        }
        
        [HasMany(typeof(EdgePrice),  Inverse = true, Lazy = true,Where="Type='Edge'")]
        public virtual IList<EdgePrice> Prices
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