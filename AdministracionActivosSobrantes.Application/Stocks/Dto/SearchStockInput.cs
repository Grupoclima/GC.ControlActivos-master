using System;
using AdministracionActivosSobrantes.Common;
using MvcPaging;
using AdministracionActivosSobrantes.Assets.Dto;

namespace AdministracionActivosSobrantes.Stocks.Dto
{
    public class SearchStockInput : IDtoViewBaseFields
    {
        public Guid? UserId { get; set; }

        public const int DefaultPageSize = 10;

        public int? Page { get; set; }

        public int MaxResultCount { get; set; }

        public int TotalItem { get; set; }

        public Guid? CellarId { get; set; }

        public string TypeAdjustment { get; set; }

        public IPagedList<Stock> Entities { get; set; }

        public SearchStockInput()
        {
            MaxResultCount = DefaultPageSize;
        }


        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}


