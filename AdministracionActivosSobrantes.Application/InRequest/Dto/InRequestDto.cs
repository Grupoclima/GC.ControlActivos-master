using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.InRequest.Dto
{
   [AutoMapFrom(typeof(InRequest))]
    public class InRequestDto : EntityDto<Guid>, IDtoViewBaseFields
    {
        public Guid Id { get; set; }

        public int RequestNumber { get; set; }

        [StringLength(50, ErrorMessage = "* El número documento no puede ser mayor a 50 caracteres.")]
        public string RequestDocumentNumber { get; set; }

        public Guid? UserId { get; set; }
        public virtual User User { get; set; }

        [StringLength(256, ErrorMessage = "* Las notas no pueden ser mayores a 256 caracteres.")]
        public string Notes { get; set; }

        public InRequestStatus Status { get; set; }

        public TypeInRequest TypeInRequest { get; set; }

        [DataType(DataType.Date, ErrorMessage = "* El Formato de la fecha debe ser dd/MM/yyyy")]
        public DateTime? ActivationDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "* El Formato de la fecha debe ser dd/MM/yyyy")]
        public DateTime? ProcessedDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "* El Formato de la fecha debe ser dd/MM/yyyy")]
        public DateTime? DeleteDate { get; set; }

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

        public IList<DetailAssetInRequestDto> Details { get; set; }

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

        public InRequestDto()
        {
            Notes = String.Empty;
            Details = new List<DetailAssetInRequestDto>();
        }
    }
}