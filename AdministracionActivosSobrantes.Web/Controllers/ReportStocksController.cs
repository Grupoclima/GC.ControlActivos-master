using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.ReportStocks;
using AdministracionActivosSobrantes.ReportStocks.Dto;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Stocks.Dto;
using AdministracionActivosSobrantes.Users;
using AdministracionActivosSobrantes.Web.Account;
using AdministracionActivosSobrantes.Web.Infrastructure;
using GcErpConnection;

namespace AdministracionActivosSobrantes.Web.Controllers
{
    [AuthorizeUser(Roles = UserRoles.SuperAdministrador | UserRoles.SupervisorUca | UserRoles.Coordinador | UserRoles.AuxiliarUca | UserRoles.Solicitante)]
    public class ReportStocksController : Controller
    {
        private readonly IReportStocksAppService _reportStocksAppService;
        private readonly IGcErpConnectionService _erpConnectionService;
        private readonly ICurrentUser _currentUser;
        string _user = ConfigurationManager.AppSettings["DbUser"];
        string _password = ConfigurationManager.AppSettings["DbPassword"];
        private readonly IDateTime _date;

        public ReportStocksController(IReportStocksAppService reportStocksAppService, ICurrentUser currentUser, IDateTime date, IGcErpConnectionService erpConnectionService)
        {
            _reportStocksAppService = reportStocksAppService;
            _currentUser = currentUser;
            _erpConnectionService = erpConnectionService;
            _date = date;
        }

        [HttpPost]
        public ViewResultBase Search(ReportStockInputDto model)
        {
            try
            {
                model.CompanyName = _currentUser.CompanyName;
                //var entities = SearchStocks(model);
                model.Cellars = _reportStocksAppService.GetAllCellars(_currentUser.CompanyName);
                model.UserName = _currentUser.UserName;
                model.CurrentDateTime = _date.Now;
                //model.StocksList = entities;
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Existencias";
            }
            //if (Request.IsAjaxRequest())
            //{
            //    return PartialView("_searchStockReportSearchPartial", model);
            //}
            return View("Index", model);
        }

        public ActionResult Index()
        {
            ReportStockInputDto model = new ReportStockInputDto();
            try
            {
                model.Query = "";
                model.CompanyName = _currentUser.CompanyName;
                _erpConnectionService.ConnectErp(_user, _password, "");
                model.StocksList = _erpConnectionService.GetAllStocksByCompanyReport(model.CompanyName);
                _erpConnectionService.DisconnectErp();
                //model.Stocks = _reportStocksAppService.SearchStocks(model);
                
                model.Cellars = _reportStocksAppService.GetAllCellars(_currentUser.CompanyName);
                model.UserName = _currentUser.UserName;
                model.CurrentDateTime = _date.Now;
                model.Control = "ReportStocks";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Existencias";
            }
            return View(model);
        }

        public IList<StocksMapReport> SearchStocks(ReportStockInputDto searchInput)
        {
            string query = "";
            if (searchInput.Query != null)
                query = searchInput.Query.ToLower();

            _erpConnectionService.ConnectErp(_user, _password, "");
            var stocksList = _erpConnectionService.GetAllStocksByCompanyReport(searchInput.CompanyName);
            _erpConnectionService.DisconnectErp();

            //var stocksList = _stockRepository.GetAll();
            stocksList = (IList<StocksMapReport>) stocksList.Where(a => a.NameAsset.ToLower().Contains(query.ToLower()));

            if (searchInput.CellarId != null)
                stocksList = (IList<StocksMapReport>) stocksList.Where(a => a.CellarId == searchInput.CellarId.Value);

            //stocksList = stocksList.Where(a => a.CompanyName.ToLower().Equals(searchInput.CompanyName.ToLower()));

            return stocksList;
        }
    }
}