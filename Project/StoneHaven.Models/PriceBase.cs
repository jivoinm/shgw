using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.CompanyPrices", DiscriminatorColumn = "Type", DiscriminatorType = "String", DiscriminatorValue = "None")]
    public class PriceBase
    {
        protected Company company;
        protected Project project;
        protected decimal price;
        
        [PrimaryKey]
        public virtual int Id { get; set; }

        [BelongsTo("CompanyId")]
        public virtual Company Company
        {
            get { return company; }
            set { company = value; }
        }

        [BelongsTo("ProjectId")]
        public virtual Project Project
        {
            get { return project; }
            set { project = value; }
        }
        
        [Property]
        public virtual decimal Price
        {
            get { return price; }
            set { price = value; }
        }
        
//        [BelongsTo("SupplyId", Cascade = CascadeEnum.All)]
//        public virtual Supply SupplyPrice { get; set; }
//        [BelongsTo("SupplyAndInstallId", Cascade = CascadeEnum.All)]
//        public virtual SupplyAndInstall SupplyAndInstallPrice { get; set; }
        
        [Property]
        public virtual decimal SupplyPrice { get; set; }
        [Property]
        public virtual decimal SupplyAndInstallPrice { get; set; }

        public int EntityId { get; set; }
    }
}