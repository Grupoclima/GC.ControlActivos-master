using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AdministracionActivosSobrantes.Adjustments;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.Filters;
using AdministracionActivosSobrantes.OutRequest;
using AdministracionActivosSobrantes.InRequest;

namespace AdministracionActivosSobrantes.HistoryChanges
{
    public class HistoryChange : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
    {
        public Guid? InRequestId { get; set; }
        public virtual InRequest.InRequest InRequest { get; set; }

        public Guid? AdjustmentId { get; set; }
        public virtual Adjustment Adjustment { get; set; }

        public Guid? OutRequestId { get; set; }
        public virtual OutRequest.OutRequest OutRequest { get; set; }

        public Guid AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        [StringLength(100, ErrorMessage = "* El Nombre no puede ser mayor a 100 caracteres.")]
        public string NameAsset { get; set; }

        public double StockAsset { get; set; }

        public double PreviousQty { get; set; }

        public double Price { get; set; }

        [StringLength(800, ErrorMessage = "* El Url no puede ser mayor a 800 caracteres.")]
        public string Url { get; set; }

        public OutRequestStatus? OutRequestStatus { get; set; }

        public InRequestStatus? InRequestStatus { get; set; }

        public AdjustmentStatus? AdjustmentStatus { get; set; }

        //--Auditing
        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }

        public HistoryStatus Status { get; set; }

        public double GetAmountAsset()
        {
            double result = 0;
            result += (StockAsset * Price);
            return result;
        }

        protected HistoryChange()
        {

        }

        public static HistoryChange Create(Guid? inRequestId, Guid? outRequestId, Guid? adjustmentId, Guid assetId, string nameAsset, double stockAsset, double price,
            Guid creatorid, DateTime createDateTime,string companyName, double previousQty, HistoryStatus status)
        {
            var @historyChange = new HistoryChange
            {
                Id = Guid.NewGuid(),
                InRequestId = inRequestId,
                OutRequestId = outRequestId,
                AdjustmentId =  adjustmentId,
                AssetId = assetId,
                NameAsset = nameAsset,
                StockAsset = stockAsset,
                PreviousQty = previousQty,
                Status = status,
                Price = price,
                CreationTime = createDateTime,
                CreatorUserId = creatorid,
                CompanyName = companyName,
                Url = "",
                IsDeleted = false
            };
            return @historyChange;
        }

        [StringLength(250)]
        public string CompanyName { get; set; }
    }


    public enum HistoryStatus
    {
        Modified = 1,
        WithoutChanges = 2,
        Removed = 3,
    }

}
