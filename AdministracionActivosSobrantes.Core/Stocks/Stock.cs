using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Filters;

namespace AdministracionActivosSobrantes.Stocks
{
    public class Stock : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
    {
        public double AssetQtyOutputs { get; private set; } // Assets ouputs quantity

        public double AssetQtyInputs { get; private set; }// Assets inputs quantity

        public double AssetQtyOutputsBlocked { get; private set; } // Assets ouputs quantity

        public Guid CellarId { get; set; } // Cellar or WareHouseId
        public virtual Cellar Cellar { get; set; }

        public Guid AssetId { get; set; } 
        public virtual Asset Asset { get; set; }

        //Monto en Salidas
        public double AmountOutput { get; private set; } // Es el Valor en dinero que se lleva en de las existencias o solo en los articulos que están en el inventario

        //Monto en Entradas
        public double AmountInput { get; private set; } // Es el Valor en dinero que se lleva en de las existencias o solo en los articulos que están en el inventario

        //Monto de Articulos Bloqueados
        public double AmountBlockedOutput { get; private set; } // Es el Valor en dinero que se lleva en de las existencias o solo en los articulos que están en el inventario

     /*   public Guid? UltOutRequest { get; set; }

        public DateTime? DateUltOutRequest { get; set; }
   
        public Guid? UltInRequest { get; set; }

        public DateTime? DateUltInRequest { get; set; }*/
        //--Auditing
        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }

        public Stock() { }

        public static Stock Create(Guid cellarId, Guid assetId,double qty,double price,
            Guid creatorUserId, DateTime creationTime, string companyName)
        {
            var @stock = new Stock
            {
                Id = Guid.NewGuid(),
                AssetQtyOutputs = 0,
                AssetQtyInputs = 0,
                AmountOutput = 0,
                AmountInput = 0,
                CellarId = cellarId,
                AssetId = assetId,
                CreatorUserId = creatorUserId,
                CreationTime = creationTime,
                CompanyName = companyName,
                IsDeleted = false
            };
            @stock.AddToStock(qty,price);
            return @stock;
        }

        public double GetAmountInStock()
        {
            return AmountInput - AmountOutput;
        }

        public double GetStockItemsQty()
        {
            return AssetQtyInputs - AssetQtyOutputs;
        }

        public double GetStockItemsQtyBlocked()
        {
            return AssetQtyInputs - AssetQtyOutputs - AssetQtyOutputsBlocked;
        }


        public void AddToStock(double quantity, double unitAmount)
        {
            AmountInput += quantity * unitAmount;
            AssetQtyInputs += quantity;
        }

        public void AddToBlockedQty(double quantity, double unitAmount)
        {
            AmountBlockedOutput += quantity * unitAmount;
            AssetQtyOutputsBlocked += quantity;
        }

        public void RemoveFromStock(double quantity, double unitAmount)
        {
            AmountOutput += quantity * unitAmount;
            AssetQtyOutputs += quantity;
        }

        public void RemoveFromBlockedStock(double quantity, double unitAmount)
        {
            AmountBlockedOutput -= quantity * unitAmount;
            AssetQtyOutputsBlocked -= quantity;
        }

        [StringLength(250)]
        public string CompanyName { get; set; }
    }
}
