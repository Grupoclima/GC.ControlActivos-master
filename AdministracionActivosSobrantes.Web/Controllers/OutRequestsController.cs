using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AdministracionActivosSobrantes.Assets.Dto;
using AdministracionActivosSobrantes.Cellars.Dto;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.OutRequest;
using AdministracionActivosSobrantes.OutRequest.Dto;
using AdministracionActivosSobrantes.Projects.Dto;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Users;
using AdministracionActivosSobrantes.Web.Account;
using AdministracionActivosSobrantes.Web.Common;
using AdministracionActivosSobrantes.Web.Infrastructure;
using GcErpConnection;
using MvcPaging;
using Newtonsoft.Json;
using AdministracionActivosSobrantes.HistoryChanges;
using AdministracionActivosSobrantes.Assets;

namespace AdministracionActivosSobrantes.Web.Controllers
{
    [AuthorizeUser(Roles = UserRoles.SuperAdministrador | UserRoles.SupervisorUca | UserRoles.Coordinador | UserRoles.AuxiliarUca | UserRoles.Solicitante)]
    public class OutRequestsController : AdministracionActivosSobrantesControllerBase
    {
        private readonly IOutRequestAppService _outRequestService;
        private readonly IGcErpConnectionService _erpConnectionService;
        private readonly ICurrentUser _currentUser;
        private readonly IAssetsAppService _assetService;
        private readonly IUserAppService _userAppService;
        string _user = ConfigurationManager.AppSettings["DbUser"];
        string _password = ConfigurationManager.AppSettings["DbPassword"];
        // GET: OutRequests
        private readonly int _defaultPageSize = 10;
        

        public OutRequestsController(IOutRequestAppService outRequestService, ICurrentUser currentUser, IGcErpConnectionService erpConnectionService, IUserAppService userAppService, IAssetsAppService assetService)
        {
            _outRequestService = outRequestService;
            _currentUser = currentUser;
            _erpConnectionService = erpConnectionService;
            _userAppService = userAppService;
            _assetService = assetService;
        }

        public ActionResult Index(int? page)
        {
            SearchOutRequestInput model = new SearchOutRequestInput();
            try
            {
                model.Query = "";
                model.UserId = _currentUser.CurrentUserId;
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _outRequestService.SearchOutRequest(model);
                model.Users = _outRequestService.GetAllUsers(_currentUser.CompanyName);
                //model.Projects = _outRequestService.GetAllProjects(_currentUser.CompanyName);
                model.Cellars = _outRequestService.GetAllCellars(_currentUser.CompanyName);
                model.User = _outRequestService.GetUser(_currentUser.CurrentUserId);
                model.Control = "OutRequests";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Artículos";
            }
            return View(model);
        }

        [HttpPost]
        public ViewResultBase Search(SearchOutRequestInput model)
        {
            try
            {
                model.UserId = _currentUser.CurrentUserId;
                model.CompanyName = _currentUser.CompanyName;
                var entities = _outRequestService.SearchOutRequest(model);
                model.Entities = entities;
                model.Users = _outRequestService.GetAllUsers(_currentUser.CompanyName);
                //model.Projects = _outRequestService.GetAllProjects(_currentUser.CompanyName);
                model.Cellars = _outRequestService.GetAllCellars(_currentUser.CompanyName);
                model.User = _outRequestService.GetUser(_currentUser.CurrentUserId);
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Solicitudes";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_outRequestListPartial", model);
            }

            return View("Index", model);
        }

        [HttpPost]
        public ViewResultBase SearchDebounceProject(string query)
        {
            SearchProjectsInput model = new SearchProjectsInput();
            try
            {
                model.Query = query;
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _outRequestService.SearchProject(model);
                model.Control = "Project";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los proyectos";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_searchProjectPartial", model);
            }
            return PartialView("_searchProjectPartial", model);
        }

        [HttpPost]
        public ViewResultBase SearchDebounce(string query)
        {
            SearchOutRequestInput model = new SearchOutRequestInput();
            try
            {
                model.Query = query;
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _outRequestService.SearchOutRequest(model);
                model.Control = "OutRequest";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Solicitudes";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_outRequestListPartial", model);
            }
            return View("Index", model);
        }

        public ActionResult AjaxPage(string query, int? page, Guid? cellarId, Guid? projectId)
        {
            SearchOutRequestInput model = new SearchOutRequestInput();
            model.Page = page;
            model.Query = query;
            model.UserId = _currentUser.CurrentUserId;
            model.CellarId = cellarId;
            model.ProjectId = projectId;
            model.CompanyName = _currentUser.CompanyName;
            model.User = _outRequestService.GetUser(_currentUser.CurrentUserId);

            try
            {
                model.Entities = _outRequestService.SearchOutRequest(model);
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
                model.Query = "";
                model.CompanyName = _currentUser.CompanyName;
                model.Users = _outRequestService.GetAllUsers(_currentUser.CompanyName);
                //model.Projects = _outRequestService.GetAllProjects(_currentUser.CompanyName);
                model.Cellars = _outRequestService.GetAllCellars(_currentUser.CompanyName);
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las solicitudes";
            }
            return PartialView("_outRequestListPartial", model);
        }


        public ActionResult Details(Guid id)
        {
            var temp = _outRequestService.Get(id);
            temp.SignatureData = "data:" + temp.SignatureData;
            temp.Users = _outRequestService.GetAllUsers(_currentUser.CompanyName);
            temp.User = _outRequestService.GetUser(_currentUser.CurrentUserId);
            temp.ResponsiblesPersons = _outRequestService.GetAllResponsibles(_currentUser.CompanyName);
            temp.TypeOutRequestValue = (int)temp.TypeOutRequest;
            temp.StateRequest = (int)temp.Status;
            temp.CompanyName = _currentUser.CompanyName;
            //var temp = null;
            return View("_detail", temp);
        }

        //// GET: OutRequests/Create
        public ActionResult Create(int? param)
        {
            CreateOutRequestInput viewModel = new CreateOutRequestInput();
            viewModel.IsEdit = false;
            try
            {
                //viewModel.NoRequest = _solicitudMovimientoService.GetNextNoSolicitudMovimiento();
                viewModel.CompanyName = _currentUser.CompanyName;
                viewModel.RequestNumber = _outRequestService.GetNextRequestNumber(_currentUser.CompanyName);
                viewModel.Status = OutRequestStatus.Active;
                viewModel.StateRequest = (int)OutRequestStatus.Active;
                var user = _userAppService.Get(_currentUser.UserName, _currentUser.CompanyName);
                //viewModel.TypeOutRequestValue = null
                //viewModel.TypeOutRequest = TypeOutRequest.Asset;

                viewModel.DetailsRequest = new List<DetailAssetOutRequestDto>();
                if(user.Rol == UserRoles.Solicitante || user.Rol==UserRoles.Coordinador)
                {
                    viewModel.Cellars = _outRequestService.GetSomeAllCellars(_currentUser.CompanyName);
                }
                else
                {
                    viewModel.Cellars = _outRequestService.GetAllCellars(_currentUser.CompanyName);
                }
     
                viewModel.ResponsiblesPersons = _outRequestService.GetAllResponsibles(_currentUser.CompanyName);
                viewModel.CellarId = viewModel.Cellars.FirstOrDefault().Id;
                //viewModel.Projects = _outRequestService.GetAllProjects(_currentUser.CompanyName);
                viewModel.Contractors = _outRequestService.GetAllContractor(_currentUser.CompanyName);
                viewModel.ProjectId = null;
                viewModel.ProjectName = "";
                viewModel.ErrorCode = 0;
                viewModel.ErrorDescription = "";
                
            }
            catch (Exception e)
            {
                viewModel.ErrorCode = -1;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return View("_create", viewModel);
        }

        [HttpPost]
        public ActionResult Create()//IList<string> entity
        {
            CreateOutRequestInput viewModel = new CreateOutRequestInput();
           
            try
            {
                //viewModel = JsonConvert.DeserializeObject<CreateOutRequestInput>(entity[0]);
                viewModel = JsonConvert.DeserializeObject<CreateOutRequestInput>(System.Web.HttpContext.Current.Request["OutRequestDataSection"]);
                var imageToUpload1 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection1"];
                var imageToUpload2 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection2"];
                var imageToUpload3 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection3"];
                var imageToUpload4 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection4"];
                var signature = System.Web.HttpContext.Current.Request.Form["SignatureStr"];
                var user = _userAppService.Get(_currentUser.UserName, _currentUser.CompanyName);
                var departamento = user.Department;
               

               // if(viewModel.DepreciableCost == null)
               // { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener un Centro de Costo a Depreciar." }); }

                if (departamento == null)
                {
                    departamento = "ND";
                }

                if (viewModel.ResponsiblePersonId == Guid.Empty || viewModel.ResponsiblePersonId == null)
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener una Persona Responsable." }); }

                if (viewModel.ProjectId == Guid.Empty || viewModel.ProjectId == null)
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener un Proyecto Asignado." }); }

                if (viewModel.AssetsReturnDate == null)
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener una Fecha Requerida." }); }

                if (viewModel.DeliveredTo == null || viewModel.DeliveredTo == String.Empty)
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe un Encargado de Retirar." }); }

                if (viewModel.DetailsRequest == null || !viewModel.DetailsRequest.Any())
                { return Json(new { ErrorCode = -2, ErrorDescription = "La solicitud debe tener algún detalle de artículos." }); }

                if (viewModel.CellarId == Guid.Empty)
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe llevar una Ubicación." }); }

                if (ModelState.IsValid)
                {
                    if (imageToUpload1 != null)
                    {
                        viewModel.Image1 = imageToUpload1;
                        if (!CheckImageFormat.IsImage(imageToUpload1))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg, *.PDF" });
                        }
                    }
                    if (imageToUpload2 != null)
                    {
                        viewModel.Image2 = imageToUpload2;
                        if (!CheckImageFormat.IsImage(imageToUpload2))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 2 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg,*.PDF" });
                        }
                    }
                    if (imageToUpload3 != null)
                    {
                        viewModel.Image3 = imageToUpload3;
                        if (!CheckImageFormat.IsImage(imageToUpload3))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 3 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg,*.PDF" });
                        }
                    }
                    if (imageToUpload4 != null)
                    {
                        viewModel.Image4 = imageToUpload4;
                        if (!CheckImageFormat.IsImage(imageToUpload4))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 4 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg, *.PDF" });
                        }
                    }
                    viewModel.Department = departamento;
                    viewModel.CreatorGuidId = _currentUser.CurrentUserId;
                    viewModel.CompanyName = _currentUser.CompanyName;
                    _outRequestService.Create(viewModel);
                    ModelState.Clear();
                    return Json(new { ErrorCode = 1, ErrorDescription = "Solicitud guardada exitosamente." });
                }
                return Json(new { ErrorCode = -1, ErrorDescription = "Error al guardar la solicitud, verifique los datos." });

            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = e.Message });
            }
        }

        public ActionResult EditDetail(Guid id,int impreso)
        {
            var outRequest = _outRequestService.GetEdit(id);
            IList<DetailAssetOutRequestDto> viewModel = new List<DetailAssetOutRequestDto>();
            viewModel = _outRequestService.GetEditDetails(id, outRequest.CellarId, _currentUser.CompanyName,impreso);
            return PartialView("_editDetailTable", viewModel);
        }

        [HttpPost]
        public ActionResult EditDetailAndChangeStatus(IList<string> entity)
        {
            CreateOutRequestInput viewModel = new CreateOutRequestInput();
            viewModel.IsEdit = true;
            try
            {
                viewModel = JsonConvert.DeserializeObject<CreateOutRequestInput>(entity[0]);

                if (ModelState.IsValid)
                {
                    if (viewModel.Status == OutRequestStatus.WaitApproval)
                        viewModel.StateRequest = (int)OutRequestStatus.Approved;
                    else if (viewModel.Status == OutRequestStatus.Approved)
                    {
                        _erpConnectionService.ConnectErp(_user, _password, "");
                        viewModel.StockMaps = _erpConnectionService.GetAllStocksByCompany(_currentUser.CompanyName);
                        _erpConnectionService.DisconnectErp();
                        viewModel.UrlEmail = this.Url.Action("Details", "OutRequests", new { id = viewModel.Id }, this.Request.Url.Scheme);
                        viewModel.StateRequest = (int)OutRequestStatus.Backorder;
                    }


                    viewModel.CreatorUserId = _currentUser.CurrentUserId;
                    viewModel.CompanyName = _currentUser.CompanyName;
                    _outRequestService.UpdateEditDetailChangeStatus(viewModel);
                    return Json(new { ErrorCode = 1, ErrorDescription = "Solicitud guardada exitosamente." });
                }

                return Json(new { ErrorCode = -1, ErrorDescription = "Error al editar la solicitud, verifique los datos." });
            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = e.Message });
            }
        }

        public ActionResult Edit(Guid id,int impreso)
        {
            CreateOutRequestInput viewModel = new CreateOutRequestInput();

            try
            {
                viewModel = _outRequestService.GetEdit(id);
                DateTime date;
                viewModel.TypeOutRequestValue = (int)viewModel.TypeOutRequest;
                viewModel.CompanyName = _currentUser.CompanyName;
                if (viewModel.AssetsReturnDate != null)
                {
                    date = new DateTime(viewModel.AssetsReturnDate.Value.Year, viewModel.AssetsReturnDate.Value.Month, viewModel.AssetsReturnDate.Value.Day);
                    viewModel.AssetsReturnDateString = date.ToString("yyyy-MM-dd");
                }
                if (viewModel.ProjectId != null)
                    viewModel.ProjectName = _outRequestService.GetProject(viewModel.ProjectId.Value).Name;
                viewModel.StateRequest = (int)viewModel.Status;
                viewModel.DetailsRequest = _outRequestService.GetEditDetails(id, viewModel.CellarId, _currentUser.CompanyName,impreso);
                viewModel.IsEdit = true;
                viewModel.Cellars = _outRequestService.GetAllCellars(_currentUser.CompanyName);
                viewModel.ResponsiblesPersons = _outRequestService.GetAllResponsibles(_currentUser.CompanyName);
                //viewModel.Projects = _outRequestService.GetAllProjects(_currentUser.CompanyName);
                viewModel.Contractors = _outRequestService.GetAllContractor(_currentUser.CompanyName);

                viewModel.ErrorCode = 0;
                viewModel.ErrorDescription = "";

            }
            catch (Exception e)
            {
                viewModel.ErrorCode = -1;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return View("_edit", viewModel);
        }

        public ActionResult DeliverRequest(Guid id,int impreso)
        {
            CreateOutRequestInput viewModel = new CreateOutRequestInput();

            try
            {
                viewModel = _outRequestService.GetEdit(id);
                DateTime date;
                viewModel.TypeOutRequestValue = (int)viewModel.TypeOutRequest;
                viewModel.CompanyName = _currentUser.CompanyName;
                if (viewModel.AssetsReturnDate != null)
                {
                    date = new DateTime(viewModel.AssetsReturnDate.Value.Year, viewModel.AssetsReturnDate.Value.Month, viewModel.AssetsReturnDate.Value.Day);
                    viewModel.AssetsReturnDateString = date.ToString("yyyy-MM-dd");
                }
                if (viewModel.ProjectId != null)
                    viewModel.ProjectName = _outRequestService.GetProject(viewModel.ProjectId.Value).Name;
                viewModel.StateRequest = (int)viewModel.Status;
                viewModel.DetailsRequest = _outRequestService.GetEditDetails(id, viewModel.CellarId, _currentUser.CompanyName,impreso).Where(a => a.Status == Status.Approved).ToList();
                viewModel.IsEdit = true;
                viewModel.Cellars = _outRequestService.GetAllCellars(_currentUser.CompanyName);
                viewModel.ResponsiblesPersons = _outRequestService.GetAllResponsibles(_currentUser.CompanyName);
                //viewModel.Projects = _outRequestService.GetAllProjects(_currentUser.CompanyName);
                viewModel.Contractors = _outRequestService.GetAllContractor(_currentUser.CompanyName);

                viewModel.ErrorCode = 0;
                viewModel.ErrorDescription = "";

            }
            catch (Exception e)
            {
                viewModel.ErrorCode = -1;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return View("_deliverFormPartial", viewModel);
        }



        [HttpPost]
        public ActionResult Edit()//IList<string> entity
        {
            CreateOutRequestInput viewModel = new CreateOutRequestInput();
            viewModel.IsEdit = true;
            try
            {
              
                //viewModel = JsonConvert.DeserializeObject<CreateOutRequestInput>(entity[0]);
                viewModel = JsonConvert.DeserializeObject<CreateOutRequestInput>(System.Web.HttpContext.Current.Request["OutRequestDataSection"]);
                var imageToUpload1 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection1"];
                var imageToUpload2 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection2"];
                var imageToUpload3 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection3"];
                var imageToUpload4 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection4"];
                var signature = System.Web.HttpContext.Current.Request.Form["SignatureStr"];

               /* if(viewModel.DepreciableCost == null)
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener Centro de Costo al Cual Depreciar." }); }*/

                if (viewModel.ResponsiblePersonId == Guid.Empty)
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener una Persona Responsable." }); }

                if (viewModel.AssetsReturnDate == null)
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener una Fecha Requerida." }); }

                if (viewModel.DeliveredTo == null || viewModel.DeliveredTo == String.Empty)
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe un Encargado de Retirar." }); }

                if (viewModel.ProjectId == Guid.Empty || viewModel.ProjectId == null)
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener un Proyecto Asignado." }); }

                if (viewModel.DetailsRequest == null || !viewModel.DetailsRequest.Any())
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener algún detalle de artículos." }); }

                if (viewModel.CellarId == Guid.Empty)
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe llevar una Ubicación." }); }

                if (ModelState.IsValid)
                {
                    if (imageToUpload1 != null)
                    {
                        viewModel.Image1 = imageToUpload1;
                        if (!CheckImageFormat.IsImage(imageToUpload1))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg,*.PDF" });
                        }
                    }
                    if (imageToUpload2 != null)
                    {
                        viewModel.Image2 = imageToUpload2;
                        if (!CheckImageFormat.IsImage(imageToUpload2))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 2 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg,*.PDF" });
                        }
                    }
                    if (imageToUpload3 != null)
                    {
                        viewModel.Image3 = imageToUpload3;
                        if (!CheckImageFormat.IsImage(imageToUpload3))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 3 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg,*.PDF" });
                        }
                    }
                    if (imageToUpload4 != null)
                    {
                        viewModel.Image4 = imageToUpload4;
                        if (!CheckImageFormat.IsImage(imageToUpload4))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 4 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg,*.PDF" });
                        }
                    }
                    if (!string.IsNullOrEmpty(signature))
                    {
                        viewModel.SignatureData = signature;
                    }

                    viewModel.CompanyName = _currentUser.CompanyName;
                    _outRequestService.Update(viewModel);
                    // ApprovedStatus(viewModel.Id);  
                    return Json(new { ErrorCode = 1, ErrorDescription = "Solicitud guardada exitosamente." });
                }

                return Json(new { ErrorCode = -1, ErrorDescription = "Error al editar la solicitud, verifique los datos." });
            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = e.Message });
            }
        }

        [HttpPost]
        public ActionResult DeliverOutRequestAction()//IList<string> entity
        {
            CreateOutRequestInput viewModel = new CreateOutRequestInput();
            viewModel.IsEdit = true;
            try
            {
                //viewModel = JsonConvert.DeserializeObject<CreateOutRequestInput>(entity[0]);
                viewModel = JsonConvert.DeserializeObject<CreateOutRequestInput>(System.Web.HttpContext.Current.Request["OutRequestDataSection"]);
                var imageToUpload1 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection1"];
                var imageToUpload2 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection2"];
                var imageToUpload3 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection3"];
                var imageToUpload4 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection4"];
                var signature = System.Web.HttpContext.Current.Request.Form["SignatureStr"];
                var observacion = System.Web.HttpContext.Current.Request.Form["Comment"];
                var company = viewModel.CompanyName;

                if (viewModel.Status != OutRequestStatus.ProcessedInWareHouse)
                {
                    if (viewModel.DetailsRequest == null || !viewModel.DetailsRequest.Any())
                    { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener algún detalle de artículos." }); }
                }
                if (ModelState.IsValid)
                {
                    if (imageToUpload1 != null)
                    {
                        viewModel.Image1 = imageToUpload1;
                        if (!CheckImageFormat.IsImage(imageToUpload1))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg,*.PDF" });
                        }
                    }
                    if (imageToUpload2 != null)
                    {
                        viewModel.Image2 = imageToUpload2;
                        if (!CheckImageFormat.IsImage(imageToUpload2))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 2 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg,*.PDF" });
                        }
                    }
                    if (imageToUpload3 != null)
                    {
                        viewModel.Image3= imageToUpload3;
                        if (!CheckImageFormat.IsImage(imageToUpload3))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 3 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg,*.PDF" });
                        }
                    }
                    if (imageToUpload4 != null)
                    {
                        viewModel.Image4 = imageToUpload4;
                        if (!CheckImageFormat.IsImage(imageToUpload4))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 4 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg,*.PDF" });
                        }
                    }
                    if (!string.IsNullOrEmpty(signature))
                    {
                        viewModel.SignatureData = signature;
                    }
                    if (viewModel.Image1 == null && viewModel.Image2 == null && viewModel.Image3 == null && viewModel.Image4 == null)
                    {
                        return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "La solicitud para ser entregada debe tener almenos una imagen adjunta." });
                    }
                    viewModel.Comment = observacion;
                    viewModel.CompanyName = _currentUser.CompanyName;
                    viewModel.CreatorUserId = _currentUser.CurrentUserId;
                    _erpConnectionService.ConnectErp(_user, _password, "");
                    var list = _erpConnectionService.GetAllStocksByCompany(_currentUser.CompanyName);
                    _erpConnectionService.DisconnectErp();
                    string actionUrl = this.Url.Action("Details", "OutRequests", new { id = viewModel.Id }, this.Request.Url.Scheme);
                    _outRequestService.DeliverRequest(viewModel, list, actionUrl);
                    return Json(new { ErrorCode = 1, ErrorDescription = "Solicitud guardada exitosamente." });
                }

                return Json(new { ErrorCode = -1, ErrorDescription = "Error al editar la solicitud, verifique los datos." });
            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = e.Message });
            }
        }
        //----------------------------
        [HttpPost]
        public ActionResult EditPictures()//IList<string> entity
        {
            CreateOutRequestInput viewModel = new CreateOutRequestInput();
           viewModel.IsEdit = true;
            try
            {
                //viewModel = JsonConvert.DeserializeObject<CreateOutRequestInput>(entity[0]);
                viewModel = JsonConvert.DeserializeObject<CreateOutRequestInput>(System.Web.HttpContext.Current.Request["OutRequestDataSection"]);
                var imageToUpload1 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection1"];
                var imageToUpload2 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection2"];
                var imageToUpload3 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection3"];
                var imageToUpload4 = System.Web.HttpContext.Current.Request.Files["OutRequestImagesSection4"];
                var signature = System.Web.HttpContext.Current.Request.Form["SignatureStr"];
                var observacion = System.Web.HttpContext.Current.Request.Form["Comment"];
                var company = viewModel.CompanyName;

                if (ModelState.IsValid)
                {
                    if (imageToUpload1 != null)
                    {
                        viewModel.Image1 = imageToUpload1;
                        if (!CheckImageFormat.IsImage(imageToUpload1))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg,*.PDF" });
                        }
                    }
                    if (imageToUpload2 != null)
                    {
                        viewModel.Image2 = imageToUpload2;
                        if (!CheckImageFormat.IsImage(imageToUpload2))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 2 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg,*.PDF" });
                        }
                    }
                    if (imageToUpload3 != null)
                    {
                        viewModel.Image3 = imageToUpload3;
                        if (!CheckImageFormat.IsImage(imageToUpload3))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 3 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg,*.PDF" });
                        }
                    }
                    if (imageToUpload4 != null)
                    {
                        viewModel.Image4 = imageToUpload4;
                        if (!CheckImageFormat.IsImage(imageToUpload4))
                        {
                            //viewModel.ErrorCode = ErrorCodeHelper.Error;
                            //viewModel.ErrorDescription = "Error al cargar la imagen 1 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg";
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "Error al cargar la imagen 4 seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg,*.PDF" });
                        }
                    }
                    if (!string.IsNullOrEmpty(signature))
                    {
                        viewModel.SignatureData = signature;
                    }
                    if (viewModel.Image1 == null && viewModel.Image2 == null && viewModel.Image3 == null && viewModel.Image4 == null)
                    {
                        return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "La solicitud para ser modificada debe tener almenos una imagen adjunta." });
                    }
                    viewModel.Comment = observacion;
                    viewModel.CompanyName = _currentUser.CompanyName;
                    viewModel.CreatorUserId = _currentUser.CurrentUserId;
                    _erpConnectionService.ConnectErp(_user, _password, "");
                    var list = _erpConnectionService.GetAllStocksByCompany(_currentUser.CompanyName);
                    _erpConnectionService.DisconnectErp();
                    string actionUrl = this.Url.Action("Details", "OutRequests", new { id = viewModel.Id }, this.Request.Url.Scheme);
                    _outRequestService.DeliverRequest(viewModel, list, actionUrl);
                    return Json(new { ErrorCode = 1, ErrorDescription = "Solicitud guardada exitosamente." });
                }

                return Json(new { ErrorCode = -1, ErrorDescription = "Error al editar la solicitud, verifique los datos." });
            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = e.Message });
            }
        }
        //----------------------------

        [HttpPost]
        public ActionResult ChangeCloseStatusRequest(IList<string> entity)
        {
            CloseOutRequestInput viewModel = new CloseOutRequestInput();
            try
            {
                viewModel = JsonConvert.DeserializeObject<CloseOutRequestInput>(entity[0]);


                if (viewModel.DetailsRequest == null || !viewModel.DetailsRequest.Any())
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener algún detalle de artículos." }); }

                if (ModelState.IsValid)
                {
                    _erpConnectionService.ConnectErp(_user, _password, "");
                    var list = _erpConnectionService.GetAllStocksByCompany(_currentUser.CompanyName);
                    _erpConnectionService.DisconnectErp();
                    viewModel.UrlAction = this.Url.Action("Details", "InRequests", new { id = viewModel.Id }, this.Request.Url.Scheme);
                    _outRequestService.CloseRequest(viewModel, list);
                    return Json(new { ErrorCode = 1, ErrorDescription = "Solicitud guardada exitosamente." });
                }

                return Json(new { ErrorCode = -1, ErrorDescription = "Error al editar la solicitud, verifique los datos." });
            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = e.Message });
            }
        }

        [HttpPost]
        public ActionResult ShowArticuloList(IList<string> jsonDetallesList, string q, Guid cellarId, Guid outRequestId, int? typeOutRequestValue, int? page)
        {
           
            ViewBag.Query = q;
            ViewBag.JsonDetallesList = jsonDetallesList;
            ViewBag.CellarId = cellarId;
            ViewBag.OutRequestId = outRequestId;
            ViewBag.TypeOutRequestValue = typeOutRequestValue;
            
            

            IList<DetailAssetOutRequestDto> detalleList = new JavaScriptSerializer().Deserialize<IList<DetailAssetOutRequestDto>>(jsonDetallesList[0]);
            int type = Convert.ToInt32(typeOutRequestValue);
            TypeOutRequest temp = (TypeOutRequest)type;
            IPagedList<Stock> listExistencias = _outRequestService.GetAllStocks(q, cellarId, null, temp, _defaultPageSize, page, _currentUser.CompanyName);
            //var listExistencias = ;
            bool isEdit = false;
            if (outRequestId != Guid.Empty)
                isEdit = true;
            var tempPrueba = CreateListForSalidaControl(detalleList, listExistencias, page, _defaultPageSize, isEdit);

            return PartialView("_searchAssetPartial", tempPrueba);
        }

        private IPagedList<OutRequestEntityDto> CreateListForSalidaControl(IEnumerable<DetailAssetOutRequestDto> detalleList, IPagedList<Stock> listExistencias, int? page, int defaultPageSize, bool isEdit)
        {
            IList<OutRequestEntityDto> newList = new List<OutRequestEntityDto>();
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            foreach (var existencia in listExistencias)
            {
                var newViewEntity = new OutRequestEntityDto();
                newViewEntity.Stock = existencia;


                if (detalleList != null)
                {
                    if (detalleList.Any(d => d.AssetId == existencia.AssetId))
                        newViewEntity.UsedInRequest = detalleList.Where(a => a.Delete == 0 && a.AssetId == existencia.AssetId).Sum(a => a.StockAsset);
                    else
                        newViewEntity.UsedInRequest = 0;
                }
                else
                    newViewEntity.UsedInRequest = 0;

                newViewEntity.IsEdit = isEdit;

                newList.Add(newViewEntity);

            }

            return newList.ToPagedList(currentPageIndex, defaultPageSize, listExistencias.TotalItemCount);
        }

        [HttpPost]
        public ActionResult SearchArticulos(IList<string> jsonDetallesList, string q, Guid cellarId, Guid outRequestId, int? typeOutRequestValue, int? page)
        {
            
            ViewBag.Query = q;
            ViewBag.CellarId = cellarId;
            ViewBag.JsonDetallesList = jsonDetallesList;
            ViewBag.OutRequestId = outRequestId;

            ViewBag.TypeOutRequestValue = typeOutRequestValue;

            IEnumerable<DetailAssetOutRequestDto> detalleList = new JavaScriptSerializer().Deserialize<IList<DetailAssetOutRequestDto>>(jsonDetallesList[0]);
            int type = Convert.ToInt32(typeOutRequestValue);
            TypeOutRequest temp = (TypeOutRequest)type;
            IPagedList<Stock> listExistencias = _outRequestService.GetAllStocks(q, cellarId, null, temp, _defaultPageSize, page, _currentUser.CompanyName);
            bool isEdit = false;
            if (outRequestId != Guid.Empty)
                isEdit = true;

            var tempPrueba = CreateListForSalidaControl(detalleList, listExistencias, page, _defaultPageSize, isEdit);

            return PartialView("_searchAssetPartial", tempPrueba);
        }

        [HttpPost]
        public ActionResult AddAndRenderListDetallesOrden(IList<string> jsonDetallesList, Guid? assetId, string nameAsset, double? stockAsset, string price, double availability,string number)
        {
           
           try
            {
                
             IEnumerable<DetailAssetOutRequestDto> detalleList = new JavaScriptSerializer().Deserialize<IList<DetailAssetOutRequestDto>>(jsonDetallesList[0]);
                
                if (assetId == null)
                    return Json(new { Error = -1, Message = "Por Favor seleccione el artículo correctamente." });

                if (stockAsset == null || stockAsset < 0)
                    return Json(new { Error = -1, Message = "Por Favor indique la cantidad del artículo correctamente." });

                if (stockAsset > availability)
                    return Json(new { Error = -1, Message = "No hay suficientes artículos en Ubicación seleccionada. Por Favor revise las existencias." });

                if (price == null || Double.Parse(price) < 0)
                    return Json(new { Error = -1, Message = "Por Favor indique el precio de costo del artículo correctamente." });


                if (detalleList.Any(item => item.AssetId == assetId && item.Delete == 0))
                {
                    return Json(new { Error = -1, Message = "Ya existe una salida para el artículo" + nameAsset + " por favor edite correctamente el detalle." });
                }

                DetailAssetOutRequestDto detalle = new DetailAssetOutRequestDto();
                detalle.AssetId = (Guid)assetId;
                detalle.NameAsset = nameAsset;
                if (stockAsset > 0.0)
                {
                    detalle.StockAsset = (double)stockAsset;
                }
                else
                {
                    return Json(new { Error = -1, Message = "Por Favor indique la cantidad del artículo correctamente." });
                }
                detalle.Price = price;
                detalle.AssetAvailability = availability;
                detalle.Saved = 0;
                detalle.Update = 1;
                detalle.Delete = 0;
                detalle.ErrorDescription = "";
                detalle.ErrorCode = 0;

                _assetService.UpdateInOutRequest(detalle.AssetId, number, "out");

                IList<DetailAssetOutRequestDto> newList = new List<DetailAssetOutRequestDto>();
                int index = 0;
                foreach (var item in detalleList)
                {
                    item.Index = index;
                    newList.Add(item);
                    index++;
                }
                detalle.Index = index;
                newList.Add(detalle);
                return PartialView("_tableDetails", newList);
            }
            catch (Exception)
            {
                return Json(new { Error = -1, Message = "Error al Agregar/Modificar la Solicitud" });
            }
        }


        [HttpPost]
        public ActionResult AddListDetallesOrden(IList<string> jsonDetallesList, string code, Guid cellarId)
        {
            try
            {
                IEnumerable<DetailAssetOutRequestDto> detalleList = new JavaScriptSerializer().Deserialize<IList<DetailAssetOutRequestDto>>(jsonDetallesList[0]);
                IList<DetailAssetOutRequestDto> newList = new List<DetailAssetOutRequestDto>();
                var asset = _outRequestService.GetAssetBarCode(code, _currentUser.CompanyName);

                if (asset == null)
                    return Json(new { Error = -1, Message = "No se ha encontrado un artículo con este código de barra" });

                var stock = _outRequestService.GetStockAsset(asset.Id, _currentUser.CompanyName, cellarId);


                bool cambio = false;
                if (detalleList.Any(item => item.AssetId == asset.Id && item.Delete == 0))
                {

                    cambio = true;
                    int index = 0;
                    foreach (var item in detalleList)
                    {
                        if (item.AssetId == asset.Id)
                        {
                            item.StockAsset = item.StockAsset + 1;
                            if (item.StockAsset > stock.GetStockItemsQtyBlocked())
                                return Json(new { Error = -1, Message = "No hay suficientes artículos en Ubicación seleccionada. Por Favor revise las existencias." });
                        }

                        item.Index = index;
                        newList.Add(item);
                        
                        index++;
                    }
                }


                if (!cambio)
                {

                    DetailAssetOutRequestDto detalle = new DetailAssetOutRequestDto();
                    detalle.AssetId = asset.Id;
                    detalle.NameAsset = asset.Name;
                    detalle.StockAsset = 1;
                    if (detalle.StockAsset > stock.GetStockItemsQtyBlocked())
                        return Json(new { Error = -1, Message = "No hay suficientes artículos en Ubicación seleccionada. Por Favor revise las existencias." });

                    double availa = stock.GetStockItemsQtyBlocked() - detalle.StockAsset;
                    detalle.Price = asset.Price.ToString();
                    detalle.AssetAvailability = availa;
                    detalle.Saved = 0;
                    detalle.Update = 1;
                    detalle.Delete = 0;
                    detalle.ErrorDescription = "";
                    detalle.ErrorCode = 0;

                    int index = 0;
                    foreach (var item in detalleList)
                    {
                       // var outrequestnum = _outRequestService.Get(item.OutRequestId);
                        item.Index = index;
                        newList.Add(item);
                        index++;
                    }
                    detalle.Index = index;
                    newList.Add(detalle);
                }


                return PartialView("_tableDetails", newList);
            }
            catch (Exception ex)
            {
                return Json(new { Error = -1, Message = "Error al Agregar/Modificar la Solicitud" });
            }
        }

        [HttpPost]
        public ActionResult RenderListDetallesOrdenAssetId(IList<string> jsonDetallesList, Guid? assetId)
        {
            try
            {
                IEnumerable<DetailAssetOutRequestDto> detalleList = new JavaScriptSerializer().Deserialize<IList<DetailAssetOutRequestDto>>(jsonDetallesList[0]);
                IList<DetailAssetOutRequestDto> newList = new List<DetailAssetOutRequestDto>();
                int index = 0;
                foreach (var item in detalleList)
                {
                    if (item.AssetId == assetId)
                    {
                        item.Status = Status.Delivered;
                    }
                    item.Index = index;
                    index++;
                    newList.Add(item);
                }
                return PartialView("_tableDeliverDetails", newList);
            }
            catch (Exception)
            {
                return Json(new { Error = -1, Message = "Error al cambiar de estado el detalle" });
            }
        }

        [HttpPost]
        public ActionResult RenderListDetallesOrdenCode(IList<string> jsonDetallesList, string code)
        {
            try
            {
                //String code1;
                //code1=code.ToUpper();
               // code = code1;
                IEnumerable<DetailAssetOutRequestDto> detalleList = new JavaScriptSerializer().Deserialize<IList<DetailAssetOutRequestDto>>(jsonDetallesList[0]);
                IList<DetailAssetOutRequestDto> newList = new List<DetailAssetOutRequestDto>();
                int index = 0;
                var asset = _outRequestService.GetAssetBarCode(code, _currentUser.CompanyName);

                if (asset == null)
                    return Json(new { Error = -1, Message = "No se ha encontrado un artículo con este código de barra" });

                foreach (var item in detalleList)
                {
                    if (item.AssetId == asset.Id)
                    {
                        item.Status = Status.Delivered;
                    }
                    item.Index = index;
                    index++;
                    newList.Add(item);
                }
                return PartialView("_tableDeliverDetails", newList);
            }
            catch (Exception)
            {
                return Json(new { Error = -1, Message = "Error al cambiar de estado el detalle" });
            }
        }

        [HttpPost]
        public ActionResult RenderListDetallesOrden(IList<string> jsonDetallesList)
        {
            try
            {
                IEnumerable<DetailAssetOutRequestDto> detalleList = new JavaScriptSerializer().Deserialize<IList<DetailAssetOutRequestDto>>(jsonDetallesList[0]);
                IList<DetailAssetOutRequestDto> newList = new List<DetailAssetOutRequestDto>();
                int index = 0;
                foreach (var item in detalleList)
                {
                    if (item.Delete == 1)
                    {
                        item.Index = -1;
                        //Hacer movimiento de entrada nuevamente// porque se borro la salida
                    }
                    else
                    {
                        if (item.Update == 1 && item.StockAsset > item.AssetAvailability)
                            return Json(new { Error = -1, Message = "No hay suficientes artículos en Ubicación seleccionada. Por Favor revise las existencias." });

                        item.Index = index;
                        index++;
                    }
                    newList.Add(item);
                }
                return PartialView("_tableDetails", newList);
            }
            catch (Exception)
            {
                return Json(new { Error = -1, Message = "Error al Agregar/Modificar la Solicitud" });
            }
        }

        [HttpPost]
        public ActionResult RenderListDetallesOrdenDetail(IList<string> jsonDetallesList)
        {
            try
            {
                IEnumerable<DetailAssetOutRequestDto> detalleList = new JavaScriptSerializer().Deserialize<IList<DetailAssetOutRequestDto>>(jsonDetallesList[0]);
                IList<DetailAssetOutRequestDto> newList = new List<DetailAssetOutRequestDto>();
                int index = 0;
                foreach (var item in detalleList)
                {
                    if (item.Delete == 1)
                    {
                        //item.Index = -1;
                        item.Index = index;
                        index++;
                        item.HistroryStatus = HistoryStatus.Removed;
                        //Hacer movimiento de entrada nuevamente// porque se borro la salida
                    }
                    else
                    {
                        if (item.Update == 2 && item.StockAsset > item.AssetAvailability)
                            return Json(new { Error = -1, Message = "No hay suficientes artículos en Ubicación seleccionada. Por Favor revise las existencias." });
                        if (item.Update == 2)
                            item.HistroryStatus = HistoryStatus.Modified;
                        else
                        {
                            item.HistroryStatus = HistoryStatus.WithoutChanges;
                        }
                        item.Index = index;
                        index++;
                    }
                    newList.Add(item);
                }
                return PartialView("_editDetailTable", newList);
            }
            catch (Exception)
            {
                return Json(new { Error = -1, Message = "Error al Agregar/Modificar la Solicitud" });
            }
        }

        [HttpPost]
        public ActionResult RenderListDetailsCloseRequest(IList<string> jsonDetallesList)
        {
            try
            {
                IEnumerable<DetailAssetCloseRequest> detalleList = new JavaScriptSerializer().Deserialize<IList<DetailAssetCloseRequest>>(jsonDetallesList[0]);
                IList<DetailAssetCloseRequest> newList = new List<DetailAssetCloseRequest>();
                int index = 0;
                foreach (var item in detalleList)
                {
                    item.Index = index;
                    item.Asset = _outRequestService.GetAsset(item.AssetId);
                    if (item.AssetReturnPartialQty != null)
                    {
                        item.LeftOver = item.StockAsset - item.AssetReturnPartialQty.Value - item.ReturnAssetQty;
                    }
                    else
                    {
                        item.LeftOver = item.StockAsset - item.ReturnAssetQty;
                    }

                    newList.Add(item);
                    index++;
                }
                return PartialView("_closeRequestsTable", newList);
            }
            catch (Exception)
            {
                return Json(new { Error = -1, Message = "Error al Agregar/Modificar la Solicitud" });
            }
        }

        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {
                Guid userId = _currentUser.CurrentUserId;
                _outRequestService.Delete(id, userId, _currentUser.CompanyName);
                return Json(new { ErrorCode = 1, ErrorDescription = "Solicitud Eliminada Correctamente" });
            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = "Error al eliminar la solicitud" });
            }
        }

        [HttpPost]
        public ActionResult WaitApprovalStatus(Guid outRequestId)
        {
            try
            {
                string actionUrl = this.Url.Action("Details", "OutRequests", new { id = outRequestId }, this.Request.Url.Scheme);
                _outRequestService.WaitApprovalStatus(outRequestId, _currentUser.CurrentUserId, actionUrl, _currentUser.CompanyName);
                return Json(new { ErrorCode = 0, ErrorDescription = "Por favor espere a que la solicitud sea aprobada." });
            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = "Error al cambiar de estado" });
            }
        }

        [HttpPost]
        public ActionResult ApprovedStatus(Guid outRequestId)
        {
        
            try
            {
                string actionUrl = this.Url.Action("Details", "OutRequests", new { id = outRequestId }, this.Request.Url.Scheme);
                _outRequestService.ApprovedStatus(outRequestId, _currentUser.CurrentUserId, actionUrl, _currentUser.CompanyName);
                return Json(new { ErrorCode = 0, ErrorDescription = "Solicitud Aprobada." });
            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = "Error al cambiar de estado" });
            }
        }

        [HttpPost]
        public ActionResult DeliverStatus(Guid outRequestId)
        {

            try
            {
                _erpConnectionService.ConnectErp(_user, _password, "");
                var list = _erpConnectionService.GetAllStocksByCompany(_currentUser.CompanyName);
                _erpConnectionService.DisconnectErp();
                string actionUrl = this.Url.Action("Details", "OutRequests", new { id = outRequestId }, this.Request.Url.Scheme);
                _outRequestService.ProcessedInWareHouseStatus(outRequestId, _currentUser.CurrentUserId, actionUrl, _currentUser.CompanyName, list);
                return Json(new { ErrorCode = 0, ErrorDescription = "Solicitud entregada correctamente." });
            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = "Error al cambiar de estado" });
            }
        }

        [HttpPost]
        public ActionResult DisprovedStatus(Guid outRequestId)
        {
            try
            {
                string actionUrl = this.Url.Action("Details", "OutRequests", new { id = outRequestId }, this.Request.Url.Scheme);
                _outRequestService.DisprovedStatus(outRequestId, _currentUser.CurrentUserId, actionUrl, _currentUser.CompanyName);
                return Json(new { ErrorCode = 0, ErrorDescription = "Solicitud rechazada correctamente." });
            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = "Error al cambiar de estado" });
            }
        }


        [HttpPost]
        public ActionResult CloseRequest(Guid outRequestId)
        {
            CloseOutRequestInput viewModel = new CloseOutRequestInput();
            try
            {

                var temp = _outRequestService.Get(outRequestId);
                viewModel.DetailsRequest = _outRequestService.GetCloseRequestDetails(outRequestId, temp.CellarId.Value, _currentUser.CompanyName);
                viewModel.Id = outRequestId;
                viewModel.CellarName = temp.Cellar.Name;
                viewModel.RequestNumber = temp.RequestNumber;
                viewModel.RequestDocumentNumber = temp.RequestDocumentNumber;
                viewModel.AssetReturnDate = temp.AssetsReturnDate.Value.ToString("yyyy-MM-dd");
                viewModel.WarehouseManName = temp.WareHouseMan.UserName;

                viewModel.ErrorCode = 0;
                viewModel.ErrorDescription = "";

            }
            catch (Exception e)
            {
                viewModel.ErrorCode = -1;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return PartialView("_closeRequests", viewModel);
        }

        [HttpPost]
        public ActionResult CloseRequestApply(IList<string> entity)
        {
            CloseOutRequestInput viewModel = new CloseOutRequestInput();
            viewModel = JsonConvert.DeserializeObject<CloseOutRequestInput>(entity[0]);
            try
            {
                
                if (viewModel.DetailsRequest == null || !viewModel.DetailsRequest.Any())
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener algún detalle de artículos." }); }

                if (ModelState.IsValid)
                {
                    _erpConnectionService.ConnectErp(_user, _password, "");
                    var list = _erpConnectionService.GetAllStocksByCompany(_currentUser.CompanyName);
                    _erpConnectionService.DisconnectErp();
                    viewModel.CreatorUserId = _currentUser.CurrentUserId;
                    viewModel.CompanyName = _currentUser.CompanyName;
                    viewModel.UrlAction = this.Url.Action("Details", "OutRequests", new { id = viewModel.Id }, this.Request.Url.Scheme);
                    _outRequestService.CloseRequest(viewModel, list);
                    ModelState.Clear();
                    return Json(new { ErrorCode = 1, ErrorDescription = "Solicitud cerrada exitosamente." });
                }
                return Json(new { ErrorCode = -1, ErrorDescription = "Error al guardar la solicitud, verifique los datos." });

            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = e.Message });
            }
        }

        [HttpPost]
        public ActionResult ApplyMovementCloseRequest(Guid outRequestId, Guid assetId, double qty)
        {
            try
            {
                if (qty <= 0)
                { return Json(new { ErrorCode = -1, ErrorDescription = "No puede realizar una devolución menor o igual a 0." }); }
                _erpConnectionService.ConnectErp(_user, _password, "");
                var list = _erpConnectionService.GetAllStocksByCompany(_currentUser.CompanyName);
                _erpConnectionService.DisconnectErp();
                _outRequestService.ClosePartialRequest(outRequestId, assetId, qty, _currentUser.CurrentUserId, _currentUser.CompanyName, list);
                //----------------------------------------------------------------------------
                CloseOutRequestInput viewModel = new CloseOutRequestInput();
                try
                {
                    var temp = _outRequestService.Get(outRequestId);
                    viewModel.CompanyName = _currentUser.CompanyName;
                    viewModel.CreatorUserId = _currentUser.CurrentUserId;
                    viewModel.DetailsRequest = _outRequestService.GetCloseRequestDetails(outRequestId, temp.CellarId.Value, _currentUser.CompanyName);
                    viewModel.ErrorCode = 1;
                    viewModel.ErrorDescription = "Movimiento Creado Exitosamente";
                }
                catch (Exception e)
                {
                    viewModel.ErrorCode = -1;
                    viewModel.ErrorDescription = "Error al obtener datos.";
                }
                return PartialView("_closeRequestsTable", viewModel.DetailsRequest);
                //----------------------------------------------------------------------------
            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = e.Message });
            }
        }

        [HttpPost]
        public ActionResult ShowProjectList(int? page, string q)
        {
            ViewBag.Query = q;
            SearchProjectsInput searchInput = new SearchProjectsInput();
            searchInput.Query = q;
            searchInput.Page = page;
            searchInput.CompanyName = _currentUser.CompanyName;
            searchInput.Entities = _outRequestService.SearchProject(searchInput);
            return PartialView("_searchProjectPartial", searchInput);
        }

        [HttpPost]
        public ActionResult SearchArticulos(string q, int? page)
        {
            ViewBag.Query = q;
            SearchProjectsInput searchInput = new SearchProjectsInput();
            searchInput.Query = q;
            searchInput.Page = page;
            searchInput.CompanyName = _currentUser.CompanyName;
            searchInput.Entities = _outRequestService.SearchProject(searchInput);
            return PartialView("_searchProjectPartial", searchInput);
        }       
        public ActionResult AjaxPageProject(string query, int? page)
        {
            SearchProjectsInput searchInput = new SearchProjectsInput();
            searchInput.Query = query;
            searchInput.Page = page;
            searchInput.CompanyName = _currentUser.CompanyName;
            searchInput.Entities = _outRequestService.SearchProject(searchInput);
            return PartialView("_searchProjectPartial", searchInput);
        }
    }
}
