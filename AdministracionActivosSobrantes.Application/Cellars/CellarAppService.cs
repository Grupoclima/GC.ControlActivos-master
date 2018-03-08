using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using AdministracionActivosSobrantes.Cellars.Dto;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Users;
using AutoMapper;
using MvcPaging;

namespace AdministracionActivosSobrantes.Cellars
{
    class CellarAppService : ApplicationService, ICellarAppService
    {
        private readonly IRepository<Cellar, Guid> _cellarRepository;
        private readonly ICellarManager _cellarManager;
        private readonly IDateTime _dateTime;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public CellarAppService(IRepository<Cellar, Guid> clientRepository, ICellarManager cellarManager, IDateTime dateTime)
        {
            _cellarRepository = clientRepository;
            _cellarManager = cellarManager;
            _dateTime = dateTime;
        }

        public IList<User> GetAllWareHouseUsers(string company)
        {
            return _cellarManager.GetAllWareHouseUsers(company);
        }

        public IList<Cellar> GetAllCellar(string company)
        {
            return _cellarManager.GetAllCellars(company);
        }

        public IPagedList<CellarDto> SearchCellar(SearchCellarsInput searchInput)
        {
            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value - 1 : 0;

            if (searchInput.Query == null)
                searchInput.Query = "";
            else
                searchInput.Query = searchInput.Query.ToLower();

            var @entities = _cellarRepository.GetAll();

            @entities = @entities.Where(c => c.IsDeleted != null && c.CompanyName.Equals(searchInput.CompanyName) && c.IsDeleted.Value == false && (c.Name.ToLower().Contains(searchInput.Query)
            || c.Name.ToLower().Equals(searchInput.Query)
            || c.Address.ToLower().Contains(searchInput.Query)
            || c.Address.ToLower().Equals(searchInput.Query)
            || c.Phone.ToLower().Contains(searchInput.Query)
            || c.Phone.ToLower().Equals(searchInput.Query)));

            return @entities.OrderByDescending(p => p.Name).MapTo<List<CellarDto>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount);
        }

        public GetCellarOutput Get(Guid id)
        {
            var @entity = _cellarRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            return Mapper.Map<GetCellarOutput>(@entity);
        }

        public UpdateCellarInput GetEdit(Guid id)
        {
            var @entity = _cellarRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            return Mapper.Map<UpdateCellarInput>(@entity);
        }

        public DetailCellarOutput GetDetail(Guid id)
        {
            var @entity = _cellarRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            var dto = Mapper.Map<DetailCellarOutput>(@entity);
            dto.CreatorUser = _cellarManager.GetUser(dto.CreatorUserId);
            dto.LastModifierUser = _cellarManager.GetUser(dto.LastModifierUserId);
            return dto;
        }


        public void Update(UpdateCellarInput input)
        {
            var @entity = _cellarRepository.Get(input.Id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            if (_cellarManager.CellarExist(input.Name, input.Id,input.CompanyName))
            {
                throw new UserFriendlyException("Existe una Ubicación con el mismo Nombre.");
            }

            @entity.Name = input.Name;
            @entity.Address = input.Address;
            @entity.Phone = input.Phone;
            @entity.WareHouseManagerId = input.WareHouseManagerId;
            @entity.Latitude = input.Latitude;
            @entity.Longitude = input.Longitude;
            @entity.CostCenter = input.CostCenter;
            @entity.LastModificationTime = _dateTime.Now;
            @entity.LastModifierUserId = input.LastModifierUserId;

            _cellarRepository.Update(@entity);
        }

        public void Create(CreateCellarInput input)
        {
            var @entity = Cellar.Create(input.Name, input.Address, input.Phone, input.WareHouseManagerId, input.Latitude, input.Latitude,
                input.CreatorGuidId, _dateTime.Now, input.CostCenter, input.CompanyName);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo crear la Ubicación.");
            }

            if (_cellarManager.CellarExist(@entity.Name, input.Id,input.CompanyName))
            {
                throw new UserFriendlyException("Existe una Ubicación con el mismo Nombre.");
            }
            _cellarRepository.Insert(@entity);
        }

        public void Delete(Guid id, Guid userId)
        {
            var @entity = _cellarRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            @entity.IsDeleted = true;
            @entity.DeleterUserId = userId;
            @entity.DeletionTime = _dateTime.Now;
            _cellarRepository.Update(@entity);
        }

        public void ChangeStatus(Guid id, Guid userId)
        {
            var @entity = _cellarRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            if (@entity.Active)
                @entity.Active = false;
            else
                @entity.Active = true;
            @entity.LastModifierUserId = userId;
            @entity.LastModificationTime = _dateTime.Now;
            _cellarRepository.Update(@entity);
        }
    }
}
