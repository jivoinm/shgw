using Castle.ActiveRecord;
using Castle.Components.Validator;

namespace StoneHaven.Models
{
    [ActiveRecord("Companies")]
    public class Company : ParentIdentifier<Company>
    {
        [Property]
        public virtual string Address { get; set; }
        [Property]
        public virtual string Phone { get; set; }
        [Property]
        public virtual string Fax { get; set; }
        [Property]
        [ValidateEmail]
        public virtual string Email { get; set; }
    }
}