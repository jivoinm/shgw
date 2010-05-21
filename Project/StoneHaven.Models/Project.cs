using System.Collections.Generic;
using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.Projects")]
    public class Project : ParentIdentifier<Project>
    {
        private IList<Project> childrens = new List<Project>();

        [HasMany(typeof(Project), ColumnKey = "ParentId", Inverse = false, Lazy = true)]
        public virtual IList<Project> Childrens
        {
            get { return childrens; }
            set { childrens = value; }
        }
        [BelongsTo("BumpoutId",Cascade = CascadeEnum.All)]
        public virtual Bumpout BumpoutPrice { get; set; }
        [BelongsTo("ArcheId",Cascade = CascadeEnum.All)]
        public virtual Arche ArchePrice { get; set; }
    }
}