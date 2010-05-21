using System.Security.Principal;
using System.Threading;
using System.Web.Security;
using Castle.MonoRail.Framework;
using Rhino.Commons;
using StoneHaven.Models;
using StoneHaven.Models.Services;

namespace MSoftLibrary.Filters
{
    public class Authentication : IFilter
    {
        public bool Perform(ExecuteWhen exec, IEngineContext context, IController controller, IControllerContext controllerContext)
        {
            if (exec == ExecuteWhen.BeforeAction)
            {
                if (context.CurrentUser.Identity.IsAuthenticated)
                {
                    var user = IoC.Resolve<IUserServices>().GetUser(context.CurrentUser.Identity.Name);
                    if (user == null) return false;
                    controllerContext.PropertyBag["isAdmin"] = user.Role.Name.Contains("Administrator") || user.Role.Name.Contains("Admin");
                    controllerContext.PropertyBag["currentUser"] = user.Name;
                    //controllerContext.PropertyBag["nrOfMessages"] = UserMessage.GetCurrentUserNewMessagesCount();
                    return true;
                }
                if("XMLHttpRequest".Equals(context.Request.Headers["X-Requested-With"])) 
                    return true;
            }

            FormsAuthentication.RedirectToLoginPage();
            return false;
        }
    }
}