using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Users.Dto;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Contractors;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Projects.Dto;
using AdministracionActivosSobrantes.Stocks.Dto;

namespace AdministracionActivosSobrantes.OutRequest.Dto
{
    [AutoMapFrom(typeof(OutRequest))]
    public class CreateOutRequestInput : EntityDto<Guid>, IDtoViewBaseFields
    {
        //public Guid Id { get; set; }

        [Display(Name = "No Solicitud: ")]
        public int RequestNumber { get; set; }

        [Display(Name = "Tipo: ")]
        public TypeOutRequest TypeOutRequest { get; set; }
        public int TypeOutRequestValue { get; set; }
        
        [Display(Name = "Estado Solicitud: ")]
        public OutRequestStatus Status { get; set; }
        public int StateRequest { get; set; }

        [Display(Name = "No Solicitud Física: ")]
        public string RequestDocumentNumber { get; set; }

        public string ImagePath1 { get; set; }

        public string ImagePath2 { get; set; }

        public string ImagePath3 { get; set; }

        public string ImagePath4 { get; set; }

        [Display(Name = "Imagen1: ")]
        public HttpPostedFile Image1 { get; set; }

        [Display(Name = "Imagen2: ")]
        public HttpPostedFile Image2 { get; set; }

        [Display(Name = "Imagen3: ")]
        public HttpPostedFile Image3 { get; set; }

        [Display(Name = "Imagen4: ")]
        public HttpPostedFile Image4 { get; set; }

        [Display(Name = "Observaciones: ")]
        public string Comment { get; set; }
        
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

        [Display(Name = "Fecha Requerida: ")]
        public DateTime? AssetsReturnDate { get; set; }

        public string AssetsReturnDateString { get; set; }

        public string SignatureData { get; set; }

        [Display(Name = "Fecha Activación: ")]
        public DateTime? CreationDate { get; set; }

        public string CellarName { get; set; }

        public Guid CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }

        public string Username { set; get; }

        public string UrlEmail { set; get; }

        [Required(ErrorMessage = "* Requerido.")]
        [StringLength(256, ErrorMessage = "* El Campo de la persona encargada de retirar no puede ser mayor a 256 caracteres.")]
        [Display(Name = "Encargado de Retirar: ")]
        public string DeliveredTo { get; set; }
        
        [Display(Name = ": ")]
        public int Availability { get; set; }

        [Display(Name = "Proyecto: ")]
        public Guid? ProjectId { get; set; }
        [Display(Name = "Proyecto: ")]
        public IList<ProjectDto> Projects { get; set; }
        public virtual Project Project { get; set; }

        [Display(Name = "Proyecto: ")]
        public string ProjectName { get; set; }

        [Required(ErrorMessage = "* Requerido.")]
        [Display(Name = "Responsable: ")]
        public string PersonInCharge { get; set; }
        public IList<ResponsiblePerson.ResponsiblePerson> ResponsiblesPersons { get; set; }
        public Guid? ResponsiblePersonId { get; set; }
        
        [Required(ErrorMessage = "* Requerido.")]
        public Guid CellarId { get; set; }
        [Display(Name = "Bodegas")]
        public IList<Cellar> Cellars { get; set; }
        public virtual Cellar Cellar { get; set; }

        public Guid? ContractorId { get; set; }
        [Display(Name = "Transportista: ")]
        public IList<Contractor> Contractors { get; set; }
        public virtual Contractor Contractor { get; set; }

        public IList<Detail> Details { get; set; }

        public IList<StockMap> StockMaps { get; set; }

        public IList<DetailAssetOutRequestDto> DetailsRequest { get; set; }

        public IList<UserDto> User { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }

        public string Department { get; set; }

        public string DepreciableCost { get; set; }

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

        public CreateOutRequestInput()
        {
            Notes = String.Empty;
            DetailsRequest = new List<DetailAssetOutRequestDto>();
            StockMaps = new List<StockMap>();
        }
    }
}
