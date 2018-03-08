using System;
using System.Web.Mvc;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.ReportMovements;
using AdministracionActivosSobrantes.ReportMovements.Dto;
using AdministracionActivosSobrantes.Users;
using AdministracionActivosSobrantes.Web.Account;
using AdministracionActivosSobrantes.Web.Infrastructure;

namespace AdministracionActivosSobrantes.Web.Controllers
{
    [AuthorizeUser(Roles = UserRoles.SuperAdministrador | UserRoles.SupervisorUca | UserRoles.Coordinador | UserRoles.AuxiliarUca | UserRoles.Solicitante)]
    public class ReportMovementsController : Controller
    {
        private readonly IReportMovementsAppService _reportMovementsAppService;
        private readonly ICurrentUser _currentUser;
        private readonly IDateTime _date;

        public ReportMovementsController(IReportMovementsAppService reportMovementsAppService, ICurrentUser currentUser, IDateTime date)
        {
            _reportMovementsAppService = reportMovementsAppService;
            _currentUser = currentUser;
            _date = date;
        }

        [HttpPost]
        public ViewResultBase Search(ReportMovementsInputDto model)
        {
            try
            {
                model.CompanyName = _currentUser.CompanyName;
                var entities = _reportMovementsAppService.SearchReportMovements(model);
                model.Cellars = _reportMovementsAppService.GetAllCellars(_currentUser.CompanyName);
                //model.Projects = _reportMovementsAppService.GetAllProjects(_currentUser.CompanyName);
                model.Users = _reportMovementsAppService.GetAllUsers(_currentUser.CompanyName);
                model.ProjectName = model.ProjectName;
                model.CurrentUserName = _currentUser.UserName;
                model.CurrentDateTime = _date.Now;
                model.Movements = entities;
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Movimientos";
            }
            //if (Request.IsAjaxRequest())
            //{
            //    return PartialView("_searchStockReportSearchPartial", model);
            //}
            return View("Index", model);
        }

        public ActionResult Index()
        {
            ReportMovementsInputDto model = new ReportMovementsInputDto();
            try
            {
                model.Query = "";
                model.CompanyName = _currentUser.CompanyName;
                model.Movements = _reportMovementsAppService.SearchReportMovements(model);
                model.Cellars = _reportMovementsAppService.GetAllCellars(_currentUser.CompanyName);
                //model.Projects = _reportMovementsAppService.GetAllProjects(_currentUser.CompanyName);
                model.Users = _reportMovementsAppService.GetAllUsers(_currentUser.CompanyName);
                model.CurrentUserName = _currentUser.UserName;
                model.CurrentDateTime = _date.Now;
                model.Control = "ReportMovements";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Movimientos";
            }
            return View(model);
        }
    }
}