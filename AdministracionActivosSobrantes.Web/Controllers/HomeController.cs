using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Users;
using AdministracionActivosSobrantes.Web.Account;
using GcErpConnection;

namespace AdministracionActivosSobrantes.Web.Controllers
{
    [AuthorizeUser(Roles = UserRoles.SuperAdministrador | UserRoles.SupervisorUca | UserRoles.Coordinador | UserRoles.AuxiliarUca | UserRoles.Solicitante)]
    public class HomeController : AdministracionActivosSobrantesControllerBase
    {
        private readonly IUserAppService _userService;
        private readonly IGcErpConnectionService _erpConnection;
        private readonly ICurrentUser _currentUser;
        string _user = ConfigurationManager.AppSettings["DbUser"];
        string _password = ConfigurationManager.AppSettings["DbPassword"];

        public HomeController(IUserAppService userService, ICurrentUser currentUser, IGcErpConnectionService erpConnection)
        {
            _userService = userService;
            _currentUser = currentUser;
            _erpConnection = erpConnection;
        }

        public ActionResult Index()
        {
            //MailSender.OutRequestAprovalEmail("0001", "Bodega1","AuxiliarUca","Solicitante","Supervisor","davinun@gmail.com", "davinun@gmail.com", "davinun@gmail.com","www.google.com",OutRequestStatus.Approved,TypeOutRequest.Asset,true);
            //_erpConnection.ConnectErp("consulta","CLIMA1");
            //_erpConnection.GetErpUsers();
            //var fullUrl = this.Url.Action("Index", "Home", new { id = 5 }, this.Request.Url.Scheme);
            //string action = this.Url.Action("Index", "Home", new { id = 5 });

            _erpConnection.ConnectErp(_user, _password, "");
            IList<Project> listProjects = _erpConnection.GetErpProyects(_currentUser.CompanyName,_currentUser.CurrentUserId);
            IList<Asset> listAsssets = _erpConnection.GetErpAssets(_currentUser.CompanyName, _currentUser.CurrentUserId);
            IList<ResponsiblePerson.ResponsiblePerson> listResponsiblePersons =
                _erpConnection.GetResposiblesPersons(_currentUser.CompanyName, _currentUser.CurrentUserId);

            _erpConnection.InsertAssetsDb(listAsssets,_currentUser.CurrentUserId,_currentUser.CompanyName);

            _erpConnection.InsertProjectsDb(listProjects);

            _erpConnection.InsertResponsiblesDb(listResponsiblePersons);

            _erpConnection.DisconnectErp();

            return View();
        }
	}
}