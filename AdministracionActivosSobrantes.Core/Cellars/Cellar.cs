using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Filters;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Cellars
{
    public class Cellar : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
    {
        public const int MaxNameLength = 350;
        public const int MaxPhoneNumberLength = 20;
        public const int MaxAddressLength = 1000;
        public const int MaxCostCenterLength = 150;

        [Required]
        public int CellarNumber { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        [StringLength(MaxAddressLength)]
        public string Address { get; set; }

        [StringLength(MaxCostCenterLength)]
        public string CostCenter { get; set; }

        [StringLength(MaxPhoneNumberLength)]
        public string Phone { get; set; }

        public Guid WareHouseManagerId { get; set; }
        public virtual User WareHouseManagerUser { get; set; }

        public bool Active { get; set; }

        [StringLength(50)]
        public virtual string Latitude { get; set; }
        [StringLength(50)]
        public virtual string Longitude { get; set; }

        public virtual ICollection<Stock> Stocks { get; set; }

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
        protected Cellar()
        {

        }

        public static Cellar Create(string name,string address, string phone,Guid whManagerId, string latitude, 
            string longitude, Guid creatorid, DateTime createDateTime, string costCenter, string companyName)
        {
            var @cellar = new Cellar
            {
                Id = Guid.NewGuid(),
                Name = name,
                Phone = phone,
                Address = address,
                WareHouseManagerId = whManagerId,
                Latitude = latitude,
                Longitude = longitude,
                CreationTime = createDateTime,
                CreatorUserId = creatorid,
                IsDeleted = false,
                CostCenter = costCenter,
                CompanyName = companyName,
                Active = true
            };
            return @cellar;
        }

        [StringLength(250)]
        public string CompanyName { get; set; }
    }
}
