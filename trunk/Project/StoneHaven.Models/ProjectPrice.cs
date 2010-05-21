using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.ProjectPrices", DiscriminatorColumn = "Type", DiscriminatorType = "String", DiscriminatorValue = "Base")]
    public class ProjectPrice 
    {
        [PrimaryKey]
        public virtual int Id { get; set; }
        [Property]
        public virtual decimal Price { get; set; }
    }

    [ActiveRecord(DiscriminatorValue = "Bumpout")]
    public class Bumpout : ProjectPrice
    { }
    
    [ActiveRecord(DiscriminatorValue = "Arche")]
    public class Arche : ProjectPrice
    { }
    
    [ActiveRecord(DiscriminatorValue = "Supply")]
    public class Supply : ProjectPrice
    { }

    [ActiveRecord(DiscriminatorValue = "SupplyAndInstall")]
    public class SupplyAndInstall : ProjectPrice
    { }


}