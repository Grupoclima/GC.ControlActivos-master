using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Common;

namespace AdministracionActivosSobrantes.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserDto : EntityDto<Guid>
    {
        public string UserCode { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string CompleteName { get; set; }

        public string CompanyName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Department { get; set; }

        public UserRoles Rol { get; set; }
       
    }
}
