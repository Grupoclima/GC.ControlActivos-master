using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdministracionActivosSobrantes.Stocks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.HistoryChanges;

namespace AdministracionActivosSobrantes.Cellars.Dto
{
    [AutoMapFrom(typeof(Detail))]
    public class OutRequestEntityDto
    {
        public Stock Stock { get; set; }

        public bool IsEdit { get; set; }

        public double UsedInRequest { get; set; }

        public double Availability()
        {
            if (IsEdit)
            {
                double stockUpdate = Stock.GetStockItemsQtyBlocked() + UsedInRequest;
                return stockUpdate - UsedInRequest;
            }
            else
            {
                return Stock.GetStockItemsQtyBlocked() - UsedInRequest;
            }
        }

    }
}
