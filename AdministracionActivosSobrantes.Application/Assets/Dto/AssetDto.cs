using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Categories;
using AdministracionActivosSobrantes.Common;

namespace AdministracionActivosSobrantes.Assets.Dto
{
    [AutoMapFrom(typeof(Asset))]
    public class AssetDto : EntityDto<Guid>, IDtoViewBaseFields
    {

        public string Code { get; set; }

        public string Name { get; set; }

        public string Barcode { get; set; }

        public string Description { get; set; }

        public DateTime? Model { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public DateTime AdmissionDate { get; private set; }

        public double MonthDepreciation { get; private set; }

        public bool IsAToolInKit { get; set; }

        public double Price { get; set; }

        public string CategoryStr { get; set; }

        public string ResponsiblePersonStr { get; set; }

        public AssetType AssetType { get; set; }

        public Guid? CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int? ErrorCode { get; set; }
         public double? MensualDepreciation { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
        public string UltOutRequest { get; set; }
        public DateTime? DateUltOutRequest { get; set; }
        public string UltInRequest { get; set; }
        public DateTime? DateUltInRequest { get; set; }
    }
}
