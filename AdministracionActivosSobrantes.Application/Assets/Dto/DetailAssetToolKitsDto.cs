using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.ToolAssets;

namespace AdministracionActivosSobrantes.Assets.Dto
{
    [AutoMapFrom(typeof(ToolAsset))]
    public class DetailAssetToolKitsDto: EntityDto<Guid>, IDtoViewBaseFields
    {
        public int Index { get; set; }

        public int DetailId { get; set; }

        public Guid AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public double Quatity { get; set; }

        public int Saved { get; set; }

        public int Update { get; set; }

        public int Delete { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }


        public DetailAssetToolKitsDto()
        {
            Name = string.Empty;
        }
    }
}