
using System.Security.Principal;
using System.Threading;
using System.Web.Security;
using Castle.MonoRail.Framework;
using MSoftLibrary.Filters;
using Rhino.Commons;
using StoneHaven.Models;
using StoneHaven.Models.Services;

namespace StoneHaven.Web.Controllers
{
    public class HomeController : BaseSecuredController
    {
        public void Index()
        {
            
        }

        [Layout("simple")]
        [SkipFilter(typeof(Authentication))]
        public void Login()
        {
            RenderView("loginPage");
        }

        [SkipFilter(typeof(Authentication))]
        public void DoLogin(string username,string password,bool remeberMe,string returnUrl)
        {
            if (username == null || password == null || !IoC.Resolve<IUserServices>().Authenticate(username, password))
            {
                SetErrorMessage("Invalid credentials, please try again.");
                RedirectToUrl(FormsAuthentication.LoginUrl);
                return;
            }
           
            FormsAuthentication.SetAuthCookie(username,remeberMe);
            if(returnUrl!=null && !returnUrl.ToLower().Contains("dologin"))
                RedirectToUrl(returnUrl);
            else
                RedirectToUrl(FormsAuthentication.DefaultUrl);
            return;
        }

        [SkipFilter(typeof(Authentication))]
        public void Logout()
        {
            FormsAuthentication.SignOut();
            RedirectToAction("Login");
        }
    }
}