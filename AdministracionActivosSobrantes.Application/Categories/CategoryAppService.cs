using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using AdministracionActivosSobrantes.Categories.Dto;
using AdministracionActivosSobrantes.Common;
using AutoMapper;
using MvcPaging;

namespace AdministracionActivosSobrantes.Categories
{
    class CategoryAppService : ApplicationService, ICategoryAppService
    {
        private readonly IRepository<Category, Guid> _categoryRepository;
        private readonly ICategoryManager _categoryManager;
        private readonly IDateTime _dateTime;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public CategoryAppService(IRepository<Category, Guid> clientRepository, ICategoryManager cellarManager, IDateTime dateTime)
        {
            _categoryRepository = clientRepository;
            _categoryManager = cellarManager;
            _dateTime = dateTime;
        }

        public IPagedList<CategoryDto> SearchCategory(SearchCategoryInput searchInput)
        {
            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value - 1 : 0;

            if (searchInput.Query == null)
                searchInput.Query = "";
            else
                searchInput.Query = searchInput.Query.ToLower();

            var @entities = _categoryRepository.GetAll();
            @entities = @entities.Where(c => c.IsDeleted != null && c.IsDeleted.Value == false && c.CompanyName.Equals(searchInput.CompanyName) &&(c.Name.ToLower().Contains(searchInput.Query) || c.Name.ToLower().Equals(searchInput.Query)));

            return @entities.OrderByDescending(p => p.Name).MapTo<List<CategoryDto>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount);
        }

        public GetcategoryOutput Get(Guid id)
        {
            var @entity = _categoryRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Categoría, fue borrada o no existe.");
            }
            return Mapper.Map<GetcategoryOutput>(@entity);
        }

        public UpdatecategoryInput GetEdit(Guid id)
        {
            var @entity = _categoryRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Categoría, fue borrada o no existe.");
            }
            return Mapper.Map<UpdatecategoryInput>(@entity);
        }

        public DetailCategoryOutput GetDetail(Guid id)
        {
            var @entity = _categoryRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Categoría, fue borrada o no existe.");
            }
            var dto = Mapper.Map<DetailCategoryOutput>(@entity);
            dto.CreatorUser = _categoryManager.GetUser(dto.CreatorUserId);
            dto.LastModifierUser = _categoryManager.GetUser(dto.LastModifierUserId);
            return dto;
        }

        public void Update(UpdatecategoryInput input)
        {
            var @entity = _categoryRepository.Get(input.Id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Categoría, fue borrada o no existe.");
            }
            if (_categoryManager.CategoryExist(input.Name,input.Id,input.CompanyName))
            {
                throw new UserFriendlyException("Existe una Categoría con el mismo Nombre.");
            }

            @entity.Name = input.Name;
            @entity.Description = input.Description;
            @entity.LastModificationTime = _dateTime.Now;
            @entity.LastModifierUserId = input.LastModifierUserId;

            _categoryRepository.Update(@entity);
        }

        public void Create(CreateCategoryInput input)
        {
            var @entity = Category.Create(input.Name, input.Description, input.CreatorGuidId,_dateTime.Now,input.CompanyName);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo crear la Categoría.");
            }

            if (_categoryManager.CategoryExist(@entity.Name, input.Id,input.CompanyName))
            {
                throw new UserFriendlyException("Existe una Categoría con el mismo Nombre.");
            }
            _categoryRepository.Insert(@entity);
        }

        public void Delete(Guid id, Guid userId)
        {
            var @entity = _categoryRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Categoría, fue borrada o no existe.");
            }
            @entity.IsDeleted = true;
            @entity.DeleterUserId = userId;
            @entity.DeletionTime = _dateTime.Now;
            _categoryRepository.Update(@entity);
        }
    }
}
