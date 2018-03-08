using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Filters;

namespace AdministracionActivosSobrantes.PriceChanges
{
    public class PriceChange : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
    {
        public Guid AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        public double OldPrice { get; set; }

        public double NewPrice { get; set; }

        [DataType(DataType.Date, ErrorMessage = "* El Formato de la fecha debe ser dd/MM/yyyy")]
        public DateTime DateChange { get; set; }

        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; } //Fecha a

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }

        protected PriceChange()
        {

        }

        public static PriceChange Create(Guid assetId, double oldPrice, double newPrice,
            Guid creatorid, DateTime createDateTime, string companyName)
        {
            var @priceChange = new PriceChange
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                OldPrice = oldPrice,
                NewPrice = newPrice,
                DateChange = createDateTime,
                CreationTime = createDateTime,
                CreatorUserId = creatorid,
                CompanyName = companyName,
                IsDeleted = false,
            };
            return @priceChange;
        }

        [StringLength(250)]
        public string CompanyName { get; set; }
    }
}
