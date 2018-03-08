using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AdministracionActivosSobrantes.Adjustments;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Filters;

namespace AdministracionActivosSobrantes.Details
{
    public class Detail : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
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

        public double Price { get; set; }

        //----------------------------
        public double? AssetReturnQty { get; set; }
        //--
        public double? AssetReturnPartialQty { get; set; }

        //--Auditing
        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }

        public Status Status { get; set; }
        
        public Impress? Impress { get; set; }
        public DateTime? DateDispatch { get; set; }
        public double GetAmountAsset()
        {
            double result = 0;
            result += (StockAsset * Price);
            return result;
        }

        protected Detail()
        {

        }

        public static Detail Create(Guid? inRequestId, Guid? outRequestId, Guid? adjustmentId, Guid assetId, string nameAsset, double stockAsset, double price,
            Guid creatorid, DateTime createDateTime, string companyName)
        {
            var @detail = new Detail
            {
                Id = Guid.NewGuid(),
                InRequestId = inRequestId,
                OutRequestId = outRequestId,
                AdjustmentId =  adjustmentId,
                AssetId = assetId,
                NameAsset = nameAsset,
                StockAsset = stockAsset,
                Price = price,
                CreationTime = createDateTime,
                CreatorUserId = creatorid,
                Status = Status.OnRequest,
                CompanyName = companyName,
                IsDeleted = false,
                //Impreso = Impreso.Noimpreso

            };
            return @detail;
        }

        [StringLength(250)]
        public string CompanyName { get; set; }
    }

    public enum Status
    {
        OnRequest = 1,
        Approved = 2,
        Delivered = 3,
        Rejected = 4,
        Closed = 5
    }
    public enum Impress
    {
        Impreso = 1,
        Noimpreso=0
    }
}
