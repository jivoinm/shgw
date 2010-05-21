using System.Collections.Generic;
using Castle.ActiveRecord;
using Rhino.Commons;

namespace StoneHaven.Models
{
    public enum ColorGroupTypeEnum
    {
        Granite,
        Quartz
    }

    [ActiveRecord("dbo.ColorGroups")]
    public class ColorGroup : ParentIdentifier<ColorGroup>
    {
        private IList<ColorGroupPrice> prices = new List<ColorGroupPrice>();
        private IList<ColorGroup> childrens = new List<ColorGroup>();

        [HasMany(typeof(ColorGroup), ColumnKey = "ParentId", Inverse = false, Lazy = true)]
        public virtual IList<ColorGroup> Childrens
        {
            get { return childrens; }
            set { childrens = value; }
        }

        [HasMany(typeof(ColorGroupPrice), Inverse = true, Lazy = true,Cascade = ManyRelationCascadeEnum.AllDeleteOrphan, Where = "Type='ColorGroup'")]
        public virtual IList<ColorGroupPrice> Prices
        {
            get { return prices; }
            set { prices = value; }
        }

        [Property]
        public virtual ColorGroupTypeEnum ColorGroupType { get; set; }
    }

}