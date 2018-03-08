using Abp.Application.Services;
using AdministracionActivosSobrantes.Stocks.Dto;
using System;
using System.Collections.Generic;

namespace AdministracionActivosSobrantes.Stocks
{
    public interface IStockAppService : IApplicationService
    {
        //IPagedList<StockDto> SearchStock(SearchStocksInput searchInput);
        //IList<User> GetAllWareHouseUsers();
        //GetStockOutput Get(Guid id);
        //UpdateStockInput GetEdit(Guid id);
        //DetailStockOutput GetDetail(Guid id);
        //void Update(UpdateStockInput input);
        void Create(CreateStockInput input);
        IList<Stock> GetStockInformation(Guid id, string company);
        //void Delete(Guid id, Guid userid);
    }
}
