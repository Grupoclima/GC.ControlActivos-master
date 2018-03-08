using System;
using Abp.Application.Services;
using AdministracionActivosSobrantes.Users.Dto;

namespace AdministracionActivosSobrantes.Users
{
    public interface IUserAppService : IApplicationService
    {
        UserDto Get(string userName, string company);
    }
}
