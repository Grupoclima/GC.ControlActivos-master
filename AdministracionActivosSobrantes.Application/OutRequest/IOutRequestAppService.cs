using System;
using System.Collections.Generic;
using Abp.Application.Services;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Assets.Dto;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Contractors;
using AdministracionActivosSobrantes.OutRequest.Dto;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Projects.Dto;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Stocks.Dto;
using AdministracionActivosSobrantes.Users;
using MvcPaging;

namespace AdministracionActivosSobrantes.OutRequest
{
    public interface IOutRequestAppService : IApplicationService
    {
        IPagedList<OutRequestDto> SearchOutRequest(SearchOutRequestInput searchInput);
        IPagedList<Stock> GetAllStocks(string query, Guid? cellarId, Guid? assetId, TypeOutRequest? typeOutRequest, int defaultPageSize, int? page, string company);
        IList<User> GetAllWareHouseUsers();
        DetailOutRequestOutput Get(Guid id);
        CreateOutRequestInput GetEdit(Guid id);
        DetailOutRequestOutput GetDetail(Guid id);
        IList<DetailAssetOutRequestDto> GetEditDetails(Guid outRequestId, Guid cellarId, string company,int impreso);
        IList<DetailAssetCloseRequest> GetCloseRequestDetails(Guid outRequestId, Guid cellarId, string company);
        void Update(CreateOutRequestInput input);
        void DeliverRequest(CreateOutRequestInput input, IList<StockMap> list, string url);
        void CloseRequest(CloseOutRequestInput input, IList<StockMap> list);
        void Create(CreateOutRequestInput input);
        void Delete(Guid id, Guid userid,string company);
        void WaitApprovalStatus(Guid outRequestId, Guid userId, string urlActionRequest, string company);
        void ApprovedStatus(Guid outRequestId, Guid userId, string urlActionRequest, string company);
        void ProcessedInWareHouseStatus(Guid outRequestId, Guid userId, string urlActionRequest, string companyName, IList<StockMap> stockList );
        void DisprovedStatus(Guid outRequestId, Guid userId, string urlActionRequest, string company);
        int GetNextRequestNumber(string company);
        User GetUser(Guid userId);
        IList<Cellar> GetAllCellars(string company);
        IList<Cellar> GetSomeAllCellars(string company);
        IList<ResponsiblePerson.ResponsiblePerson> GetAllResponsibles(string company);
        IList<Project> GetAllProjects(string company);
        IList<Contractor> GetAllContractor(string company);
        IList<User> GetAllUsers(string company);
        Asset GetAsset(Guid id);
        Asset GetAssetBarCode(string codes, string companyName);
        ProjectDto GetProject(Guid id);
        Stock GetStockAsset(Guid id, string companyName, Guid cellarId);
        void ClosePartialRequest(Guid outRequestId, Guid assetId, double qty, Guid userId, string company, IList<StockMap> stockList);
        void UpdateEditDetailChangeStatus(CreateOutRequestInput input);
        IPagedList<ProjectDto> SearchProject(SearchProjectsInput searchInput);
        IPagedList<AssetDto> SearchAssets(SearchAssetsInput searchInput);
        IList<Details.Detail> ActualizaImpresion(Guid outRequestId);
       // IList<Cellar> GetSomeAllCellars(string company);
    }
}
