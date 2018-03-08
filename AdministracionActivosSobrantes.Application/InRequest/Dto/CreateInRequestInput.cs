using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.Projects;

namespace AdministracionActivosSobrantes.InRequest.Dto
{
    [AutoMapFrom(typeof(InRequest))]
    public class CreateInRequestInput : EntityDto<Guid>, IDtoViewBaseFields
    {
        //public Guid Id { get; set; }

        [Display(Name = "No Solicitud: ")]
        public int RequestNumber { get; set; }

        public string ImagePath1 { get; set; }

        public string ImagePath2 { get; set; }

        [Display(Name = "Imagen1: ")]
        public HttpPostedFile Image1 { get; set; }

        [Display(Name = "Imagen2: ")]
        public HttpPostedFile Image2 { get; set; }

        public string SignatureData { get; set; }

        [StringLength(100, ErrorMessage = "* El número de orden no puede ser mayor a 50 caracteres.")]
        [Display(Name = "No Orden de Compra: ")]
        public string PurchaseOrderNumber { get; set; }

        [Display(Name = "Devuelto por: ")]
        public string PersonInCharge { get; set; }

        [Display(Name = "Observaciones: ")]
        public string Comment { get; set; }

        [Display(Name = "Tipo: ")]
        public TypeInRequest TypeInRequest { get; set; }
        public int TypeInRequestValue { get; set; }
        
        [Display(Name = "Estado Solicitud: ")]
        public InRequestStatus Status { get; set; }
        public int StateRequest { get; set; }

        [Display(Name = "No Solicitud Física: ")]
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

        public string Username { set; get; }
        
        [Display(Name = ": ")]
        public int Availability { get; set; }

        [Display(Name = "Proyecto: ")]
        public Guid? ProjectId { get; set; }
        [Display(Name = "Proyectos: ")]
        public IList<Project> Projects { get; set; }
        public virtual Project Project { get; set; }

        [Required(ErrorMessage = "* Requerido.")]
        public Guid CellarId { get; set; }
        [Display(Name = "Aplicar en Ubicación: ")]
        public IList<Cellar> Cellars { get; set; }
        public virtual Cellar Cellar { get; set; }

        public IList<Detail> Details { get; set; }

        public IList<DetailAssetInRequestDto> DetailsRequest { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }

        public double GetStockAssetsTotal()
        {
            double result = 0;
            result += DetailsRequest.Sum(d => d.StockAsset);
            return Math.Round(result);
        }

        public double GetSubTotal()
        {
            double result = 0;
            result += DetailsRequest.Sum(d => d.GetAssetAmount());
            return result;
        }

        public bool IsDelete { get; set; }

        public bool IsEdit { get; set; }

        public CreateInRequestInput()
        {
            Notes = String.Empty;
            PurchaseOrderNumber = String.Empty;
            DetailsRequest = new List<DetailAssetInRequestDto>();
        }
    }
}
