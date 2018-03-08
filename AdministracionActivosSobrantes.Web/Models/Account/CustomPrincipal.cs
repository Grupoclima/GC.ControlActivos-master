using System;
using System.Security.Principal;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Web.Models.Account
{
    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role) { return false; }

        public CustomPrincipal(string userName)
        {
            Identity = new GenericIdentity(userName);
        }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Department { get; set; }
        public UserRoles Rol { get; set; }
        public string CompleteName { get; set; }
        public string CompanyName { get; set; }

    }
}