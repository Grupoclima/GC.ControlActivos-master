using System;
using Abp.Application.Services;
using AdministracionActivosSobrantes.Projects.Dto;
using MvcPaging;

namespace AdministracionActivosSobrantes.Projects
{
    public interface IProjectAppService : IApplicationService
    {
        IPagedList<ProjectDto> SearchProject(SearchProjectsInput searchInput);
        GetProjectOutput Get(Guid id);
        UpdateProjectInput GetEdit(Guid id);
        DetailProjectOutput GetDetail(Guid id);
        void Update(UpdateProjectInput input);
        void Create(CreateProjectInput input);
        void Delete(Guid id, Guid userid);
    }
}
