using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Filters;

namespace AdministracionActivosSobrantes.Contractors
{
    public class Contractor : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
    {
        public const int MaxCompleteNameLength = 128;
        public const int MaxEmailLength = 256;
        public const int MaxCodeLength = 64;
        public const int MaxPhoneLength = 48;

        [Required]
        [StringLength(MaxCodeLength)]
        public string ContractorCode { get; set; }

        [Required]
        [StringLength(MaxCompleteNameLength)]
        public string CompleteName { get; set; }

        [StringLength(MaxEmailLength)]
        public string Email { get; set; }

        [StringLength(MaxPhoneLength)]
        public string Phone { get; set; }

        //--Auditing
        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }

        [StringLength(250)]
        public string CompanyName { get; set; }

        protected Contractor() { }

        public static Contractor Create(string name, string code, string email, string phone, Guid creatorid, DateTime createDateTime, string companyName)
        {
            var @entity = new Contractor
            {
                Id = Guid.NewGuid(),
                CompleteName = name,
                ContractorCode = code,
                Email = email,
                Phone = phone,
                CreationTime = createDateTime,
                CreatorUserId = creatorid,
                CompanyName = companyName,
                IsDeleted = false
            };
            return @entity;
        }

    }
}
