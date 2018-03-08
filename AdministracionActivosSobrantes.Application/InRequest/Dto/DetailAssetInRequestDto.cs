using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Details;

namespace AdministracionActivosSobrantes.InRequest.Dto
{
    [AutoMapFrom(typeof(Detail))]
    public class DetailAssetInRequestDto: EntityDto<Guid>, IDtoViewBaseFields
    {
        public int Index { get; set; }

        public int DetailId { get; set; }

        public Guid? InRequestId { get; set; }//nullable porque puede que no sea por medio de una compra

        public Guid AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        public string NameAsset { get; set; }

        public string AssetCode { get; set; }

        public double StockAsset { get; set; }

        public string Price { get; set; }

        public int Delete { get; set; }

        public int Saved { get; set; }

        public int IsEdit { get; set; }

        public int Update { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }

        public double GetAssetAmount()
        {
            double result = 0;
            result += (StockAsset * Double.Parse(Price));
            return result;
        }

        

        public DetailAssetInRequestDto()
        {
            NameAsset = string.Empty;
        }
    }
}