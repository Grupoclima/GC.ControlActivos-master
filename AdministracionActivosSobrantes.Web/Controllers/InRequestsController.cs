using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AdministracionActivosSobrantes.Assets.Dto;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.InRequest;
using AdministracionActivosSobrantes.InRequest.Dto;
using AdministracionActivosSobrantes.OutRequest.Dto;
using AdministracionActivosSobrantes.Users;
using AdministracionActivosSobrantes.Web.Account;
using AdministracionActivosSobrantes.Web.Common;
using AdministracionActivosSobrantes.Web.Infrastructure;
using GcErpConnection;
using Newtonsoft.Json;

namespace AdministracionActivosSobrantes.Web.Controllers
{
    [AuthorizeUser(Roles = UserRoles.SuperAdministrador | UserRoles.SupervisorUca | UserRoles.Coordinador | UserRoles.AuxiliarUca | UserRoles.Solicitante)]
    public class InRequestsController : AdministracionActivosSobrantesControllerBase
    {
        private readonly IInRequestAppService _inRequestService;
        private readonly ICellarAppService _cellarService;
        private readonly ICurrentUser _currentUser;
        private readonly IGcErpConnectionService _erpConnectionService;
        private readonly IUserAppService _userAppService;
        string _user = ConfigurationManager.AppSettings["DbUser"];
        string _password = ConfigurationManager.AppSettings["DbPassword"];
        // GET: InRequests
        private readonly int _defaultPageSize = 15;
        public InRequestsController(IInRequestAppService inRequestService, ICurrentUser currentUser, ICellarAppService cellarService, IGcErpConnectionService erpConnectionService, IUserAppService userAppService)
        {
            _inRequestService = inRequestService;
            _cellarService = cellarService;
            _currentUser = currentUser;
            _erpConnectionService = erpConnectionService;
            _userAppService = userAppService;
        }

        public ActionResult Index(int? page)
        {
            SearchInRequestInput model = new SearchInRequestInput();
            try
            {
                model.Query = "";
                model.UserId = _currentUser.CurrentUserId;
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _inRequestService.SearchInRequest(model);
                model.Users = _inRequestService.GetAllUsers(_currentUser.CompanyName);
                model.Cellars = _inRequestService.GetAllCellars(_currentUser.CompanyName);
                model.User = _inRequestService.GetUser(_currentUser.CurrentUserId);
                model.Control = "InRequests";
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
        public ViewResultBase Search(SearchInRequestInput model)
        {
            try
            {
                model.UserId = _currentUser.CurrentUserId;
                model.CompanyName = _currentUser.CompanyName;
                var entities = _inRequestService.SearchInRequest(model);
                model.Entities = entities;
                model.Users = _inRequestService.GetAllUsers(_currentUser.CompanyName);
                model.Cellars = _inRequestService.GetAllCellars(_currentUser.CompanyName);
                model.User = _inRequestService.GetUser(_currentUser.CurrentUserId);
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
                return PartialView("_inRequestListPartial", model);
            }
            return View("Index", model);
        }

        public ActionResult AjaxPage(string query, int? page, Guid? cellarId, Guid? projectId)
        {
            SearchInRequestInput model = new SearchInRequestInput();
            model.Page = page;
            model.Query = query;
            model.UserId = _currentUser.CurrentUserId;
            model.CellarId = cellarId;
            model.ProjectId = projectId;
            model.User = _inRequestService.GetUser(_currentUser.CurrentUserId);
           // model.Stocks = _inRequestService.GetAllStocks(_currentUser.CompanyName);

            try
            {
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _inRequestService.SearchInRequest(model);
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
                model.Query = "";
                model.Users = _inRequestService.GetAllUsers(_currentUser.CompanyName);
                model.Cellars = _inRequestService.GetAllCellars(_currentUser.CompanyName);
                
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las solicitudes";
            }
            return PartialView("_inRequestListPartial", model);
        }

        public ActionResult Details(Guid id)
        {
            var temp = _inRequestService.Get(id);
            temp.SignatureData = "data:" + temp.SignatureData;
            temp.Users = _inRequestService.GetAllUsers(_currentUser.CompanyName);
            //var temp = null;
            return View("_detail", temp);
        }

        //// GET: InRequests/Create
        public ActionResult Create(int? param)
        {
            CreateInRequestInput viewModel = new CreateInRequestInput();
            viewModel.IsEdit = false;
            try
            {
                //viewModel.NoRequest = _solicitudMovimientoService.GetNextNoSolicitudMovimiento();
                viewModel.CompanyName = _currentUser.CompanyName;
                viewModel.RequestNumber = _inRequestService.GetNextRequestNumber(_currentUser.CompanyName);
                viewModel.Status = InRequestStatus.Active;
                viewModel.StateRequest = (int)InRequestStatus.Active;
                viewModel.DetailsRequest = new List<DetailAssetInRequestDto>();
                viewModel.Cellars = _inRequestService.GetAllCellars(_currentUser.CompanyName);
                viewModel.CellarId = viewModel.Cellars.FirstOrDefault().Id;
                var firstOrDefault = _cellarService.GetAllCellar(_currentUser.CompanyName).FirstOrDefault();
                if (firstOrDefault != null)
                    viewModel.CellarId = firstOrDefault.Id;
                viewModel.ProjectId = null;
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
            CreateInRequestInput viewModel = new CreateInRequestInput();
            try
            {
                viewModel = JsonConvert.DeserializeObject<CreateInRequestInput>(System.Web.HttpContext.Current.Request["InRequestDataSection"]);
                var imageToUpload1 = System.Web.HttpContext.Current.Request.Files["InRequestImagesSection1"];
                var imageToUpload2 = System.Web.HttpContext.Current.Request.Files["InRequestImagesSection2"];
                var signature = System.Web.HttpContext.Current.Request.Form["SignatureStr"];
               
                viewModel.SignatureData = signature;
               // _currentUser.
                //if (viewModel.RequestDocumentNumber == String.Empty || viewModel.RequestDocumentNumber == null)
                //{ return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener un numero de solicitud fisica." }); }

                if (viewModel.DetailsRequest == null || !viewModel.DetailsRequest.Any())
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener algún detalle de artículos." }); }

                //if (viewModel.PurchaseOrderNumber == String.Empty || viewModel.PurchaseOrderNumber == null)
                //{ return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener algún número de orden de compra." }); }

                if (viewModel.PersonInCharge == String.Empty || viewModel.PersonInCharge == null)
                { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener algún responsable." }); }

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

                    viewModel.CreatorGuidId = _currentUser.CurrentUserId;
                    viewModel.CompanyName = _currentUser.CompanyName;
                    try
                    {
                        _inRequestService.Create(viewModel);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { ErrorCode = -1, ErrorDescription = "Error al guardar la solicitud, verifique los datos." });

                    }
                   
                    ModelState.Clear();
                    return Json(new { ErrorCode = 1, ErrorDescription = "Orden de Compra guardada exitosamente." });
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
            CreateInRequestInput viewModel = new CreateInRequestInput();
            try
            {
                viewModel = _inRequestService.GetEdit(id);
                DateTime date;
                
                    viewModel.TypeInRequestValue = (int)viewModel.TypeInRequest;
                    if (viewModel.AssetsReturnDate != null)
                    {
                        date = new DateTime(viewModel.AssetsReturnDate.Value.Year, viewModel.AssetsReturnDate.Value.Month, viewModel.AssetsReturnDate.Value.Day);
                        viewModel.AssetsReturnDateString = date.ToString("yyyy-MM-dd");
                    }
                    viewModel.StateRequest = (int)viewModel.Status;
                    viewModel.DetailsRequest = _inRequestService.GetEditDetails(id, viewModel.CellarId);
                    viewModel.IsEdit = true;
                    viewModel.Cellars = _inRequestService.GetAllCellars(_currentUser.CompanyName);
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
        public ActionResult Edit()//IList<string> entity
        {
            CreateInRequestInput viewModel = new CreateInRequestInput();
            viewModel.IsEdit = true;
            try
            {
                if(viewModel.Status == InRequestStatus.Processed || viewModel.Status == InRequestStatus.Closed)
                {
                    return Json(new { ErrorCode = -1, ErrorDescription = "Refresque la pagina y verifique los datos, no puede modificar una solicitud ya procesada" });
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<CreateInRequestInput>(System.Web.HttpContext.Current.Request["InRequestDataSection"]);
                    var imageToUpload1 = System.Web.HttpContext.Current.Request.Files["InRequestImagesSection1"];
                    var imageToUpload2 = System.Web.HttpContext.Current.Request.Files["InRequestImagesSection2"];

                    //if (viewModel.RequestDocumentNumber == String.Empty || viewModel.RequestDocumentNumber == null)
                    //{ return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener un numero de solicitud fisica." }); }

                    if (viewModel.DetailsRequest == null || !viewModel.DetailsRequest.Any())
                    { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener algún detalle de artículos." }); }

                    //if (viewModel.PurchaseOrderNumber == String.Empty || viewModel.PurchaseOrderNumber == null)
                    //{ return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener algún número de orden de compra." }); }

                    if (viewModel.PersonInCharge == String.Empty || viewModel.PersonInCharge == null)
                    { return Json(new { ErrorCode = -1, ErrorDescription = "La solicitud debe tener algún responsable." }); }

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

                        if (viewModel.Image1 == null && viewModel.Image2 == null)
                        {
                            return Json(new { ErrorCode = ErrorCodeHelper.Error, ErrorDescription = "La solicitud para ser entregada debe tener almenos una imagen adjunta." });
                        }

                        viewModel.CompanyName = _currentUser.CompanyName;
                        _inRequestService.Update(viewModel);
                        return Json(new { ErrorCode = 1, ErrorDescription = "Solicitud guardada exitosamente." });
                    }

                    return Json(new { ErrorCode = -1, ErrorDescription = "Error al editar la solicitud, verifique los datos." });
                }
            }
              
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = e.Message });
            }
        }

        [HttpPost]
        public ActionResult ShowArticuloList(int? page, int? typeInRequestValue, string q)
        {
            SearchAssetPartialInput newList = new SearchAssetPartialInput();
            ViewBag.Query = q;
            newList.Query = q;
            newList.TypeInRequestValue = typeInRequestValue.ToString();
            int type = Convert.ToInt32(typeInRequestValue);
            TypeInRequest temp = (TypeInRequest)type;
            newList.Entities = _inRequestService.SearchAssets(q, page, temp, _currentUser.CompanyName);
            newList.TotalItem = _inRequestService.TotalCount(q, page, temp, _currentUser.CompanyName);
            return PartialView("_searchAssetPartial", newList);
        }

        public ActionResult SearchArticulos(string q, int? typeInRequestValue, int? page)
        {
            SearchAssetPartialInput newList = new SearchAssetPartialInput();
            newList.Query = q;
            newList.TypeInRequestValue = typeInRequestValue.ToString();
            ViewBag.Query = q;
            int type = Convert.ToInt32(typeInRequestValue);
            TypeInRequest temp = (TypeInRequest)type;
            newList.Entities = _inRequestService.SearchAssets(q, page, temp, _currentUser.CompanyName);
            newList.TotalItem = _inRequestService.TotalCount(q, page, temp, _currentUser.CompanyName);
            return PartialView("_searchAssetPartial", newList);
        }

        [HttpPost]
        public ActionResult AddAndRenderListDetallesOrden(IList<string> jsonDetallesList, Guid? assetId, string nameAsset, double? stockAsset, string price)
        {
            try
            {
                IEnumerable<DetailAssetInRequestDto> detalleList = new JavaScriptSerializer().Deserialize<IList<DetailAssetInRequestDto>>(jsonDetallesList[0]);

                if (assetId == null)
                    return Json(new { Error = -1, Message = "Por Favor seleccione el artículo correctamente." });

                if (stockAsset == null || stockAsset < 0)
                    return Json(new { Error = -1, Message = "Por Favor indique la cantidad del artículo correctamente." });

              


                if (detalleList.Any(item => item.AssetId == assetId && item.Delete == 0))
                {
                    return Json(new { Error = -1, Message = "Ya existe una salida para el artículo" + nameAsset + " por favor edite correctamente el detalle." });
                }

                DetailAssetInRequestDto detalle = new DetailAssetInRequestDto();
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
                detalle.Saved = 0;
                detalle.Update = 1;
                detalle.Delete = 0;
                detalle.ErrorDescription = "";
                detalle.ErrorCode = 0;

                IList<DetailAssetInRequestDto> newList = new List<DetailAssetInRequestDto>();
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
        public ActionResult AddListDetallesOrden(IList<string> jsonDetallesList, string code)
        {
            try
            {
                IEnumerable<DetailAssetInRequestDto> detalleList = new JavaScriptSerializer().Deserialize<IList<DetailAssetInRequestDto>>(jsonDetallesList[0]);
                IList<DetailAssetInRequestDto> newList = new List<DetailAssetInRequestDto>();
                var asset = _inRequestService.GetAssetBarCode(code, _currentUser.CompanyName);

                if (asset == null)
                    return Json(new { Error = -1, Message = "No se ha encontrado un artículo con este código de barra" });

                // var stock = _inRequestService.GetStockAsset(asset.Id, _currentUser.CompanyName, cellarId);


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
                            //if (item.StockAsset > stock.GetStockItemsQtyBlocked())
                              //  return Json(new { Error = -1, Message = "No hay suficientes artículos en Ubicación seleccionada. Por Favor revise las existencias." });
                        }

                        item.Index = index;
                        newList.Add(item);
                        index++;
                    }
                }


                if (!cambio)
                {

                    DetailAssetInRequestDto detalle = new DetailAssetInRequestDto();
                    detalle.AssetId = asset.Id;
                    detalle.NameAsset = asset.Name;
                    detalle.StockAsset = 1;
                    
                    detalle.Price = asset.Price.ToString();
                    detalle.Saved = 0;
                    detalle.Update = 1;
                    detalle.Delete = 0;
                    detalle.ErrorDescription = "";
                    detalle.ErrorCode = 0;

                    int index = 0;
                    foreach (var item in detalleList)
                    {
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
        public ActionResult RenderListDetallesOrden(IList<string> jsonDetallesList)
        {
            try
            {
                IEnumerable<DetailAssetInRequestDto> detalleList = new JavaScriptSerializer().Deserialize<IList<DetailAssetInRequestDto>>(jsonDetallesList[0]);
                IList<DetailAssetInRequestDto> newList = new List<DetailAssetInRequestDto>();
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
            var status = _inRequestService.GetDetail(id);

            try
            {
                if(status.Status == InRequestStatus.Processed || status.Status == InRequestStatus.Closed)
                {
                    return Json(new { ErrorCode = -1, ErrorDescription = "Reresque la pagina, la solicitud no puede ser eliminada por que ya fue procesada" });
                }
                else
                {
                    Guid userId = _currentUser.CurrentUserId;
                    _inRequestService.Delete(id, userId);
                    return Json(new { ErrorCode = 1, ErrorDescription = "Solicitud Eliminada Correctamente" });
                }
                
            }
            catch (Exception e)
            {
                return Json(new { ErrorCode = -1, ErrorDescription = "Error al eliminar la solicitud" });
            }
        }

        [HttpPost]
        public ActionResult ChangeProcessedStatus(Guid inRequestId)
        {
            var entrada = _inRequestService.GetDetail(inRequestId);

            if (entrada.Status != InRequestStatus.Processed)
            {
                try
                {
                    _erpConnectionService.ConnectErp(_user, _password, "");
                    var list = _erpConnectionService.GetAllStocksByCompany(_currentUser.CompanyName);
                    _erpConnectionService.DisconnectErp();
                    _inRequestService.ChangeProcessedStatus(inRequestId, _currentUser.CurrentUserId, _currentUser.CompanyName, list);
                    return Json(new { ErrorCode = 0, ErrorDescription = "Entrada aplicada correctamente." });
                }
                catch (Exception e)
                {
                    return Json(new { ErrorCode = -1, ErrorDescription = "Error al cambiar el estado" });
                }
            }
            else
            {
                return Json(new { ErrorCode = -1, ErrorDescription = "Favor Refrescar la pagina, su entrada ya fue procesada" });

            }
        }
    }
}
