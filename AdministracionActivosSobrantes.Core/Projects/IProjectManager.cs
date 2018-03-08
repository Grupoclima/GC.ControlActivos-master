using System;
using Abp.Domain.Services;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Projects
{
    public interface IProjectManager : IDomainService
    {
        bool ProjectExist(string name, Guid id, string company);
        User GetUser(Guid? id);
        void Create(Project entity);
    }
}
