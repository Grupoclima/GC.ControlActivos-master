using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;

namespace AdministracionActivosSobrantes.Stocks.Dto
{
    [AutoMapFrom(typeof(Stock))]
    public class CreateStockInput : EntityDto<Guid>, IDtoViewBaseFields
    {
        //public double AssetQtyOutputs { get; set; }

        //public double AssetQtyInputs { get; set; }

        public Guid CellarId { get; set; }

        public Cellar Cellar { get; set; }

        public Guid AssetId { get; set; }

        public Asset Asset { get; set; }

        public double Price { get; set; }

        public double Qty { get; set; }

        //public double AmountOutput { get; set; }

        //public double AmountInput { get; set; }

        public Guid CreatorUserId { get; set; }


        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
