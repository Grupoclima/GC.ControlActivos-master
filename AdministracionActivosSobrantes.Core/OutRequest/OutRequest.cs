using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Abp.Domain.Entities;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Contractors;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.Filters;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.OutRequest
{
    public class OutRequest : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
    {
        public int RequestNumber { get; set; }

        [StringLength(50, ErrorMessage = "* El número documento no puede ser mayor a 50 caracteres.")]
        public string RequestDocumentNumber { get; set; }

        public OutRequestStatus Status { get; set; }

        public TypeOutRequest TypeOutRequest { get; set; }

        public string SignatureData { get; private set; }

        [Required(ErrorMessage = "* Requerido.")]
        [StringLength(150, ErrorMessage = "* La Descripción no puede ser mayor a 150 caracteres.")]
        public string Description { get; set; }

        //[Required(ErrorMessage = "* Requerido.")]
        //[StringLength(256, ErrorMessage = "* El Campo Responsable no puede ser mayor a 256 caracteres.")]
        //public string PersonInCharge { get; set; }

        [StringLength(256, ErrorMessage = "* El Campo de la persona encargada de retirar no puede ser mayor a 256 caracteres.")]
        public string DeliveredTo { get; set; }

        [StringLength(512, ErrorMessage = "* El Campo de Observaciones no puede ser mayor a 512 caracteres.")]
        public string Comment { get; set; }
        
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

        public Guid? ResponsiblePersonId { get; set; }
        public virtual ResponsiblePerson.ResponsiblePerson ResponsiblePerson { get; set; }

        public Guid? WareHouseManId { get; set; }
        public virtual User WareHouseMan { get; set; }

        public Guid? ContractorId { get; set; }
        public virtual Contractor Contractor { get; set; }

        public Guid? ApprovalUserId { get; set; }
        public virtual User ApprovalUser { get; set; }

        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; } //Fecha a

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }

        [StringLength(1024)]
        public string ImagePath1 { get; private set; }          

        [StringLength(1024)]
        public string ImagePath2 { get; private set; }

        [StringLength(1024)]
        public string ImagePath3 { get; private set; }

        [StringLength(1024)]
        public string ImagePath4 { get; private set; }

        public virtual ICollection<Detail> Details { get; set; }

        [StringLength(256)]
        public string Department { get; set; }

        [StringLength(256)]
        public string DepreciableCost { get; set; }

        protected OutRequest()
        {
            
        }

        public static OutRequest Create(int requestNumber, string requestDocumentNumber, string description, string notes, Guid? projectId,
            Guid? cellarId, DateTime? dueDate, OutRequestStatus status, TypeOutRequest typeOutRequest, 
            Guid personInCharge, string deliveredTo, Guid? contractorId, 
            Guid creatorid, DateTime createDateTime,string companyName, string comment,string departa,string depreciableCost)
        {
         
            var @outRequest = new OutRequest
            {
                Id = Guid.NewGuid(),
                RequestNumber = requestNumber,
                RequestDocumentNumber = requestDocumentNumber,
                Description = description,
                Notes = notes,
                ProjectId = projectId,
                CellarId = cellarId,
                AssetsReturnDate = dueDate,
                DeliverDate = dueDate,
                Status = status,
                TypeOutRequest = typeOutRequest,
                CreationTime = createDateTime,
                CreatorUserId = creatorid,
                Comment = comment,
                ResponsiblePersonId = personInCharge,
                DeliveredTo = deliveredTo,
                ContractorId = contractorId,
                CompanyName = companyName,
                IsDeleted = false,
                IsDelayed = false,
                Department = departa,
                DepreciableCost = depreciableCost
            };
            return @outRequest;
        }

        public void SetImage1(string imgString)
        {
            ImagePath1 = imgString;
        }

        public void SetImage2(string imgString)
        {
            ImagePath2 = imgString;
        }
        public void SetImage3(string imgString)
        {
            ImagePath3 = imgString;
        }
        public void SetImage4(string imgString)
        {
            ImagePath4 = imgString;
        }

        public void SetSignatureData(string signature)
        {
            SignatureData = signature;
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

        [StringLength(250)]
        public string CompanyName { get; set; }
    }

    public enum OutRequestStatus
    {
        Draft, //Borrador
        Active, //Activo
        Backorder,
        WaitApproval, //Esperando Aprobacion
        Approved, // Aprobada
        WaitAssetsReturn, //Esperando que entreguen el activo
        PartialAssetsReturn,
        ProcessedInWareHouse,//Entregado
        Disproved,
        Confirmado,  //Cerrado
        ImpresoyEntregado
    }

    public enum TypeOutRequest
    {
        Asset,//is a regular asset
        LeftOver //not an asset its a left over
    }
}
