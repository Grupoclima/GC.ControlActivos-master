using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Categories;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Filters;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.ToolAssets
{
    public class ToolAsset : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
    {
        public const int MaxNameLength = 256;
        public const int MaxDescriptionLength = 1024;
        public const int MaxBarCodeLength = 256;
        public const int MaxCodeLength = 256;

        [Required]
        [StringLength(MaxCodeLength)]
        public string Code { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        [StringLength(MaxDescriptionLength)]
        public string Description { get; set; }

        [Required]
        public DateTime AdmissionDate { get; private set; }

        [Required]
        public double Quatity { get; set; }

        public Guid AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        //--Auditing
        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }

        protected ToolAsset() { }

        public static ToolAsset Create(string name, string description,
            string code, Guid assetId, Guid creatorUserId, DateTime creationTime, double qty, string companyName)
        {
            var @asset = new ToolAsset
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Code = code,
                CreatorUserId = creatorUserId,
                CreationTime = creationTime,
                AdmissionDate = creationTime,
                Quatity = qty,
                IsDeleted = false,
                CompanyName = companyName,
                AssetId = assetId
            };
            return @asset;
        }

        [StringLength(250)]
        public string CompanyName { get; set; }
    }
}
