using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using Abp.Web;
using AdministracionActivosSobrantes.Web.Models.Account;
using Castle.Facilities.Logging;

namespace AdministracionActivosSobrantes.Web
{
    public class MvcApplication : AbpWebApplication
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            AbpBootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("log4net.config"));
            base.Application_Start(sender, e);
        }


        protected override void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie == null) return;
            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            var serializer = new JavaScriptSerializer();

            if (authTicket != null)
            {
                var serializeModel = serializer.Deserialize<CustomPrincipalSerializeModel>(authTicket.UserData);

                var newUser = new CustomPrincipal(authTicket.Name)
                {
                    UserId = serializeModel.UserId,
                    UserName = serializeModel.UserName,
                    CompleteName = serializeModel.CompleteName,
                    Rol = serializeModel.Rol,
                    CompanyName = serializeModel.CompanyName
                };
                HttpContext.Current.User = newUser;
            }
        }
    }
}
