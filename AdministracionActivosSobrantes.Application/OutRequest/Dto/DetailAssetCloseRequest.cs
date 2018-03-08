using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Details;

namespace AdministracionActivosSobrantes.OutRequest.Dto
{
    [AutoMapFrom(typeof(Detail))]
    public class DetailAssetCloseRequest: EntityDto<Guid>
    {
        public int Index { get; set; }

        public int DetailId { get; set; }

        public Guid? OutRequestId { get; set; }//nullable porque puede que no sea por medio de una compra

        public Guid AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        public string NameAsset { get; set; }

        public double StockAsset { get; set; }

        public double Price { get; set; }

        public double ReturnAssetQty { get; set; }

        public double LeftOver { get; set; }

        public Status Status { get; set; }
        
        public double? AssetReturnPartialQty { get; set; }

        public double GetAssetAmount()
        {
            double result = 0;
            result += (StockAsset * Price);
            return result;
        }

        public DetailAssetCloseRequest()
        {
            NameAsset = string.Empty;
        }
    }
}