using System;
using System.Web.Mvc;
using AdministracionActivosSobrantes.InRequest;
using AdministracionActivosSobrantes.OutRequest;
using AdministracionActivosSobrantes.Users;
using AdministracionActivosSobrantes.Web.Account;
using PdfGenerator;

namespace AdministracionActivosSobrantes.Web.Controllers
{
    [AuthorizeUser(Roles = UserRoles.SuperAdministrador | UserRoles.SupervisorUca | UserRoles.Coordinador | UserRoles.AuxiliarUca | UserRoles.Solicitante)]
    public class PdfGeneratorController : PdfViewController
    {
        private readonly IOutRequestAppService _outRequestService;
        private readonly IInRequestAppService _inRequestService;
        private readonly ICurrentUser _currentUser;

        public PdfGeneratorController(IOutRequestAppService outRequestService, IInRequestAppService inRequestService,ICurrentUser currentUser)
        {
            _outRequestService = outRequestService;
            _inRequestService = inRequestService;
            _currentUser = currentUser;
        }
        
        public ActionResult Index()
        {
            return ViewPdf("prueba", "~/Content/bootstrap.min.css", "TestPage", null, true); 
        }

        public ActionResult OutRequestDetails(Guid id,Guid cellar)
        {
           
            var temp = _outRequestService.Get(id);
            _outRequestService.GetEditDetails(id,cellar,_currentUser.CompanyName,0);
            temp.SignatureData = "data:" + temp.SignatureData;
            temp.ResponsiblesPersons = _outRequestService.GetAllResponsibles(_currentUser.CompanyName);
            temp.Users = _outRequestService.GetAllUsers(_currentUser.CompanyName);

            
           var updatedDetails = _outRequestService.ActualizaImpresion(id);
            //temp.Details = updatedDetails;

            return ViewPdf("", "~/Content/bootstrap.min.css", "OutRequestPdf", temp, false); 
        }


        public ActionResult InRequestDetails(Guid id)
        {
            var temp = _inRequestService.Get(id);
            temp.Users = _inRequestService.GetAllUsers(_currentUser.CompanyName);
            return ViewPdf("", "~/Content/bootstrap.min.css", "InRequestPdf", temp, false); 
        }

    }
}