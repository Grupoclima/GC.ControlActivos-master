using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Projects;

namespace AdministracionActivosSobrantes.Adjustments.Dto
{
    [AutoMapFrom(typeof(Adjustment))]
    public class AdjustmentDto : EntityDto<Guid>, IDtoViewBaseFields
    {
        public Guid Id { get; set; }

        [Display(Name = "Tipo: ")]
        public TypeAdjustment TypeAdjustment { get; set; }

        public AdjustmentStatus Status { get; set; }

        [Display(Name = "No Solicitud: ")]
        public int RequestNumber { get; set; }

        [Display(Name = "No Solicitud Física: ")]
        public string RequestDocumentNumber { get; set; }

        [Display(Name = "Notas: ")]
        [StringLength(350, ErrorMessage = "* Las Notas Adicionales no pueden ser mayor a 350 caracteres.")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }


        [Display(Name = "Fecha Activación: ")]
        public DateTime? ActivationDate { get; set; }

        [Display(Name = "Fecha Procesada: ")]
        public DateTime? ProcessedDate { get; set; }

        [Display(Name = "Fecha Borrada: ")]
        public DateTime? DeleteDate { get; set; }

        [Display(Name = "Fecha Activación: ")]
        public DateTime? AssetsReturnDate { get; set; }

        [Display(Name = "Fecha Vencimiento: ")]
        public DateTime? DeliverDate { get; set; }

        public bool IsDelayed { get; set; }

        public string CellarName { get; set; }

        public string Username { set; get; }
        
        [Display(Name = ": ")]
        public int Availability { get; set; }

        public string Description { get; set; }

        [Display(Name = "Proyecto: ")]
        public Guid? ProjectId { get; set; }
        [Display(Name = "Proyectos: ")]
        public IList<Project> Projects { get; set; }
        public virtual Project Project { get; set; }
        
        [Required(ErrorMessage = "* Requerido.")]
        public Guid? CellarId { get; set; }
        [Display(Name = "Aplicar en Ubicación: ")]
        public IList<Cellar> Cellars { get; set; }
        public virtual Cellar Cellar { get; set; }
        
        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }

        public IList<DetailAssetAdjustmentDto> Details { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }

        public double GetStockAssetsTotal()
        {
            double result = 0;
            result += Details.Sum(d => d.StockAsset);
            return Math.Round(result);
        }

        public double GetSubTotal()
        {
            double result = 0;
            result += Details.Sum(d => d.GetAssetAmount());
            return result;
        }

        public bool IsDelete { get; set; }

        public bool IsEdit { get; set; }

        public AdjustmentDto()
        {
            Notes = String.Empty;
            Details = new List<DetailAssetAdjustmentDto>();
        }
    }
}