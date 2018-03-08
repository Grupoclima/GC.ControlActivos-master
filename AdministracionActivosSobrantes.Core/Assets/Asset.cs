using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using AdministracionActivosSobrantes.Categories;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.CustomFields;
using AdministracionActivosSobrantes.Filters;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.ToolAssets;

namespace AdministracionActivosSobrantes.Assets
{
    public class Asset : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
    {
        public const int MaxNameLength = 256;
        public const int MaxDescriptionLength = 1024;
        public const int MaxImagePathLength = 1024;
        public const int MaxBarCodeLength = 256;
        public const int MaxCodeLength = 256;

        public const int MaxBrandLength = 256;
        public const int MaxModelStrLength = 256;
        public const int MaxSeriesLength = 256;
        public const int MaxPlateLength = 256;

        public const int ResponsiblePersonLength = 256;
        public const int CategoryStrLength = 256;

        [Required]
        [StringLength(MaxCodeLength)]
        public string Code { get; set; }

        [StringLength(CategoryStrLength)]
        public string CategoryStr { get; set; }

        [StringLength(ResponsiblePersonLength)]
        public string ResponsiblePersonStr { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        [StringLength(MaxBarCodeLength)]
        public string Barcode { get; set; }

        [StringLength(MaxDescriptionLength)]
        public string Description { get; set; }
        
        [StringLength(MaxBrandLength)]
        public string Brand { get; set; }

        [StringLength(MaxModelStrLength)]
        public string ModelStr { get; set; }

        [StringLength(MaxSeriesLength)]
        public string Series { get; set; }

        [StringLength(MaxPlateLength)]
        public string Plate { get; set; }

        public DateTime? Model { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public DateTime AdmissionDate { get; private set; }

        [StringLength(MaxCodeLength)]
        public string ImagePath { get; private set; }

        [Required]
        public double DepreciationMonthsQty { get; private set; } // Cantidad de Meses de Depresiacion se define por el usuario
        //public double MonthDepreciation { get; private set; }

        [Required]
        public double Price { get; set; }

        public int? WarrantyPeriod { get; set; }

        public AssetType AssetType { get; set; }

        public virtual ICollection<Stock> Stocks { get; private set; }

        public virtual ICollection<CustomField> CustomFields { get; private set; }

        public virtual ICollection<ToolAsset> ToolAssets { get; private set; }

        [Required]
        public bool IsAToolInKit { get; set; }
        
        public Guid? CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }

       public double? MensualDepreciation { get; set; }

        public double? CostinBooks { get; set; }
        [StringLength(256)]
        public string UltimatePlace { get; set; }
        [StringLength(256)]
        public string UltimateResponsable{ get; set; }
        [StringLength(256)]
        public string UltOutRequest { get; set; }
        public DateTime? DateUltOutRequest { get; set; }
        [StringLength(256)]
        public string UltInRequest { get; set; }
        public DateTime? DateUltInRequest { get; set; }


        /// <summary>
        /// We don't make constructor public and forcing to create clients using <see cref="Create"/> method.
        /// But constructor can not be private since it's used by EntityFramework.
        /// Thats why we did it protected.
        /// </summary>
        protected Asset()
        {
            
        }

        public static Asset Create(string name, string description, string code, string barcode, DateTime adminssionDate, DateTime? purchaseDate, double price, double depreciationMonthsQty, AssetType assetType, Guid? categoryId, Guid creatorid, DateTime createDateTime, string brand, string modelstr, string plate, string series, bool isAToolKit,string companyName)
        {
            var @asset = new Asset
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Code = code,
                Barcode = barcode,
                AdmissionDate = adminssionDate,
                PurchaseDate = purchaseDate,
             //   Price = price,
                DepreciationMonthsQty = depreciationMonthsQty,
                AssetType = assetType,
                CategoryId = categoryId,
                CreationTime = createDateTime,
                CreatorUserId = creatorid,
                IsDeleted = false,
                IsAToolInKit = isAToolKit,
                CustomFields = new Collection<CustomField>(),
                ToolAssets = new Collection<ToolAsset>(),
                Brand = brand,
                Series = series,
                ModelStr = modelstr,
                CompanyName =  companyName,
                Plate = plate
               
            };
            return @asset;
        }

        public void SetImage(string imgString)
        {
            ImagePath = imgString;
        }

        public void SetAssetType(AssetType assetType)
        {
            AssetType = assetType;
            //dviquez: see there any other funtionality
        }

        public void SetDepretiation(DateTime adminssionDate, double depreciationMonthsQty)
        {
            AdmissionDate = adminssionDate;
            DepreciationMonthsQty = depreciationMonthsQty;
        }

        private double GetMontlyDepretiationAmt()
        {
            double depAmt = Price/DepreciationMonthsQty;
            return depAmt;
        }

        private double DepretiationMonthsCount(DateTime currentDate)
        {
            int actualMonth = currentDate.Month;
            int admitMonth = AdmissionDate.Month;
            double depreciatedMonths = (actualMonth - admitMonth);
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

        [StringLength(250)]
        public string CompanyName { get; set; }
    }
    public enum AssetType
    {
        Asset,//is a regular asset
        LeftOver //not an asset its a left over
    }
}
