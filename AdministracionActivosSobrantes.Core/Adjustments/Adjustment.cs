using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Abp.Domain.Entities;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.Filters;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Adjustments
{
    public class Adjustment : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
    {
        public int RequestNumber { get; set; }

        [StringLength(50, ErrorMessage = "* El número documento no puede ser mayor a 50 caracteres.")]
        public string RequestDocumentNumber { get; set; }

        public TypeAdjustment TypeAdjustment { get; set; }

        public AdjustmentStatus Status { get; set; }
        
        [StringLength(256, ErrorMessage = "* Las notas no pueden ser mayores a 256 caracteres.")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "* Requerido.")]
        [StringLength(256, ErrorMessage = "* El Campo Responsable no puede ser mayor a 256 caracteres.")]
        public string PersonInCharge { get; set; }

        [StringLength(512, ErrorMessage = "* El Campo de Observaciones no puede ser mayor a 512 caracteres.")]
        public string Comment { get; set; }

        [DataType(DataType.Date, ErrorMessage = "* El Formato de la fecha debe ser dd/MM/yyyy")]
        public DateTime? ProcessedDate { get; set; }//Fecha Procesada

        public Guid? CellarId { get; set; }
        public virtual Cellar Cellar { get; set; }

        public Guid? ApprovalUserId { get; set; }
        public virtual User ApprovalUser { get; set; }

        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; } //Fecha a

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }
        
        public virtual ICollection<Detail> Details { get; set; }

        protected Adjustment()
        {

        }

        public static Adjustment Create(int requestNumber, string requestDocumentNumber, string notes,
            Guid? cellarId, AdjustmentStatus adjustmentStatus, TypeAdjustment typeAdjustment, Guid creatorid, DateTime createDateTime, string personInCharge, string companyName)
        {
            var @adjustment = new Adjustment
            {
                Id = Guid.NewGuid(),
                RequestNumber = requestNumber,
                RequestDocumentNumber = requestDocumentNumber,
                Notes = notes,
                CellarId = cellarId,
                Status = adjustmentStatus,
                TypeAdjustment = typeAdjustment,
                CreationTime = createDateTime,
                CreatorUserId = creatorid,
                PersonInCharge = personInCharge,
                IsDeleted = false,
                CompanyName = companyName
            };
            return @adjustment;
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

    public enum AdjustmentStatus
    {
        Active, //Activo
        Processed, //Esperando Aprobacion
        Closed //Cerrado
    }


    public enum TypeAdjustment
    {
        Asset,//is a regular asset
        LeftOver //not an asset its a left over
    }
}
