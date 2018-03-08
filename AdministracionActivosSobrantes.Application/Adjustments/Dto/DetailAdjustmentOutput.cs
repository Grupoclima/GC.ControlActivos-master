using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Adjustments.Dto
{
    [AutoMapFrom(typeof(Adjustment))]
    public class DetailAdjustmentOutput : EntityDto<Guid>, IDtoViewBaseFields
    {
        public int RequestNumber { get; set; }

        [StringLength(50, ErrorMessage = "* El número documento no puede ser mayor a 50 caracteres.")]
        public string RequestDocumentNumber { get; set; }

        public TypeAdjustment TypeAdjustment { get; set; }

        public AdjustmentStatus Status { get; set; }

        public string Notes { get; set; }
        
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

        public IList<Detail> Details { get; set; }

        public IList<User> Users { get; set; }

       public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
