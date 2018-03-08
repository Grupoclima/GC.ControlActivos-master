using System;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Contractors
{
    public class ContractorManager : DomainService, IContractorManager
    {
        private readonly IRepository<Contractor, Guid> _contractorRepository;
        private readonly IRepository<User, Guid> _userRepository;

        public ContractorManager(IRepository<Contractor, Guid> contractorRepository, IRepository<User, Guid> userRepository )
        {
            _contractorRepository = contractorRepository;
            _userRepository = userRepository;
        }

        public bool ContractorExist(string name, Guid id, string company)
        {
            var @entity = _contractorRepository.FirstOrDefault(e => e.CompleteName.Equals(name) && e.CompanyName.Equals(company) && e.Id != id && e.IsDeleted.Value == false);
            return @entity != null;
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
