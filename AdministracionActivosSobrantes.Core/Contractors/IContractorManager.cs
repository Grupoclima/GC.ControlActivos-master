using System;
using Abp.Domain.Services;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Contractors
{
    public interface IContractorManager : IDomainService
    {
        bool ContractorExist(string name, Guid id, string company);
        User GetUser(Guid? id);

    }
}
