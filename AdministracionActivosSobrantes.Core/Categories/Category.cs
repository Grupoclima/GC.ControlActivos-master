using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Filters;

namespace AdministracionActivosSobrantes.Categories
{
    public class Category : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
    {
        public const int MaxNameLength = 256;
        public const int MaxDescriptionLength = 1024;

        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        [StringLength(MaxDescriptionLength)]
        public string Description { get; set; }

        //--Auditing
        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }

        /// <summary>
        /// We don't make constructor public and forcing to create clients using <see cref="Create"/> method.
        /// But constructor can not be private since it's used by EntityFramework.
        /// Thats why we did it protected.
        /// </summary>
        protected Category() { }

        public static Category Create(string name, string description, Guid creatorid, DateTime createDateTime, string companyName)
        {
            var @asset = new Category
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                CreationTime = createDateTime,
                CreatorUserId = creatorid,
                CompanyName = companyName,
                IsDeleted = false
            };
            return @asset;
        }

        [StringLength(250)]
        public string CompanyName { get; set; }
    }
}
