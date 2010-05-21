using System;
using System.Collections.Generic;
using Castle.ActiveRecord;
using Castle.Components.Validator;
using Rhino.Commons;
using StoneHaven.Models.Services;


namespace StoneHaven.Models
{
    public enum SupplyEnum
    {
        Supply = 0,
        SupplyAndInstall = 1
    }

    [ActiveRecord("dbo.Quotations")]
    public class Quotation : Traceable
    {
        [Property]
        [ValidateNonEmpty]
        public virtual long QuoteNr { get; set; }
        
        [Property]
        public virtual string CompanyAtt{ get; set; }

        [Property]
        public virtual DateTime? Date { get; set; }

        [BelongsTo("CompanyId"), ValidateNonEmpty]
        public virtual Company Company { get; set; }

        [BelongsTo("CustomerId")]
        public virtual Customer Customer { get; set; }
        
        private IList<QuoteItem> quoteItems = new List<QuoteItem>();

        [HasMany(typeof(QuoteItem), Inverse = true, Lazy = true, Cascade = ManyRelationCascadeEnum.Delete)]
        public virtual IList<QuoteItem> QuoteItems
        {
            get { return quoteItems; }
            set { quoteItems = value; }
        }
        [Property]
        public virtual bool Deleted { get; set; }

        public virtual decimal Total
        {
            get
            {
                var total = IoC.Resolve<IQuoteServices>().CalculateQuoteTotal(this);
                return total;
            }
        }
    }
}
