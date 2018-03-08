using System;
using System.Security.Principal;

namespace AdministracionActivosSobrantes.Web.Models.Account
{
    public interface ICustomPrincipal : IPrincipal
    {
        Guid UserId { get; set; }
        string UserName { get; set; }
        string CompleteName { get; set; }
        string CompanyName { get; set; }
        string Department { get; set; }

    }
}