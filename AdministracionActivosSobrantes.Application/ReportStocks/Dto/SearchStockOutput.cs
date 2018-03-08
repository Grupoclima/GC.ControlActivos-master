using System.Collections.Generic;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Stocks;

namespace AdministracionActivosSobrantes.ReportStocks.Dto
{
    public class SearchStockOutput : IDtoViewBaseFields
    {
        public string Name { get; set; }
        public IList<Stock> Stocks{ get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
