using System;
using System.Web;
using System.Web.Mvc;
using NLog;
using System.Threading.Tasks;
using Emaratech.Services.Channels.SSO.ChannelService;
using System.Web.Configuration;

namespace Emaratech.Services.Channels.SSO.Controllers
{
    public class DefaultController : Controller
    {
        private static readonly IUserChannelService UserService = new UserChannelServiceClient();

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SetCookies()
        {
            string login = null;
            foreach (var formKey in this.Request.Form.AllKeys)
            {
                if (string.Equals("login", formKey))
                {
                    login = Request.Form[formKey];
                }
                else
                {
                    AddSecurityCookie(formKey, Request.Form[formKey]);
                }
            }

            if (login != "null")
            {
                var userId = await UserService.GetUserIdAsync(login);
                AddSecurityCookie("USER_ID", userId);
            }

            return Redirect(WebConfigurationManager.AppSettings["RedirectUrl"]);
        }

        private void AddSecurityCookie(string argName, string argValue)
        {
            HttpCookie cookie = new HttpCookie(argName);
            cookie.Value = argValue;
            cookie.Expires = DateTime.Now.AddYears(1);           
            Logger.Trace($"Setting security logger {argName} to domain {cookie.Domain}");
            this.Response.Cookies.Add(cookie);
        }

        public ActionResult ClearCookies()
        {
            string[] myCookies = Request.Cookies.AllKeys;
            foreach (string cookie in myCookies)
            {
                this.Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }
            return View();
        }
    }
}