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

        public string GetCompanyInfo()
        {
            return string.Format("Company:&nbsp;{0}<br/> Address:&nbsp;{1}<br/>ATT:{2}",Company.Name,Company.Address,CompanyAtt);
        }

        public string GetCompanyContactInfo()
        {
            return string.Format("Phone: {0}<br/> Fax: {1}<br/>Email:{2}", Company.Phone, Company.Fax,Company.Email);
        }

        public string GetCustomerInfo()
        {
            return string.Format("Customer:&nbsp;{0}<br/> Address:&nbsp;{1}", Customer.CustomerName, Customer.Address);
        }

        public string GetCustomerContactInfo()
        {
            return string.Format("Phone: {0}<br/> Fax: {1}", Customer.Phone + (!string.IsNullOrEmpty(Customer.PhoneExt) ? "(ext:" + Customer.PhoneExt + ")" : ""), Customer.Fax);
        }

    }
}
