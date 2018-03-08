using System;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.ResponsiblePerson
{
    public class ResponsiblePersonManager : DomainService, IResponsiblePersonManager
    {
        private readonly IRepository<ResponsiblePerson, Guid> _responsiblePersonRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IDateTime _dateTime;

        public ResponsiblePersonManager(IRepository<ResponsiblePerson, Guid> responsiblePersonRepository, IRepository<User, Guid> userRepository, IDateTime dateTime)
        {
            _responsiblePersonRepository = responsiblePersonRepository;
            _userRepository = userRepository;
            _dateTime = dateTime;
        }

        public bool ResponsiblePersonExist(string name, Guid id, string company)
        {
            var @entity = _responsiblePersonRepository.FirstOrDefault(e => e.Name.Equals(name) && e.CompanyName.Equals(company) && e.Id != id && e.IsDeleted.Value == false);
            return @entity != null;
        }

        public ResponsiblePerson GetResponsiblePerson(string name, string company)
        {
            var @entity = _responsiblePersonRepository.FirstOrDefault(e => e.Name.Equals(name) && e.CompanyName.Equals(company));
            return @entity;
        }

        public void Create(ResponsiblePerson entity)
        {
            _responsiblePersonRepository.Insert(entity);
        }

        public User GetUser(Guid? id)
        {
            if (id == null)
                return null;

            Guid idTemp = id.Value;
            var @entity = _userRepository.Get(idTemp);
            return @entity;
        }
    }
}
