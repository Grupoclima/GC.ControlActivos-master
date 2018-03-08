using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Abp.Domain.Entities;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.Filters;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.InRequest
{
    public class InRequest : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
    {
        public int RequestNumber { get; set; }

        [StringLength(50, ErrorMessage = "* El número documento no puede ser mayor a 50 caracteres.")]
        public string RequestDocumentNumber { get; set; }

        [StringLength(100, ErrorMessage = "* El número de orden no puede ser mayor a 50 caracteres.")]
        public string PurchaseOrderNumber { get; set; }

        [Required(ErrorMessage = "* Requerido.")]
        [StringLength(256, ErrorMessage = "* El Campo Devuelto por no puede ser mayor a 256 caracteres.")]
        public string PersonInCharge { get; set; }

        [StringLength(512, ErrorMessage = "* El Campo de Observaciones no puede ser mayor a 512 caracteres.")]
        public string Comment { get; set; }

        public string UrlPhisicalInRequest { get; set; }

        public Guid CellarId { get; set; }
        public virtual Cellar Cellar { get; set; }

        public Guid? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public Guid? UserId { get; set; }
        public virtual User User { get; set; }

        [StringLength(20, ErrorMessage = "* El usuario no pueden ser mayores a 20 caracteres.")]
        public string Username { get; set; }

        public virtual ICollection<Detail> Details { get; set; }

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

        [StringLength(1024)]
        public string ImagePath1 { get; private set; }

        [StringLength(1024)]
        public string ImagePath2 { get; private set; }

        //[StringLength(1024)]
        //public string SignatureData { get; private set; }

        public string SignatureData { get; private set; }

        public Guid? ResponsiblePersonId { get; set; }
        public virtual ResponsiblePerson.ResponsiblePerson ResponsiblePerson { get; set; }

        //--Auditing
        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }

        public void SetSignatureData(string signature)
        {
            SignatureData = signature;
        }

        public void SetImage1(string imgString)
        {
            ImagePath1 = imgString;
        }

        public void SetImage2(string imgString)
        {
            ImagePath2 = imgString;
        }


        public double GetTotalUnidades()
        {
            double result = 0;
            result += Details.Sum(d => d.StockAsset);
            return Math.Round(result);
        }

        public double GetSubTotal()
        {
            double result = 0;
            result += Details.Sum(d => d.GetAmountAsset());
            return result;
        }

        protected InRequest()
        {
            Details = new Collection<Detail>();
        }

        public static InRequest Create(int requestNumber, string requestDocumentNumber,string purchaseOrderNumber, string notes, Guid? projectId,
            Guid cellarId, InRequestStatus status, TypeInRequest typeInRequest, Guid creatorid, 
            DateTime createDateTime, string personInCharge, string companyName, string comment)
        {
            var @inRequest = new InRequest
            {
                Id = Guid.NewGuid(),
                RequestNumber = requestNumber,
                RequestDocumentNumber = requestDocumentNumber,
                Notes = notes,
                ProjectId = projectId,
                CellarId = cellarId,
                Comment = comment,
                Status = status,
                TypeInRequest = typeInRequest,
                CreationTime = createDateTime,
                ActivationDate = createDateTime,
                CreatorUserId = creatorid,
                PurchaseOrderNumber =  purchaseOrderNumber,
                PersonInCharge = personInCharge,
                CompanyName = companyName,
                IsDeleted = false,
            };
            return @inRequest;
        }

        [StringLength(250)]
        public string CompanyName { get; set; }
    }

    public enum InRequestStatus
    {
        Active, //Activo
        Processed, //Esperando Aprobacion
        Closed //Cerrado
    }

    public enum TypeInRequest
    {
        Asset,//is a regular asset
        LeftOver //not an asset its a left over
    }
}
