using System;
using System.Collections.Generic;
using Abp.Domain.Services;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Cellars
{
    public interface ICellarManager:IDomainService
    {
        bool CellarExist(string name, Guid id, string company);
        IList<User> GetAllWareHouseUsers(string company);
        User GetUser(Guid? id);
        IList<Cellar> GetAllCellars(string company);
    }
}
