using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AdministracionActivosSobrantes.Adjustments;
using AdministracionActivosSobrantes.Adjustments.Dto;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Stocks.Dto;
using AdministracionActivosSobrantes.Users;
using AdministracionActivosSobrantes.Web.Account;
using AdministracionActivosSobrantes.Web.Infrastructure;
using GcErpConnection;
using MvcPaging;
using Newtonsoft.Json;

namespace AdministracionActivosSobrantes.Web.Controllers
{
    [AuthorizeUser(Roles = UserRoles.SuperAdministrador | UserRoles.SupervisorUca | UserRoles.Coordinador | UserRoles.AuxiliarUca | UserRoles.Solicitante)]
    public class AdjustmentsController : AdministracionActivosSobrantesControllerBase
    {
        private readonly IAdjustmentAppService _adjustmentsService;
        private readonly ICurrentUser _currentUser;
        private readonly IGcErpConnectionService _erpConnectionService;
        string _user = ConfigurationManager.AppSettings["DbUser"];
        string _password = ConfigurationManager.AppSettings["DbPassword"];
        // GET: Adjustments
        private readonly int _defaultPageSize = 10;
        public AdjustmentsController(IAdjustmentAppService outRequestService, ICurrentUser currentUser, IGcErpConnectionService erpConnectionService)
        {
            _adjustmentsService = outRequestService;
            _currentUser = currentUser;
            _erpConnectionService = erpConnectionService;
        }

        public ActionResult Index(int? page)
        {
            SearchAdjustmentInput model = new SearchAdjustmentInput();
            try
            {
                model.Query = "";
                model.UserId = _currentUser.CurrentUserId;
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _adjustmentsService.SearchAdjustment(model);
                model.Users = _adjustmentsService.GetAllUsers(_currentUser.CompanyName);
                model.Cellars = _adjustmentsService.GetAllCellars(_currentUser.CompanyName);
                model.User = _adjustmentsService.GetUser(_currentUser.CurrentUserId);
                model.Control = "Adjustments";
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
        public ViewResultBase Search(SearchAdjustmentInput model)
        {
            try
            {
                model.UserId = _currentUser.CurrentUserId;
                model.CompanyName = _currentUser.CompanyName;
                var entities = _adjustmentsService.SearchAdjustment(model);
                model.Entities = entities;
                model.Users = _adjustmentsService.GetAllUsers(_currentUser.CompanyName);
                model.Cellars = _adjustmentsService.GetAllCellars(_currentUser.CompanyName);
                model.User = _adjustmentsService.GetUser(_currentUser.CurrentUserId);
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
                return PartialView("_adjustmentListPartial", model);
            }
            return View("Index", model);
        }

        public ActionResult AjaxPage(string query, int? page, Guid? cellarId, Guid? projectId)
        {
            SearchAdjustmentInput model = new SearchAdjustmentInput();
            model.Page = page;
            model.Query = query;
            model.UserId = _currentUser.CurrentUserId;
            model.CellarId = cellarId;
            model.ProjectId = projectId;
            model.User = _adjustmentsService.GetUser(_currentUser.CurrentUserId);
            try
            {
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _adjustmentsService.SearchAdjustment(model);
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
                model.Query = "";
                model.Users = _adjustmentsService.GetAllUsers(_currentUser.CompanyName);
                model.Cellars = _adjustmentsService.GetAllCellars(_currentUser.CompanyName);
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las solicitudes";
            }
            return PartialView("_adjustmentListPartial", model);
        }
       
        public ActionResult Details(Guid id)
        {
            var temp = _adjustmentsService.Get(id);
            temp.Users = _adjustmentsService.GetAllUsers(_currentUser.CompanyName);
            //var temp = null;
            return View("_detail", temp);
        }

        //// GET: Adjustments/Create
        public ActionResult Create()
        {
            CreateAdjustmentInput viewModel = new CreateAdjustmentInput();
            viewModel.IsEdit = false;
            try
            {
                //viewModel.NoRequest = _solicitudMovimientoService.GetNextNoSolicitudMovimiento();
                viewModel.RequestNumber = _adjustmentsService.GetNextRequestNumber(_currentUser.CompanyName);
                viewModel.CompanyName = _currentUser.CompanyName;
                viewModel.Status = AdjustmentStatus.Active;

                //viewModel.TypeAdjustmentValue = (int)TypeAdjustment.Asset;
                //viewModel.TypeAdjustment = TypeAdjustment.Asset;

                viewModel.DetailsAdjustment = new List<DetailAssetAdjustmentDto>();
                viewModel.Cellars = _adjustmentsService.GetAllCellars(_currentUser.CompanyName);
                viewModel.CellarId = viewModel.Cellars.FirstOrDefault().Id;
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
        public ActionResult Create(IList<string> entity)
        {
            CreateAdjustmentInput viewModel = new CreateAdjustmentInput();
            try
            {
                viewModel = JsonConvert.DeserializeObject<CreateAdjustmentInput>(entity[0]);
                //if (string.IsNullOrEmpty(viewModel.RequestDocumentNumber))
                //{ return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener un numero de solicitud fisica." }); }

                if (viewModel.DetailsAdjustment == null || !viewModel.DetailsAdjustment.Any())
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener algún detalle de artículos." }); }

                if (viewModel.CellarId == Guid.Empty)
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe llevar un Ubicación." }); }

                if (ModelState.IsValid)
                {
                    viewModel.CreatorGuidId = _currentUser.CurrentUserId;
                    viewModel.CompanyName = _currentUser.CompanyName; //importante
                    _adjustmentsService.Create(viewModel);
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

        public ActionResult Edit(Guid id)
        {
            CreateAdjustmentInput viewModel = new CreateAdjustmentInput();
            try
            {
                viewModel = _adjustmentsService.GetEdit(id);
                DateTime date;
                viewModel.TypeAdjustmentValue = (int)viewModel.TypeAdjustment;
                if (viewModel.AssetsReturnDate != null)
                {
                    date = new DateTime(viewModel.AssetsReturnDate.Value.Year, viewModel.AssetsReturnDate.Value.Month, viewModel.AssetsReturnDate.Value.Day);
                    viewModel.AssetsReturnDateString = date.ToString("yyyy-MM-dd");
                }

                viewModel.DetailsAdjustment = _adjustmentsService.GetEditDetails(id, viewModel.CellarId);
                viewModel.IsEdit = true;
                viewModel.CompanyName = _currentUser.CompanyName;
                viewModel.Cellars = _adjustmentsService.GetAllCellars(_currentUser.CompanyName);

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


        [HttpPost]
        public ActionResult Edit(IList<string> entity)
        {
            CreateAdjustmentInput viewModel = new CreateAdjustmentInput();
            viewModel.IsEdit = true;
            try
            {
                viewModel = JsonConvert.DeserializeObject<CreateAdjustmentInput>(entity[0]);

                //if (string.IsNullOrEmpty(viewModel.RequestDocumentNumber))
                //{ return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener un numero de solicitud fisica." }); }

                if (viewModel.DetailsAdjustment == null || !viewModel.DetailsAdjustment.Any())
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener algún detalle de artículos." }); }

                if (viewModel.CellarId == Guid.Empty)
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe llevar un Ubicación." }); }

                if (ModelState.IsValid)
                {
                    viewModel.CompanyName = _currentUser.CompanyName;
                    _adjustmentsService.Update(viewModel);
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
        public ActionResult ShowArticuloList(IList<string> jsonDetallesList, string q, Guid cellarId, string typeAdjustment, int? page)
        {
            SearchStockInput newList = new SearchStockInput();
            ViewBag.Query = q;
            ViewBag.BodegaId = cellarId;
            ViewBag.JsonDetallesList = jsonDetallesList;
            int type = Convert.ToInt32(typeAdjustment);
            TypeAdjustment temp = (TypeAdjustment)type;
            newList.Entities = _adjustmentsService.GetAllStocks(q, cellarId, temp, _defaultPageSize, page, _currentUser.CompanyName);
            newList.TotalItem = _adjustmentsService.GetTotalItem(q, cellarId, temp, _currentUser.CompanyName);
            newList.Query = q;
            newList.TypeAdjustment = typeAdjustment;
            newList.CellarId = cellarId;
            return PartialView("_searchAssetPartial", newList);
        }

        [HttpPost]
        public ActionResult SearchArticulos(IList<string> jsonDetallesList, string q, Guid cellarId, string typeAdjustment, int? page)
        {
            SearchStockInput newList = new SearchStockInput();
            ViewBag.Query = q;
            ViewBag.cellarId = cellarId;
            ViewBag.JsonDetallesList = jsonDetallesList;
            int type = Convert.ToInt32(typeAdjustment);
            TypeAdjustment temp = (TypeAdjustment)type;
            newList.Entities = _adjustmentsService.GetAllStocks(q, cellarId, temp, _defaultPageSize, page, _currentUser.CompanyName);
            newList.TotalItem = _adjustmentsService.GetTotalItem(q, cellarId, temp, _currentUser.CompanyName);
            newList.Query = q;
            newList.TypeAdjustment = typeAdjustment;
            newList.CellarId = cellarId;
            return PartialView("_searchAssetPartial", newList);
        }

        [HttpPost]
        public ActionResult AddAndRenderListDetallesOrden(IList<string> jsonDetallesList, Guid? assetId, string nameAsset, double? stockAsset, double? price)
        {
            try
            {
                IEnumerable<DetailAssetAdjustmentDto> detalleList = new JavaScriptSerializer().Deserialize<IList<DetailAssetAdjustmentDto>>(jsonDetallesList[0]);
                if (assetId == null)
                    return Json(new { Error = -1, Message = "Por Favor seleccione el artículo correctamente." });

                if (stockAsset == null || stockAsset < 0)
                    return Json(new { Error = -1, Message = "Por Favor indique la cantidad del artículo correctamente." });

                if (price == null || price < 0)
                    return Json(new { Error = -1, Message = "Por Favor indique el precio de costo del artículo correctamente." });

                if (detalleList.Any(item => item.AssetId == assetId && item.Delete == 0))
                {
                    return Json(new { Error = -1, Message = "Ya existe una salida para el artículo" + nameAsset + " por favor edite correctamente el detalle." });
                }

                DetailAssetAdjustmentDto detalle = new DetailAssetAdjustmentDto();
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
                detalle.Price = (double)price;
                detalle.Saved = 0;
                detalle.Update = 1;
                detalle.Delete = 0;
                detalle.ErrorDescription = "";
                detalle.ErrorCode = 0;

                IList<DetailAssetAdjustmentDto> newList = new List<DetailAssetAdjustmentDto>();
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
        public ActionResult RenderListDetallesOrden(IList<string> jsonDetallesList)
        {
            try
            {
                IEnumerable<DetailAssetAdjustmentDto> detalleList = new JavaScriptSerializer().Deserialize<IList<DetailAssetAdjustmentDto>>(jsonDetallesList[0]);
                IList<DetailAssetAdjustmentDto> newList = new List<DetailAssetAdjustmentDto>();
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

        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {
                Guid userId = _currentUser.CurrentUserId;
                _adjustmentsService.Delete(id, userId,_currentUser.CompanyName);
                return Json(new { ErrorCode = 1, ErrorDescription = "Solicitud Eliminada Correctamente" });
            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = "Error al eliminar la solicitud" });
            }
        }

        [HttpPost]
        public ActionResult ChangeProcessedStatus(Guid adjustmentId)
        {
            try
            {
                _erpConnectionService.ConnectErp(_user, _password, "");
                var list = _erpConnectionService.GetAllStocksByCompany(_currentUser.CompanyName);
                _erpConnectionService.DisconnectErp();
                _adjustmentsService.ChangeProcessedStatus(adjustmentId, _currentUser.CurrentUserId, _currentUser.CompanyName, list);
                return Json(new { ErrorCode = 0, ErrorDescription = "Ajuste aplicado correctamente." });
            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = "Error al cambiar el estado" });
            }
        }
    }
}
