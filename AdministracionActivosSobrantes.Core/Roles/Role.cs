using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Filters;

namespace AdministracionActivosSobrantes.Roles
{
    public class Role : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
    {
        //--Auditing
        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }

        [StringLength(250)]
        public string RolName { get; set; }

        public int RolNumber { get; set; }

        [StringLength(250)]
        public string CompanyName { get; set; }
    }
}
