using System;
using System.Threading;
using Castle.ActiveRecord;
using Castle.Components.Validator;

namespace StoneHaven.Models
{
    public class Traceable : ParentIdentifier
    {
        private string createdBy;
        private DateTime? createdOn;
        private string lastUpdatedBy;
        private DateTime? lastUpdatedOn;

        [Property]
        public virtual string CreatedBy
        {
            get { return Id >=0 ? createdBy : Thread.CurrentPrincipal.Identity.Name;}
            set { createdBy = value; }

        }

        [Property]
        public virtual DateTime? CreatedOn
        {
            get { return Id > 0 ? createdOn.HasValue ? createdOn : DateTime.Now : DateTime.Now; }
            set { createdOn = value; }
        }

        [Property]
        public virtual string LastUpdatedBy
        {
            get { return Thread.CurrentPrincipal.Identity.Name; }
            set { lastUpdatedBy = value;} 
        }

        [Property(Access = PropertyAccess.NosetterCamelcase)]
        public virtual DateTime? LastUpdatedOn
        {
            get { return DateTime.Now; }
            set { lastUpdatedOn = value;}
        }

        [Property]
        public virtual string Message { get; set; }

     

    }
}