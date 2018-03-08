using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Projects.Dto;
using AdministracionActivosSobrantes.ReportStocks.Dto;
using AdministracionActivosSobrantes.Stocks;
using AutoMapper;
using MvcPaging;

namespace AdministracionActivosSobrantes.ReportStocks
{
    class ReportStocksAppService : ApplicationService, IReportStocksAppService
    {
        private readonly IRepository<Stock, Guid> _stockRepository;
        private readonly IRepository<Cellar, Guid> _cellarRepository;
        private readonly IDateTime _dateTime;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public ReportStocksAppService(IRepository<Stock, Guid> stockRepository, IRepository<Cellar, Guid> cellarRepository, IDateTime dateTime)
        {
            _stockRepository = stockRepository;
            _cellarRepository = cellarRepository;
            _dateTime = dateTime;
        }

        public IList<Cellar> GetAllCellars(string company)
        {
            var @entities = _cellarRepository.GetAllList(e => e.IsDeleted.Value == false && e.CompanyName.Equals(company));
            return @entities;
        }

        public IEnumerable<Stock> SearchStocks(ReportStockInputDto searchInput)
        {
            string query = "";
            if (searchInput.Query != null)
                query = searchInput.Query.ToLower();

            var stocksList = _stockRepository.GetAll();
            stocksList = stocksList.Where(a => a.IsDeleted == false && a.Asset.Name.ToLower().Contains(query.ToLower())
            || a.Asset.Code.ToLower().Equals(query.ToLower()) || a.Asset.Code.ToLower().Contains(query) || a.Asset.Code.ToLower().Equals(query));

            if (searchInput.CellarId != null)
                stocksList = stocksList.Where(a => a.CellarId == searchInput.CellarId.Value);

            stocksList = stocksList.Where(a => a.CompanyName.ToLower().Equals(searchInput.CompanyName.ToLower()));
            
            return stocksList.Include(a => a.Asset).Include(a => a.Cellar).OrderBy(a => a.Asset.Name).Take(200).ToList();
        }
    }
}
