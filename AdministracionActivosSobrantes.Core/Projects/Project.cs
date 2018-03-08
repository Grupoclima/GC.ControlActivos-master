using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Filters;
using AdministracionActivosSobrantes.Movements;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Projects
{
    public class Project : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
    {
        public const int MaxNameLength = 256;
        public const int MaxDescriptionLength = 1024;
        public const int MaxCodeLength = 256;
        public const int MaxCostCenterLength = 150;

        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(MaxCodeLength)]
        public string Code { get; set; }

        [StringLength(MaxCostCenterLength)]
        public string CostCenter { get; set; }

        [StringLength(MaxDescriptionLength)]
        public string Description { get; set; }

        public EstadoProyecto EstadoProyecto { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? FinalDate { get; set; }

        public virtual ICollection<Movement> Movements { get; set; }

        public Guid? UserId { get; set; }// o hacer clase encargado
        public virtual User User { get; set; }

        public bool? IsDeleted { get; set; }

        public Guid? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }
        public DateTime? LastModificationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }
        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId{ get; set; }
        
        protected Project()
        {

        }

        public static Project Create(string name, string code, string description, DateTime? starDate, DateTime? finalDate, EstadoProyecto estado, 
            Guid userId, string costCenter, string companyName)
        {
            var @project = new Project
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = name,
                Description = description,
                FinalDate = finalDate,
                StartDate = starDate,
                EstadoProyecto = estado,
                CreatorUserId = userId,
                CostCenter = costCenter,
                CompanyName = companyName,
                IsDeleted = false
            };
            return @project;
        }

        [StringLength(250)]
        public string CompanyName { get; set; }
    }

    public enum EstadoProyecto
    {
        Active = 0,
        Fishished = 1,
        Suspended = 2
    }

}
