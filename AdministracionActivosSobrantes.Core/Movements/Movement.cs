using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using AdministracionActivosSobrantes.Adjustments;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Filters;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Movements
{
    public class Movement : Entity<Guid>, IFullAuditedCustom, ITenantCompanyName
    {
        public int MovementNumber { get; set; }

        public double StockMovement { get; set; }

        public double Price { get; set; }

        public DateTime ApplicationDateTime { get; set; }

        public StatusMovement Status { get; set; }

        public TypeMovement TypeMovement { get; set; }

        public MovementCategory MovementCategory { get; set; }

        //-------------------------------------------------------------------------
        //---------------------- Montos Generales --------------------
        // Estado de los datos en existencias y montos del articulo antes del movimiento
        //referencia a CntAnteriorExistenciaBodega 
        public double PreviosCellarQty { get; set; }// no se esta usando eliminar mas adelante

        //refrerencia a MontoAnteriorExistenciaBodega { get; set; }
        public double PreviousCellarAmount { get; set; }

        //-----------------
        // Estado de totales del inventario del artículo antes del movimiento
        public double PreviousCellarStockQtyInv { get; set; } // no se esta usando eliminar mas adelante

        public double PreviousCellarStockAmountInv { get; set; }

        // Estado de totales generales del inventario y Ubicación antes del movimiento
        public double PreviousGeneralInvAmount { get; set; }
        public double PreviousGeneralStockAmount { get; set; }

        public Guid CellarId { get; set; }
        public virtual Cellar Cellar { get; set; }

        public Guid AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        public Guid? UserId { get; set; }
        public virtual User User { get; set; }

        public Guid? InRequestId { get; set; }
        public virtual InRequest.InRequest InRequest { get; set; }

        public Guid? OutRequestId { get; set; }
        public virtual OutRequest.OutRequest OutRequest { get; set; }

        public Guid? AdjustmentId { get; set; }
        public virtual Adjustment Adjustment { get; set; }

        public Guid? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        //--Auditing
        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }

        protected Movement()
        {

        }

        public static Movement Create(int numberMovement, double stock, double price, DateTime aplicationDate, StatusMovement status,TypeMovement type,
            MovementCategory movementCategory, double previosCellarQty, double previousCellarAmount, double previousCellarStockQtyInv, double previousCellarStockAmountInv,
            double previousGeneralInvAmount, double previousGeneralStockAmount, Guid cellarId, Guid assetId, Guid? userId, Guid? inRequestId,
            Guid? outRequestId, Guid? adjustmentId, Guid? projectId, Guid creatorid, DateTime createDateTime,string companyName)
        {
            var @movement = new Movement
            {
                Id = Guid.NewGuid(),
                MovementNumber = numberMovement,
                StockMovement = stock,
                Price = price,
                ApplicationDateTime = aplicationDate,
                Status = status,
                TypeMovement = type,
                MovementCategory = movementCategory,
                PreviosCellarQty = previosCellarQty,
                PreviousCellarAmount = previousCellarAmount,
                PreviousCellarStockQtyInv = previousCellarStockQtyInv,
                PreviousCellarStockAmountInv = previousCellarStockAmountInv,
                PreviousGeneralInvAmount = previousGeneralInvAmount,
                PreviousGeneralStockAmount = previousGeneralStockAmount,
                CellarId = cellarId,
                AssetId = assetId,
                UserId = userId,
                InRequestId = inRequestId,
                OutRequestId = outRequestId,
                AdjustmentId = adjustmentId,
                ProjectId = projectId,
                CreationTime = createDateTime,
                CreatorUserId = creatorid,
                CompanyName = companyName,
                IsDeleted = false

            };
            return @movement;
        }

        public double GetAmountMovement()
        {
            double result = 0;
            result += StockMovement * Price;
            return result;
        }

        //    public double GetCantidadIventarioDespuesMovimiento()
        //    {
        //        double result = 0;
        //        if (TipoMovimiento.TipoMovimiento.Value == (int)Model.TipoMovimiento.Entrada)
        //            result = CntAntExisteciasInvMigrado + CantidadMovimientoMigrado;
        //        else if (TipoMovimiento.TipoMovimiento.Value == (int)Model.TipoMovimiento.Salida)
        //            result = CntAntExisteciasInvMigrado - CantidadMovimientoMigrado;
        //        return result;
        //    }

        public double GetNewCellarQty()
        {
            double result = 0;
            if (TypeMovement.Input == TypeMovement)
                result = PreviosCellarQty + StockMovement;
            else  if (TypeMovement.Output == TypeMovement)
                    result = PreviosCellarQty - StockMovement;
            return result;
        }


        //    public double GetMontoInventarioDespuesMovimiento()
        //    {
        //        double result = 0;
        //        if (TipoMovimiento.TipoMovimiento.Value == (int)Model.TipoMovimiento.Entrada)
        //            result = MontoGeneralAntInventario + GetMontoMovimiento();
        //        else if (TipoMovimiento.TipoMovimiento.Value == (int)Model.TipoMovimiento.Salida)
        //            result = MontoGeneralAntInventario - GetMontoMovimiento();
        //        return result;
        //    }

        //    public double GetMontoBodegaDespuesMovimiento()
        //    {
        //        double result = 0;
        //        if (TipoMovimiento.TipoMovimiento.Value == (int)Model.TipoMovimiento.Entrada)
        //            result = MontoGeneralAntBodega + GetMontoMovimiento();
        //        else if (TipoMovimiento.TipoMovimiento.Value == (int)Model.TipoMovimiento.Salida)
        //            result = MontoGeneralAntBodega - GetMontoMovimiento();
        //        return result;
        //    }

        //    /// <summary>
        //    /// NO ACTIVOS
        //    /// </summary>
        //    /// <returns></returns>

        //    public double GetMontoInventarioDespuesMovimientoNoActivo()
        //    {
        //        double result = 0;
        //        if (TipoMovimiento.TipoMovimiento.Value == (int)Model.TipoMovimiento.Entrada)
        //        {
        //            if (MontoGeneralAntInventarioNoActivo != null)
        //                result = (double)MontoGeneralAntInventarioNoActivo + GetMontoMovimiento();
        //        }
        //        else if (TipoMovimiento.TipoMovimiento.Value == (int)Model.TipoMovimiento.Salida)
        //            if (MontoGeneralAntInventarioNoActivo != null)
        //                result = (double)MontoGeneralAntInventarioNoActivo - GetMontoMovimiento();
        //        return result;
        //    }

        //    public double GetMontoBodegaDespuesMovimientoNoActivo()
        //    {
        //        double result = 0;
        //        if (TipoMovimiento.TipoMovimiento.Value == (int)Model.TipoMovimiento.Entrada)
        //        {
        //            if (MontoGeneralAntBodegaNoActivo != null)
        //                result = (double)MontoGeneralAntBodegaNoActivo + GetMontoMovimiento();
        //        }
        //        else if (TipoMovimiento.TipoMovimiento.Value == (int)Model.TipoMovimiento.Salida)
        //            if (MontoGeneralAntBodegaNoActivo != null)
        //                result = (double)MontoGeneralAntBodegaNoActivo - GetMontoMovimiento();
        //        return result;
        //    }

        //    public double GetMontoBodegaxArticuloDespuesMovimiento()
        //    {
        //        double result = 0;
        //        if (TipoMovimiento.TipoMovimiento.Value == (int)Model.TipoMovimiento.Entrada)
        //            result = MontoAnteriorExistenciaBodega + GetMontoMovimiento();
        //        else if (TipoMovimiento.TipoMovimiento.Value == (int)Model.TipoMovimiento.Salida)
        //            result = MontoAnteriorExistenciaBodega - GetMontoMovimiento();
        //        return result;
        //    }
        //}


        [StringLength(250)]
        public string CompanyName { get; set; }
    }

    public enum StatusMovement
    {
        Activo = 0,
        AplicadoInventario = 1,
        Anulado = 2,
        Reversado = 3
    }

    public enum TypeMovement
    {
        Input = 0,
        Output = 1,
    }

    public enum MovementCategory
    {
        InRequest = 0,
        OutRequest = 1,
        Adjustment = 2,
    }
}
