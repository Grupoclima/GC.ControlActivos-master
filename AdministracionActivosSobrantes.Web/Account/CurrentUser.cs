using System;
using System.Security.Principal;
using System.Web;
using AdministracionActivosSobrantes.Web.Models.Account;

namespace AdministracionActivosSobrantes.Web.Account
{
    public class CurrentUser : ICurrentUser
    {
        private readonly Guid _userId = Guid.Empty;

        public Guid CurrentUserId
        {
            get { return _userId; }
        }

        private readonly string _username;
        public string UserName
        {
            get { return _username; }
        }

        private readonly string _companyname;
        public string CompanyName
        {
            get { return _companyname; }
        }

        private readonly string _ipaddress;
        public string IpAddress
        {
            get { return _ipaddress; }
        }

        private readonly string _Department;
        public string Department
        {
            get { return _Department; }
        }

        public CurrentUser()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (!(HttpContext.Current.User is GenericPrincipal))
                {
                    _userId = ((CustomPrincipal)HttpContext.Current.User).UserId;
                    _companyname = ((CustomPrincipal) HttpContext.Current.User).CompanyName;
                    _username = HttpContext.Current.User.Identity.Name;
                    _Department = ((CustomPrincipal)HttpContext.Current.User).Department;

                }
                _ipaddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
        }
    }
}