using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Projects.Dto;
using AutoMapper;
using MvcPaging;

namespace AdministracionActivosSobrantes.Projects
{
    class ProjectAppService : ApplicationService, IProjectAppService
    {
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IProjectManager _projectManager;
        private readonly IDateTime _dateTime;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public ProjectAppService(IRepository<Project, Guid> projectRepository, IProjectManager projectManager, IDateTime dateTime)
        {
            _projectRepository = projectRepository;
            _projectManager = projectManager;
            _dateTime = dateTime;
        }

        public IPagedList<ProjectDto> SearchProject(SearchProjectsInput searchInput)
        {
            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value - 1 : 0;

            if (searchInput.Query == null)
                searchInput.Query = "";
            else
                searchInput.Query = searchInput.Query.ToLower();

            var @entities = _projectRepository.GetAll();
            int totalCount = @entities.Count();

            @entities = @entities.Where(c => c.IsDeleted != null && c.IsDeleted.Value == false && c.CompanyName.Equals(searchInput.CompanyName)
            && (c.Name.ToLower().Contains(searchInput.Query) || c.Name.ToLower().Equals(searchInput.Query) ||
            c.Code.ToLower().Contains(searchInput.Query) || c.Code.ToLower().Equals(searchInput.Query)
            || c.Description.Contains(searchInput.Query)));

            return @entities.OrderByDescending(p => p.Name).Skip(currentPageIndex * searchInput.MaxResultCount).Take(searchInput.MaxResultCount).MapTo<List<ProjectDto>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount,totalCount);
        }

        public GetProjectOutput Get(Guid id)
        {
            var @entity = _projectRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el Proyecto, fue borrada o no existe.");
            }
            return Mapper.Map<GetProjectOutput>(@entity);
        }

        public  UpdateProjectInput GetEdit(Guid id)
        {
            var @entity = _projectRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el Proyecto, fue borrada o no existe.");
            }
            return Mapper.Map<UpdateProjectInput>(@entity);
        }

        public DetailProjectOutput GetDetail(Guid id)
        {
            var @entity = _projectRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el Proyecto, fue borrada o no existe.");
            }
            var dto = Mapper.Map<DetailProjectOutput>(@entity);
            dto.CreatorUser = _projectManager.GetUser(dto.CreatorUserId);
            dto.LastModifierUser = _projectManager.GetUser(dto.LastModifierUserId);
            return dto;
        }

        public void Update(UpdateProjectInput input)
        {
            var @entity = _projectRepository.Get(input.Id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el Proyecto, fue borrada o no existe.");
            }
            if (_projectManager.ProjectExist(input.Name,input.Id, input.CompanyName))
            {
                throw new UserFriendlyException("Existe un Proyecto con el mismo Nombre.");
            }

            @entity.Name = input.Name;
            @entity.Description = input.Description;
            @entity.Code = input.Code;
            @entity.EstadoProyecto = input.EstadoProyecto;
            @entity.FinalDate = input.FinalDate;
            @entity.StartDate= input.StartDate;
            @entity.CostCenter = input.CostCenter;
            @entity.LastModificationTime = _dateTime.Now;
            @entity.LastModifierUserId = input.LastModifierUserId;

            _projectRepository.Update(@entity);
        }

        public void Create(CreateProjectInput input)
        {
            var @entity = Project.Create(input.Name, input.Code, input.Description, input.StartDate, input.FinalDate, 
                input.EstadoProyecto, input.CreatorUserId, input.CostCenter, input.CompanyName);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo crear el Proyecto.");
            }

            if (_projectManager.ProjectExist(@entity.Name, input.Id, input.CompanyName))
            {
                throw new UserFriendlyException("Existe un Proyecto con el mismo Nombre.");
            }
            _projectRepository.Insert(@entity);
        }

        public void Delete(Guid id, Guid userId)
        {
            var @entity = _projectRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el Proyecto, fue borrada o no existe.");
            }
            @entity.IsDeleted = true;
            @entity.DeleterUserId = userId;
            @entity.DeletionTime = _dateTime.Now;
            _projectRepository.Update(@entity);
        }
    }
}
