using System;
using System.Collections.Generic;
using System.Threading;
using Castle.ActiveRecord;
using Castle.Components.Validator;

namespace StoneHaven.Models
{
    public class ParentIdentifier<T> : Traceable
    {
        [BelongsTo("ParentId")]
        public virtual T Parent { get; set; }
        [Property, ValidateNonEmpty]
        public virtual string Name { get; set; }
        [Property]
        public virtual string Description { get; set; }
    }

    public class ParentIdentifier 
    {
        [PrimaryKey(PrimaryKeyType.Identity), ValidateNonEmpty]
        public virtual int Id { get; set; }

    }
}