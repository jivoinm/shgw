using System.Collections.Generic;
using System.Threading;
using Castle.ActiveRecord;
using Query;
using Rhino.Commons;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.UserMessages")]
    public class UserMessage : Traceable
    {
        [BelongsTo("FromUserId")]
        public virtual User From { get; set; }
        [BelongsTo("ToUserId")]
        public virtual User To { get; set; }
        [Property]
        public virtual bool IsNew { get; set; }

        public static int GetCurrentUserNewMessagesCount()
        {
            return (int)Repository<UserMessage>.Count(Where.UserMessage.To.UserName == Thread.CurrentPrincipal.Identity.Name && Where.UserMessage.IsNew == true);
        }

        public static IList<UserMessage> GetCurrentUserNewMessages()
        {
            return (IList<UserMessage>)Repository<UserMessage>.FindAll(Where.UserMessage.To.UserName == Thread.CurrentPrincipal.Identity.Name && Where.UserMessage.IsNew == true, OrderBy.UserMessage.CreatedOn.Desc);
        }

        public static IList<UserMessage> GetCurrentUserAllMessages()
        {
            return (IList<UserMessage>) Repository<UserMessage>.FindAll(Where.UserMessage.To.UserName==Thread.CurrentPrincipal.Identity.Name,OrderBy.UserMessage.CreatedOn.Desc);
        }
    }
}
