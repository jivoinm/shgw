using System;
using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.Measurements", DiscriminatorColumn = "Type", DiscriminatorType = "String", DiscriminatorValue = "Base")]
    public abstract class Measurement
    {
        [PrimaryKey]
        public virtual int Id { get; set; }
        [Property]
        public virtual double Length { get; set; }
        [Property]
        public virtual double Width { get; set; }

        [BelongsTo("QuoteItemId")]
        public virtual QuoteItem QuoteItem { get; set; }

        public abstract double GetTotalSqFt();
    }
}