using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Filters;

namespace AdministracionActivosSobrantes.ResponsiblePerson
{
    public class ResponsiblePerson : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
    {
        public const int MaxNameLength = 350;
        
        [StringLength(MaxNameLength)]
        public string Responsable { get; set; }

        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        public Guid? DeleterUserId { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Guid? LastModifierUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid CreatorUserId { get; set; }
        
        [StringLength(250)]
        public string CompanyName { get; set; }
        
        

        /// <summary>
        /// We don't make constructor public and forcing to create entity using <see cref="Create"/> method.
        /// But constructor can not be private since it's used by EntityFramework.
        /// Thats why we did it protected.
        /// </summary>
        protected ResponsiblePerson() { }

        public static ResponsiblePerson Create(string name, string responsable, Guid creatorid, DateTime createDateTime, string companyName)
        {
            var @entity = new ResponsiblePerson
            {
                Id = Guid.NewGuid(),
                Responsable = responsable,
                Name = name,
                CreationTime = createDateTime,
                CreatorUserId = creatorid,
                CompanyName = companyName,
                IsDeleted = false
            };
            return @entity;
        }
    }
}
