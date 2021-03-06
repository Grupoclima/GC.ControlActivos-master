﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Categories;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.CustomFields;
using AdministracionActivosSobrantes.CustomFields.Dto;
using AdministracionActivosSobrantes.ToolAssets;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Assets.Dto
{
    [AutoMapFrom(typeof(Asset))]
    public class UpdateAssetInput : EntityDto<Guid>, IDtoViewBaseFields
    {
        [Display(Name = "Código: ")]
        [Required(ErrorMessage = "* Requerido.")]
        [StringLength(Asset.MaxCodeLength, ErrorMessage = "* El Código no puede ser mayor a 256 caracteres.")]
        public string Code { get; set; }

        [Display(Name = "Nombre: ")]
        [Required(ErrorMessage = "* Requerido.")]
        [StringLength(Asset.MaxNameLength, ErrorMessage = "* El Nombre no puede ser mayor a 256 caracteres.")]
        public string Name { get; set; }

        [Display(Name = "Código de Barras: ")]
        [StringLength(Asset.MaxBarCodeLength,
            ErrorMessage = "* El Código de Barras no puede ser mayor a 256 caracteres.")]
        public string Barcode { get; set; }

        [Display(Name = "Categoría: ")]
        public string CategoryStr { get; set; }

        [Display(Name = "Responsable: ")]
        public string ResponsiblePersonStr { get; set; }

        [Display(Name = "Descripción: ")]
        [StringLength(Asset.MaxDescriptionLength,
            ErrorMessage = "* La Descripción no puede ser mayor a 1024 caracteres.")]
        public string Description { get; set; }

        [Display(Name = "Marca: ")]
        [StringLength(Asset.MaxBrandLength)]
        public string Brand { get; set; }

        [Display(Name = "Modelo: ")]
        [StringLength(Asset.MaxModelStrLength)]
        public string ModelStr { get; set; }

        [Display(Name = "Serie: ")]
        [StringLength(Asset.MaxSeriesLength)]
        public string Series { get; set; }

        [Display(Name = "Placa: ")]
        [StringLength(Asset.MaxPlateLength)]
        public string Plate { get; set; }

        [Display(Name = "Modelo: ")]
        public DateTime? Model { get; set; }

        [Display(Name = "Fecha de Compra: ")]
        public DateTime? PurchaseDate { get; set; }

        [Display(Name = "Fecha de Ingreso: ")]
        [Required(ErrorMessage = "* Requerido.")]
        public DateTime AdmissionDate { get; set; }

        [Display(Name = "Meses Depreciables: ")]
        [Required(ErrorMessage = "* Requerido.")]
        public double DepreciationMonthsQty { get; set; }

        [Display(Name = "Valor: ")]
        [Required(ErrorMessage = "* Requerido.")]
        [Range(0, Double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
        public double Price { get; set; }

        //[Display(Name = "Cantidad Inicial: ")]
        //public double? Stock { get; set; }

        [Display(Name = "Set de Herramientas ")]
        public bool IsAToolInKit { get; set; }

        [Display(Name = "Tipo: ")]
        [Required(ErrorMessage = "* Requerido.")]
        public AssetType AssetType { get; set; }

        [Display(Name = "Categoría: ")]
        public Guid? CategoryId { get; set; }

        public virtual Category Category { get; set; }

        [Display(Name = "Ubicación: ")]
        [Required(ErrorMessage = "* Requerido.")]
        public Guid? CellarId { get; set; }

        public virtual Cellar Cellar { get; set; }

        [Display(Name = "Categorías: ")]
        public IList<Category> Categories { get; set; }

        [Display(Name = "Ubicaciones: ")]
        public IList<Cellar> Cellars { get; set; }

        [Display(Name = "Imagen: ")]
        public HttpPostedFile Image { get; set; }

        public string ImagePath { get; set; }

        public IList<CustomFieldDto> CustomFieldsDto { get; set; }

        public IList<DetailAssetToolKitsDto> DetailAssetToolKitsDto { get; set; }

        public IList<CustomField> CustomFields { get; set; }

        public IList<ToolAsset> ToolAssets { get; set; }

        public Guid? LastModifierUserId { get; set; }

        public User LastModifierUser { get; set; }

        public Guid CreatorGuidId { get; set; }

        public DateTime NowDate { get; set; } 


        private double GetMontlyDepretiationAmt()
        {
            double depAmt = Price / DepreciationMonthsQty;
            return depAmt;
        }

        private double DepretiationMonthsCount(DateTime currentDate)
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

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
        public double? MensualDepreciation { get; set; }
        public double? CostinBooks { get; set; }
        public string UltOutRequest { get; set; }
        public DateTime? DateUltOutRequest { get; set; }
        public string UltInRequest { get; set; }
        public DateTime? DateUltInRequest { get; set; }
    }
}
