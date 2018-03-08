using System;
using System.Collections.Generic;
using Abp.Application.Services;
using AdministracionActivosSobrantes.Categories.Dto;
using AdministracionActivosSobrantes.Users;
using MvcPaging;

namespace AdministracionActivosSobrantes.Categories
{
    public interface ICategoryAppService : IApplicationService
    {
        IPagedList<CategoryDto> SearchCategory(SearchCategoryInput searchInput);
        GetcategoryOutput Get(Guid id);
        UpdatecategoryInput GetEdit(Guid id);
        DetailCategoryOutput GetDetail(Guid id);
        void Update(UpdatecategoryInput input);
        void Create(CreateCategoryInput input);
        void Delete(Guid id, Guid userid);
    }
}
