using Castle.ActiveRecord;
using Castle.Components.Validator;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.Users")]
    public class User : ParentIdentifier<User>
    {
        [Property]
        [ValidateNonEmpty]
        [ValidateIsUnique("UserName must be unique",ExecutionOrder = 2)]
        public virtual string UserName { get; set; }
        [Property]
        [ValidateNonEmpty]
        public virtual string Password { get; set; }
        [BelongsTo("RoleId")]
        [ValidateNonEmpty]
        public virtual Role Role { get; set; }
    }

    [ActiveRecord("dbo.Roles")]
    public class Role : ParentIdentifier<Role>
    {
    }
}