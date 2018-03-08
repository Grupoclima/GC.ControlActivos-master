using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Cellars;

namespace AdministracionActivosSobrantes.Stocks.Dto
{
    public class StocksMapReport
    {
        public Guid CellarId { get; set; } // Cellar or WareHouseId
        public virtual Cellar Cellar { get; set; }

        public Guid AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        public string NameAsset { get; set; }

        public string NameCellar { get; set; }

        public string CompanyName { get; set; }
        
        public double AssetQtyOutputs { get; set; }

        public double AssetQtyInputs { get; set; }

        public double AssetQtyOutputsBlocked { get; set; }

        public double GetStockItemsQty()
        {
            return AssetQtyInputs - AssetQtyOutputs;
        }

        public double GetStockItemsQtyBlocked()
        {
            return AssetQtyInputs - AssetQtyOutputs - AssetQtyOutputsBlocked;
        }

    }
}
