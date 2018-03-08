using System;
using Abp.Application.Services;
using AdministracionActivosSobrantes.Categories.Dto;
using AdministracionActivosSobrantes.Contractors.Dto;
using MvcPaging;

namespace AdministracionActivosSobrantes.Contractors
{
    public interface IContractorAppService : IApplicationService
    {
        IPagedList<ContractorDto> SearchContractor(SearchContractorInput searchInput);
        GetContractorOutput Get(Guid id);
        UpdateContractorInput GetEdit(Guid id);
        DetailContractorOutput GetDetail(Guid id);
        void Update(UpdateContractorInput input);
        void Create(CreateContractorInput input);
        void Delete(Guid id, Guid userid);
    }
}
