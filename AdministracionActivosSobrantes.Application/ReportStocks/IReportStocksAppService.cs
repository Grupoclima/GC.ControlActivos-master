using System.Collections.Generic;
using Abp.Application.Services;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.ReportStocks.Dto;
using AdministracionActivosSobrantes.Stocks;

namespace AdministracionActivosSobrantes.ReportStocks
{
    public interface IReportStocksAppService : IApplicationService
    {
        IEnumerable<Stock> SearchStocks(ReportStockInputDto searchInput);
        IList<Cellar> GetAllCellars(string company);
    }
}
