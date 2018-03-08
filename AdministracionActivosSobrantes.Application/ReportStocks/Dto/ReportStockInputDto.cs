using System;
using System.Collections.Generic;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Stocks.Dto;

namespace AdministracionActivosSobrantes.ReportStocks.Dto
{
    public class ReportStockInputDto : IDtoViewBaseFields
    {
        public Guid? CellarId { get; set; }

        public IEnumerable<Cellar> Cellars { get; set; }

        public string UserName { get; set; }

        public DateTime CurrentDateTime { get; set; }

        public IEnumerable<Stock> Stocks { get; set; }

        public IList<StocksMapReport> StocksList { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
