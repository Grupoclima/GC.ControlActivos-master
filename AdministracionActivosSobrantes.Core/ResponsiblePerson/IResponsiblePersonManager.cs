using System;
using Abp.Domain.Services;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.ResponsiblePerson
{
    public interface IResponsiblePersonManager : IDomainService
    {
        bool ResponsiblePersonExist(string name, Guid id, string company);
        User GetUser(Guid? id);
        ResponsiblePerson GetResponsiblePerson(string name, string company);
        void Create(ResponsiblePerson entity);
    }
}
