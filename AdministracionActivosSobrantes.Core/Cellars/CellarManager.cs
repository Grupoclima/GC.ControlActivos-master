using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Cellars
{
    public class CellarManager : DomainService, ICellarManager
    {
        private readonly IRepository<Cellar, Guid> _cellarRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IDateTime _dateTime;

        public CellarManager(IRepository<Cellar, Guid> cellarRepository, IRepository<User, Guid> userRepository, IDateTime dateTime)
        {
            _cellarRepository = cellarRepository;
            _userRepository = userRepository;
            _dateTime = dateTime;
        }

        public bool CellarExist(string name, Guid id, string company)
        {
            var @entity = _cellarRepository.FirstOrDefault(e => e.Name.Equals(name) && e.CompanyName.Equals(company) && e.Id != id && e.IsDeleted.Value == false);
            return @entity != null;
        }

        public IList<Cellar> GetAllCellars(string company)
        {
            var @list = _cellarRepository.GetAll().Where(c => c.CompanyName.Equals(company)).ToList();
            return @list;
        }

        public IList<User> GetAllWareHouseUsers(string company)
        {
            var @entities = _userRepository.GetAllList(u=>u.Rol == UserRoles.AuxiliarUca && u.CompanyName.Equals(company));
            return @entities;
        }

        public User GetUser(Guid? id)
        {
            if (id==null)
                return null;

            Guid idTemp = id.Value;
            var @entity = _userRepository.Get(idTemp);
            return @entity;
        }

    }
}
