using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Details;

namespace AdministracionActivosSobrantes.Adjustments.Dto
{
    [AutoMapFrom(typeof(Adjustment))]
    public class CreateAdjustmentInput : EntityDto<Guid>, IDtoViewBaseFields
    {
        //public Guid Id { get; set; }

        [Display(Name = "Responsable: ")]
        public string PersonInCharge { get; set; }

        [Display(Name = "No Ajuste: ")]
        public int RequestNumber { get; set; }

        [Display(Name = "Tipo: ")]
        public TypeAdjustment TypeAdjustment { get; set; }
        public int TypeAdjustmentValue { get; set; }
        
        public AdjustmentStatus Status { get; set; }
      
        [Display(Name = "No Ajuste Físico: ")]
        public string RequestDocumentNumber { get; set; }

        public Guid? CreatorGuidId { get; set; }

        [Display(Name = "Notas: ")]
        [StringLength(350, ErrorMessage = "* Las Notas Adicionales no pueden ser mayor a 350 caracteres.")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public string Description { get; set; }
        
        public bool IsDelayed { get; set; }

        [Display(Name = "Fecha Activación: ")]
        public DateTime? ActivationDate { get; set; }

        [Display(Name = "Fecha Procesada: ")]
        public DateTime? ProcessedDate { get; set; }

        [Display(Name = "Fecha Borrada: ")]
        public DateTime? DeliverDate { get; set; }

        [Display(Name = "Fecha Vencimiento: ")]
        public DateTime? AssetsReturnDate { get; set; }

        public string AssetsReturnDateString { get; set; }

        [Display(Name = "Fecha Activación: ")]
        public DateTime? CreationDate { get; set; }

        public string CellarName { get; set; }

        public Guid CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }

        

        [Required(ErrorMessage = "* Requerido.")]
        public Guid CellarId { get; set; }
        [Display(Name = "Aplicar en Ubicación: ")]
        public IList<Cellar> Cellars { get; set; }
        public virtual Cellar Cellar { get; set; }

        public IList<Detail> Details { get; set; }

        public IList<DetailAssetAdjustmentDto> DetailsAdjustment { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }

        [Required]
        public string CompanyName { get; set; }

        public double GetStockAssetsTotal()
        {
            double result = 0;
            result += DetailsAdjustment.Sum(d => d.StockAsset);
            return Math.Round(result);
        }

        public double GetSubTotal()
        {
            double result = 0;
            result += DetailsAdjustment.Sum(d => d.GetAssetAmount());
            return result;
        }

        public bool IsDelete { get; set; }

        public bool IsEdit { get; set; }

        public CreateAdjustmentInput()
        {
            Notes = String.Empty;
            DetailsAdjustment = new List<DetailAssetAdjustmentDto>();
        }
    }
}
