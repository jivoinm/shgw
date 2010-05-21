using System;
using System.Collections.Generic;
using Castle.ActiveRecord;
using Castle.Components.Validator;

namespace StoneHaven.Models
{
    [ActiveRecord("Jobs")]
    public class Job : Traceable
    {
        private IList<Deposit> deposits = new List<Deposit>();
        private IList<JobItem> jobItems = new List<JobItem>();
        private IList<Note> jobNotes = new List<Note>();

        [Property, ValidateNonEmpty]
        public virtual long JobNr { get; set; }
        
        [Property]
        public virtual string InvoiceNr { get; set; }
        
        [BelongsTo("QuoteId")]
        public virtual Quotation Quotation { get; set; }

        [Property]
        public virtual JobStatus Status { get; set; }
        
        [Property]
        public virtual DateTime? CreateDate { get; set; }
        [Property]
        public virtual DateTime? TemplateDate { get; set; }

        [Property]
        public virtual DateTime? InstallationDate { get; set; }
        
        [Property]
        public virtual DateTime? ServiceDate { get; set; }
        
        [Property]
        public virtual string TemplateBy { get; set; }

        [Property]
        public virtual string InstallationBy { get; set; }
        
        [Property]
        public virtual string ServiceBy { get; set; }
        
        [Property]
        public virtual decimal Tax { get; set; }

        [HasMany(typeof(JobItem), Inverse = true, Lazy = true, Cascade = ManyRelationCascadeEnum.Delete)]
        public virtual IList<JobItem> JobItems
        {
            get { return jobItems; }
            set { jobItems = value; }
        }
        
        [HasMany(typeof(Deposit),Inverse = true, Lazy = true,Cascade = ManyRelationCascadeEnum.Delete)]
        public virtual IList<Deposit> Deposits
        {
            get { return deposits; }
            set { deposits = value; }
        }

        [HasMany(typeof(Note), Inverse = true, Lazy = true, Cascade = ManyRelationCascadeEnum.Delete)]
        public virtual IList<Note> JobNotes
        {
            get { return jobNotes; }
            set { jobNotes = value; }
        }

        [Property]
        public virtual bool Deleted { get; set; }

        [Property()]
        public virtual decimal Total { get; set; }

        public virtual decimal Balance
        {
            get
            {
                decimal paid = 0;
                foreach (var deposit in deposits)
                {
                    paid += deposit.Amount;
                }
                return TotalPlusTax - paid;
            }
        }
        public virtual decimal TotalPlusTax
        {
            get { return Total + Tax; }
        }
        public virtual string JobStatusColor
        {
            get
            {
                return Status == JobStatus.Confirmed
                           ? "#AAAA11"
                           : Status == JobStatus.InHold
                                 ? "#A87070"
                                 : Status == JobStatus.JobInProgress
                                       ? "#668CD9"
                                       : Status == JobStatus.Services
                                             ? "#DD4477"
                                             : Status == JobStatus.Templates ? "#D6AE00" : "Transparent";
            }
        }
    }

    [ActiveRecord("Notes")]
    public class Note : ParentIdentifier<Note>
    {
        private DateTime? date = DateTime.Now;

        [Property]
        public virtual DateTime? Date
        {
            get { return date; }
            set { date = value; }
        }
        [BelongsTo("JobId")]
        public virtual Job Job { get; set; }
    }

    [ActiveRecord("JobItems")]
    public class JobItem 
    {
        [PrimaryKey(PrimaryKeyType.Identity), ValidateNonEmpty]
        public virtual int Id { get; set; }

        [BelongsTo("JobId")]
        public virtual Job Job { get; set; }
        [BelongsTo("ColorGroupId")]
        public virtual ColorGroup FinalColorGroup { get; set; }
        [BelongsTo("EdgeId")]
        public virtual Edge FinalEdge { get; set; }
        [BelongsTo("ThicknesId")]
        public virtual Thicknes FinalThicknes { get; set; }
        [Property]
        public virtual string Backsplash { get; set; }
        [Property]
        public virtual int TagNr { get; set; }
        [Property]
        public virtual int NrOfSlabs { get; set; }
        [Property]
        public virtual string SinkType { get; set; }
        [Property]
        public virtual string SlabLocation { get; set; }
        [Property]
        public virtual string SinkLocation { get; set; }
        [Property]      
        public virtual string Thicknes { get; set; }

    }
    
    [ActiveRecord("Deposits")]
    public class Deposit : Traceable
    {
        [BelongsTo("JobId")]
        public virtual Job Job { get; set; }

        [Property]
        public virtual decimal Amount { get; set; }
        [Property]
        public virtual DateTime? PaidOn { get; set; }
        [Property]
        public virtual PaymentMethod PaidBy { get; set; }
    }

    public enum PaymentMethod
    {
        CreditCard = 0,
        Cash = 1,
        Cheque = 2
    }

    public enum JobStatus
    {
        Confirmed = 0,
        Templates = 1,
        JobInProgress = 2,
        Services = 3,
        InHold = 4,
    }
}