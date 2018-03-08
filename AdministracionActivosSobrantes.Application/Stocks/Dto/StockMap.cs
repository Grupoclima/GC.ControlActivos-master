using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Cellars;

namespace AdministracionActivosSobrantes.Stocks.Dto
{
    public class StockMap
    {
        public Guid CellarId { get; set; } // Cellar or WareHouseId
        public virtual Cellar Cellar { get; set; }

        public Guid AssetId { get; set; }
        public virtual Asset Asset { get; set; }
        //Monto en Salidas
        public double AmountOutput { get; set; } // Es el Valor en dinero que se lleva en de las existencias o solo en los articulos que están en el inventario

        //Monto en Entradas
        public double AmountInput { get; set; } // Es el Valor en dinero que se lleva en de las existencias o solo en los articulos que están en el inventario

        public double GetAmountInStock()
        {
            return AmountInput - AmountOutput;
        }

    }
}
