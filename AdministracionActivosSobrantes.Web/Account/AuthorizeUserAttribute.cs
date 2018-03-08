using System;
using System.Web;
using System.Web.Mvc;
using AdministracionActivosSobrantes.Users;
using AdministracionActivosSobrantes.Web.Models.Account;
using AllowAnonymousAttribute = AdministracionActivosSobrantes.Web.Account.AllowAnonymousAttribute;

namespace AdministracionActivosSobrantes.Web.Account
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        // the "new" must be used here because we are hiding
        // the Roles property on the underlying class
        public new UserRoles Roles;
        //private static bool _failedRolesAuth;

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");
            //var isAuthorized = base.AuthorizeCore(httpContext);
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                // The user is not authorized => no need to continue
                return false;
            }

            var cu = (CustomPrincipal)HttpContext.Current.User;
            var role = cu.Rol;
            var dep = cu.Department;

            if (Roles != 0 && (Roles & role) != role)
            {
                //_failedRolesAuth = true;
                return true;
                //return false;
            }

            return true;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            var skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) ||
                                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(
                                    typeof(AllowAnonymousAttribute), true);
            if (!skipAuthorization)
            {
                base.OnAuthorization(filterContext);
            }
            //if (_failedRolesAuth)
            //{
            //    filterContext.Result = new ViewResult { ViewName = "NotAuth" };
            //}
        }

    }
}