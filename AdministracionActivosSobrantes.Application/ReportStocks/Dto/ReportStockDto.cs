using AdministracionActivosSobrantes.Common;

namespace AdministracionActivosSobrantes.ReportStocks.Dto
{
    public class ReportStockDto : IDtoViewBaseFields
    {
        public string Name { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
