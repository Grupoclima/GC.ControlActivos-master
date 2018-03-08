using System;
using System.Collections.Generic;
using Abp.Application.Services;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.InRequest.Dto;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Stocks.Dto;
using AdministracionActivosSobrantes.Users;
using MvcPaging;

namespace AdministracionActivosSobrantes.InRequest
{
    public interface IInRequestAppService : IApplicationService
    {
        IPagedList<InRequestDto> SearchInRequest(SearchInRequestInput searchInput);
        IPagedList<Stock> GetAllStocks(string query, Guid? cellarId, Guid? assetId, int defaultPageSize, int? page);
        IPagedList<Asset> SearchAssets(string query, int? page, TypeInRequest type, string company);
        DetailInRequestOutput Get(Guid id);
        CreateInRequestInput GetEdit(Guid id);
        DetailInRequestOutput GetDetail(Guid id);
        IList<DetailAssetInRequestDto> GetEditDetails(Guid inRequestId, Guid cellarId);
        void Update(CreateInRequestInput input);
        void Create(CreateInRequestInput input);
        void Delete(Guid id, Guid userid);
        void ChangeProcessedStatus(Guid inRequestId, Guid userId, string company, IList<StockMap> stockList);
        int GetNextRequestNumber(string company);
        User GetUser(Guid userId);
        IList<Cellar> GetAllCellars(string company);
        IList<User> GetAllUsers(string company);
        IList<Stock> GetAllStocks(string company);
        int TotalCount(string query, int? page, TypeInRequest type, string company);
        Asset GetAssetBarCode(string code, string companyName);
//        Stock GetStockAsset(Guid id, string companyName);
    }
}
