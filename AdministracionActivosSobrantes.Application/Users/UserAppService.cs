using System;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using AdministracionActivosSobrantes.Users.Dto;
using AutoMapper;

namespace AdministracionActivosSobrantes.Users
{
    class UserAppService : ApplicationService, IUserAppService
    {
        private readonly IRepository<User, Guid> _userRepository;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public UserAppService(IRepository<User, Guid> clientRepository)
        {
            _userRepository = clientRepository;
        }

        public UserDto Get(string userName,string company)
        {
            try
            {
                
                var @entity = _userRepository.FirstOrDefault(u => u.UserName.ToLower().Equals(userName.ToLower()) && u.CompanyName.Equals(company));
                if (@entity == null)
                {
                    throw new UserFriendlyException("No se pudo encontrar el Usuario, fue borrado o no existe en la compañia especificada.");
                }
                return Mapper.Map<UserDto>(@entity);
            }
            catch (Exception)
            {

                return null;
            }
            
        }
    }
}
