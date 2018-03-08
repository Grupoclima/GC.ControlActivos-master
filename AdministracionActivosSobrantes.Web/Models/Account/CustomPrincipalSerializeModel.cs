using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Web.Models.Account
{
    public class CustomPrincipalSerializeModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string CompanyName { get; set; }
        public string CompleteName { get; set; }
        public UserRoles Rol { get; set; }
        public string Department { get; set; }
    }
}