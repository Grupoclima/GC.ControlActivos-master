using System;
using Abp.Domain.Services;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Categories
{
    public interface ICategoryManager: IDomainService
    {
        bool CategoryExist(string name, Guid id, string company);
        User GetUser(Guid? id);
        Category GetCategory(string name, string company);
    }
}
