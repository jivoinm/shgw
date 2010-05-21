using System.Collections.Generic;
using System.Text;
using Castle.ActiveRecord;
using Castle.Components.Validator;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.Customers")]
    public class Customer
    {
        [PrimaryKey]
        public virtual int Id { get; set; }
        [Property]
        [ValidateNonEmpty]
        public virtual string CustomerName { get; set; }
        [Property]
        public virtual string Address { get; set; }
        [Property]
        public virtual string PostalCode { get; set; }
        [BelongsTo("CityId")]
        [ValidateNonEmpty]
        public virtual City City { get; set; }
        [Property]
        public virtual string Phone { get; set; }
        [Property]
        public virtual string PhoneExt { get; set; }
        [Property]
        public virtual string Fax { get; set; }

        public override string ToString()
        {
            var customer = new List<string>();
            if(!string.IsNullOrEmpty(Address)) customer.Add(Address);
            if(!string.IsNullOrEmpty(PostalCode))customer.Add(PostalCode);
            if(City!=null)customer.Add(City.Name);
            return string.Join(", ", customer.ToArray());
        }
    }

    [ActiveRecord("dbo.Cities")]
    public class City 
    {
        [PrimaryKey]
        public virtual int Id { get; set; }
        [Property]
        [ValidateNonEmpty]
        public virtual string Name { get; set; }
    }
}