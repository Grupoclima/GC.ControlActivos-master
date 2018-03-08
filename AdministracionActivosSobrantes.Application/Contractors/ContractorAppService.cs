using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Contractors.Dto;
using AutoMapper;
using MvcPaging;

namespace AdministracionActivosSobrantes.Contractors
{
    class ContractorAppService : ApplicationService, IContractorAppService
    {
        private readonly IRepository<Contractor, Guid>_contractorRepository;
        private readonly IContractorManager _contractorManager;
        private readonly IDateTime _dateTime;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public ContractorAppService(IRepository<Contractor, Guid> clientRepository, IContractorManager cellarManager, IDateTime dateTime)
        {
           _contractorRepository = clientRepository;
           _contractorManager = cellarManager;
            _dateTime = dateTime;
        }

        public IPagedList<ContractorDto> SearchContractor(SearchContractorInput searchInput)
        {
            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value - 1 : 0;

            if (searchInput.Query == null)
                searchInput.Query = "";
            else
                searchInput.Query = searchInput.Query.ToLower();

            var @entities = _contractorRepository.GetAll();
            @entities = @entities.Where(c => c.IsDeleted != null && c.CompanyName.Equals(searchInput.CompanyName) && c.IsDeleted.Value == false && (c.CompleteName.ToLower().Contains(searchInput.Query) || c.CompleteName.ToLower().Equals(searchInput.Query)));

            return @entities.OrderByDescending(p => p.CompleteName).MapTo<List<ContractorDto>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount);
        }

        public GetContractorOutput Get(Guid id)
        {
            var @entity =_contractorRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el Transportista, fue borrado o no existe.");
            }
            return Mapper.Map<GetContractorOutput>(@entity);
        }

        public UpdateContractorInput GetEdit(Guid id)
        {
            var @entity =_contractorRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el Transportista, fue borrado o no existe.");
            }
            return Mapper.Map<UpdateContractorInput>(@entity);
        }

        public DetailContractorOutput GetDetail(Guid id)
        {
            var @entity =_contractorRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el Transportista, fue borrado o no existe.");
            }
            var dto = Mapper.Map<DetailContractorOutput>(@entity);
            dto.CreatorUser =_contractorManager.GetUser(dto.CreatorUserId);
            dto.LastModifierUser =_contractorManager.GetUser(dto.LastModifierUserId);
            return dto;
        }


        public void Update(UpdateContractorInput input)
        {
            var @entity =_contractorRepository.Get(input.Id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el Transportista, fue borrado o no existe.");
            }
            if (_contractorManager.ContractorExist(input.CompleteName,input.Id, input.CompanyName))
            {
                throw new UserFriendlyException("Existe un Transportista con el mismo Nombre.");
            }

            @entity.CompleteName = input.CompleteName;
            @entity.ContractorCode = input.ContractorCode;
            @entity.Email = input.Email;
            @entity.Phone = input.Phone;
            @entity.LastModificationTime = _dateTime.Now;
            @entity.LastModifierUserId = input.LastModifierUserId;

           _contractorRepository.Update(@entity);
        }

        public void Create(CreateContractorInput input)
        {
            var @entity = Contractor.Create(input.CompleteName, input.ContractorCode,input.Email,input.Phone, input.CreatorGuidId,_dateTime.Now,input.CompanyName);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo crear el Transportista.");
            }

            if (_contractorManager.ContractorExist(@entity.CompleteName, input.Id, input.CompanyName))
            {
                throw new UserFriendlyException("Existe un Transportista con el mismo Nombre.");
            }
           _contractorRepository.Insert(@entity);
        }

        public void Delete(Guid id, Guid userId)
        {
            var @entity =_contractorRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el Transportista,  fue borrado o no existe.");
            }
            @entity.IsDeleted = true;
            @entity.DeleterUserId = userId;
            @entity.DeletionTime = _dateTime.Now;
           _contractorRepository.Update(@entity);
        }
    }
}
