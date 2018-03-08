using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Categories;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Assets.Dto
{
    [AutoMapFrom(typeof(Asset))]
    public class DetailAssetOutput : EntityDto<Guid>, IDtoViewBaseFields
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Barcode { get; set; }

        public string ResponsiblePersonStr { get; set; }
        
        public string Description { get; set; }

        public string Brand { get; set; }

        public string ModelStr { get; set; }

        public string Series { get; set; }

        public string Plate { get; set; }

        public bool IsAToolInKit { get; set; }

        public DateTime? Model { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public DateTime AdmissionDate { get; set; }

        public double MonthDepreciation { get; set; }

        public double Price { get; set; }

        public double? MensualDepreciation { get; set; }

        public double? CostinBooks { get; set; }

        [Display(Name = "Imagen: ")]
        public HttpPostedFile Image { get; set; }

        public string ImagePath { get; set; }

        public AssetType AssetType { get;  set; }

        public double DepreciationMonthsQty { get; set; }

        private double GetMontlyDepretiationAmt()
        {
            double depAmt = Price / DepreciationMonthsQty;
            return depAmt;
        }

        public double DepretiationMonthsCount(DateTime currentDate)
        {
            int actualMonth = currentDate.Month;
            int admitMonth = AdmissionDate.Month;
            double depreciatedMonths = (actualMonth - admitMonth);
            if (depreciatedMonths < 0)
                depreciatedMonths = 0;

            return depreciatedMonths;
        }

        private double GetDepretiatedAmount(DateTime currentDate)
        {
            double montlyDepretiationAmt = GetMontlyDepretiationAmt() * DepretiationMonthsCount(currentDate);
            return montlyDepretiationAmt;
        }

        public double GetDepretiatedCurrentPrice(DateTime currentDate)
        {
            double depretiatedCurrentPrice = Price - GetDepretiatedAmount(currentDate);
            return depretiatedCurrentPrice;
        }

        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }
        public User LastModifierUser { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }
        public User CreatorUser { get; set; }

        public DateTime NowDate { get; set; }

        public int? ErrorCode { get; set; }
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
