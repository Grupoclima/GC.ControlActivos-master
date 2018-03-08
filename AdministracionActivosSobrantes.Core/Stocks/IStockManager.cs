using System;
using System.Collections.Generic;

namespace AdministracionActivosSobrantes.Stocks
{
    public interface IStockManager
    {
        void Create(Stock @entity);
        IList<Stock> SearchStockInfo(Guid assetId, string company);
    }
}
