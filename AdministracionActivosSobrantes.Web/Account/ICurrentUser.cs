

using System;
using Abp.Application.Services;

namespace AdministracionActivosSobrantes.Web.Account
{
    public interface ICurrentUser : IApplicationService
    {
        Guid CurrentUserId { get; }
        string UserName { get; }
        string IpAddress { get; }
        string CompanyName { get; }
        //string Department { get; }
    }
}
