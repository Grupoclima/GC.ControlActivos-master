using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using System.Collections.Generic;

namespace AdministracionActivosSobrantes.Stocks.Dto
{
    [AutoMapFrom(typeof(Stock))]
    public class StockDto : EntityDto<Guid>, IDtoViewBaseFields
    {
        public double AssetQtyOutputs { get; set; } 

        public double AssetQtyInputs { get; set; }

        public Guid CellarId { get; set; }

        public Cellar Cellar { get; set; }

        public Guid AssetId { get; set; }
        public Asset Asset { get; set; }

        public double AmountOutput { get; set; } 

        public double AmountInput { get; set; } 

        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }
     
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
        //public IList<StockDto> GetStockInformation { get; set;}

    }
}
