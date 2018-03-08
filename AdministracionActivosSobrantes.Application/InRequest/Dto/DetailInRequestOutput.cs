using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.InRequest.Dto
{
    [AutoMapFrom(typeof(InRequest))]
    public class DetailInRequestOutput : EntityDto<Guid>, IDtoViewBaseFields
    {
        [StringLength(100, ErrorMessage = "* El número de orden no puede ser mayor a 50 caracteres.")]
        public string PurchaseOrderNumber { get; set; }

        public string PersonInCharge { get; set; }

        public int RequestNumber { get; set; }

        [StringLength(50, ErrorMessage = "* El número documento no puede ser mayor a 50 caracteres.")]
        public string RequestDocumentNumber { get; set; }

        public InRequestStatus Status { get; set; }

        public TypeInRequest TypeInRequest { get; set; }

        [Required(ErrorMessage = "* Requerido.")]
        [StringLength(150, ErrorMessage = "* La Descripción no puede ser mayor a 150 caracteres.")]
        public string Description { get; set; }

        [StringLength(256, ErrorMessage = "* Las notas no pueden ser mayores a 256 caracteres.")]
        public string Notes { get; set; }

        [DataType(DataType.Date, ErrorMessage = "* El Formato de la fecha debe ser dd/MM/yyyy")]
        public DateTime? DeliverDate { get; set; }//Fecha de Despacho

        [DataType(DataType.Date, ErrorMessage = "* El Formato de la fecha debe ser dd/MM/yyyy")]
        public DateTime? AssetsReturnDate { get; set; }//Fecha de Entrega o Retorno de Materiales


        [DataType(DataType.Date, ErrorMessage = "* El Formato de la fecha debe ser dd/MM/yyyy")]
        public DateTime? AprovedDate { get; set; }//Fecha de Entrega o Retorno de Materiales

        public bool IsDelayed { get; set; }

        public Guid? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public Guid? CellarId { get; set; }
        public virtual Cellar Cellar { get; set; }

        public Guid? WareHouseManId { get; set; }
        public virtual User WareHouseMan { get; set; }

        public Guid? ApprovalUserId { get; set; }
        public virtual User ApprovalUser { get; set; }

        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; } //Fecha a

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }

        public IList<Detail> Details { get; set; }

        public IList<User> Users { get; set; }


        public string ImagePath1 { get; set; }

        public string ImagePath2 { get; set; }

        public string SignatureData { get; set; }

        [StringLength(512, ErrorMessage = "* El Campo de Observaciones no puede ser mayor a 512 caracteres.")]
        public string Comment { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
