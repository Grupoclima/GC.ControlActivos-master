using System;
using System.Collections.Generic;
using Abp.Application.Services;
using AdministracionActivosSobrantes.Cellars.Dto;
using AdministracionActivosSobrantes.Users;
using MvcPaging;

namespace AdministracionActivosSobrantes.Cellars
{
    public interface ICellarAppService : IApplicationService
    {
        IPagedList<CellarDto> SearchCellar(SearchCellarsInput searchInput);
        IList<User> GetAllWareHouseUsers(string company);
        IList<Cellar> GetAllCellar(string company);
        GetCellarOutput Get(Guid id);
        UpdateCellarInput GetEdit(Guid id);
        DetailCellarOutput GetDetail(Guid id);
        void Update(UpdateCellarInput input);
        void Create(CreateCellarInput input);
        void Delete(Guid id, Guid userid);
        void ChangeStatus(Guid id, Guid userid);
    }
}
