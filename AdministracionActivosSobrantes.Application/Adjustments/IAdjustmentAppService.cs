using System;
using System.Collections.Generic;
using Abp.Application.Services;
using AdministracionActivosSobrantes.Adjustments.Dto;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Stocks.Dto;
using AdministracionActivosSobrantes.Users;
using MvcPaging;

namespace AdministracionActivosSobrantes.Adjustments
{
    public interface IAdjustmentAppService : IApplicationService
    {
        IPagedList<AdjustmentDto> SearchAdjustment(SearchAdjustmentInput searchInput);
        IPagedList<Stock> GetAllStocks(string query, Guid? cellarId, TypeAdjustment? typeAdjustment, int defaultPageSize, int? page, string company);
        IList<User> GetAllWareHouseUsers();
        DetailAdjustmentOutput Get(Guid id);
        CreateAdjustmentInput GetEdit(Guid id);
        DetailAdjustmentOutput GetDetail(Guid id);
        IList<DetailAssetAdjustmentDto> GetEditDetails(Guid outRequestId, Guid cellarId);
        void Update(CreateAdjustmentInput input);
        void Create(CreateAdjustmentInput input);
        void Delete(Guid id, Guid userid,string company);
        int GetNextRequestNumber(string company);
        int GetTotalItem(string query, Guid? cellarId, TypeAdjustment? typeAdjustment, string company);
        User GetUser(Guid userId);
        IList<Cellar> GetAllCellars(string company);
        void ChangeProcessedStatus(Guid adjustmentId, Guid userId, string company, IList<StockMap> stockList);
        IList<User> GetAllUsers(string company);
    }
}
