using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Assets.Dto;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.CustomFields;
using AdministracionActivosSobrantes.CustomFields.Dto;
using AdministracionActivosSobrantes.Users;
using AdministracionActivosSobrantes.Web.Account;
using AdministracionActivosSobrantes.Web.Common;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Web.Infrastructure;
using AdministracionActivosSobrantes.Stocks;
using Newtonsoft.Json;

namespace AdministracionActivosSobrantes.Web.Controllers
{
    [AuthorizeUser(Roles = UserRoles.SuperAdministrador | UserRoles.SupervisorUca )]
    public class AssetController : Controller
    {
        private readonly IAssetsAppService _assetService;
        private readonly ICurrentUser _currentUser;
        private readonly IDateTime _date;
        private readonly IStockAppService _stock;
        

        public AssetController(IAssetsAppService assetService, ICurrentUser currentUser, IDateTime date,IStockAppService stock)
        {
            _assetService = assetService;
            _currentUser = currentUser;
            _date = date;
            _stock = stock;
        }

        [HttpPost]
        public ViewResultBase Search(SearchAssetsInput model)
        {
            try
            {
                model.CompanyName = _currentUser.CompanyName;
                var entities = _assetService.SearchAssets(model);
                model.Entities = entities;
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Artículos";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_assetListPartial", model);
            }
            return View("Index", model);
        }

        [HttpPost]
        public ViewResultBase SearchDebounce(string query)
        {
            SearchAssetsInput model = new SearchAssetsInput();
            try
            {
                model.Query = query;
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _assetService.SearchAssets(model);
                model.Control = "Asset";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Artículos";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_assetListPartial", model);
            }
            return View("Index", model);
        }

        public ActionResult Index(int? page)
        {
            SearchAssetsInput model = new SearchAssetsInput();
            try
            {
                model.Query = "";
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _assetService.SearchAssets(model);
                model.Control = "Asset";
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

        public ActionResult AjaxPage(string query, int? page)
        {
            SearchAssetsInput model = new SearchAssetsInput();
            model.Page = page;
            model.Query = query;

            try
            {
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _assetService.SearchAssets(model);
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Ubicaciones";
            }
            return PartialView("_assetListPartial", model);
        }

        public ActionResult Details(Guid id)
        {
            var entity = _assetService.GetDetail(id,_currentUser.CompanyName);
            return PartialView("_detailsPartial", entity);
        }

       public ActionResult DetailsOut(Guid id)
        {
            ViewBag.Activo = Asset.MaxImagePathLength;
            var entity = _assetService.GetDetail(id, _currentUser.CompanyName);
            return PartialView("_searchAssetPartial", entity);
        }
        public ActionResult Create()
        {
            CreateAssetInput viewModel = new CreateAssetInput();
            viewModel.AdmissionDate = _date.Now;
            try
            {
                viewModel.Categories = _assetService.GetAllCategories(_currentUser.CompanyName);
                //viewModel.Cellars = _assetService.GetAllCellars();
                if (viewModel.Categories != null && viewModel.Categories.Any())
                    viewModel.CategoryId = viewModel.Categories.First().Id;
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return View("_create", viewModel);
        }

        [HttpPost]
        public ActionResult Create(IList<string> entity)
        {
            CreateAssetInput viewModel = new CreateAssetInput();
            try
            {
                viewModel = JsonConvert.DeserializeObject<CreateAssetInput>(System.Web.HttpContext.Current.Request["AssetsDataSection"]);
                var imageToUpload = System.Web.HttpContext.Current.Request.Files["AssetsImagesSection"];

                if (ModelState.IsValid)
                {
                    viewModel.Categories = _assetService.GetAllCategories(_currentUser.CompanyName);

                    if (string.IsNullOrEmpty(viewModel.Code))
                    {
                        ModelState.AddModelError("Code", "*Requerido");
                        viewModel.ErrorCode = ErrorCodeHelper.Error;
                        viewModel.ErrorDescription = "El código es requerido.";
                        return PartialView("_create", viewModel);
                    }
                    if (string.IsNullOrEmpty(viewModel.Name))
                    {
                        ModelState.AddModelError("Name", "*Requerido");
                        viewModel.ErrorCode = ErrorCodeHelper.Error;
                        viewModel.ErrorDescription = "El nombre es requerido.";
                        return PartialView("_create", viewModel);
                    }
                    if (viewModel.Price <= 0)
                    {
                        ModelState.AddModelError("Price", "*Requerido.");
                        viewModel.ErrorCode = ErrorCodeHelper.Error;
                        viewModel.ErrorDescription = "El precio debe ser mayor que 0.";
                        return PartialView("_create", viewModel);
                    }
                    if (viewModel.DepreciationMonthsQty <= 0)
                    {
                        ModelState.AddModelError("DepreciationMonthsQty", "*Requerido.");
                        viewModel.ErrorCode = ErrorCodeHelper.Error;
                        viewModel.ErrorDescription = "La cantidad de meses depreciables debe ser mayor que 0.";
                        return PartialView("_create", viewModel);
                    }
                    if (viewModel.AdmissionDate > _date.Now)
                    {
                        ModelState.AddModelError("AdmissionDate", "* Error.");
                        viewModel.ErrorCode = ErrorCodeHelper.Error;
                        viewModel.ErrorDescription = "La fecha de admisión no puede ser mayor a la fecha de hoy.";
                        return PartialView("_create", viewModel);
                    }

                    if (imageToUpload != null)
                    {
                        viewModel.Image = imageToUpload;
                        if (!CheckImageFormat.IsImage(imageToUpload))
                        {
                            viewModel.ErrorCode = ErrorCodeHelper.Error;
                            viewModel.ErrorDescription = "Error al cargar la imagen seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg, *.PDF";
                            return PartialView("_create", viewModel);
                        }
                    }

                    viewModel.CreatorGuidId = _currentUser.CurrentUserId;
                    viewModel.CompanyName = _currentUser.CompanyName;
                    _assetService.Create(viewModel);
                    ModelState.Clear();
                    var newVm = new CreateAssetInput();
                    newVm.Categories = _assetService.GetAllCategories(_currentUser.CompanyName);
                    newVm.AdmissionDate = DateTime.Now;
                    newVm.ErrorCode = ErrorCodeHelper.Ok;
                    newVm.ErrorDescription = "¡Artículo guardado exitosamente!";
                    return PartialView("_create", newVm);
                }
                viewModel.Categories = _assetService.GetAllCategories(_currentUser.CompanyName);
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al intentar guardar los datos.";
                return PartialView("_create", viewModel);
            }
            catch (Exception e)
            {
                viewModel.Categories = _assetService.GetAllCategories(_currentUser.CompanyName);
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = e.Message;
                return PartialView("_create", viewModel);
            }
        }

        public ActionResult Edit(Guid id)
        {
            UpdateAssetInput viewModel = new UpdateAssetInput();
            try
            {
                viewModel = _assetService.GetEdit(id,_currentUser.CompanyName);
                viewModel.CompanyName = _currentUser.CompanyName;
                viewModel.NowDate = _date.Now;
                viewModel.CustomFieldsDto = _assetService.GetEditCustomFields(viewModel.Id,_currentUser.CompanyName);
                viewModel.DetailAssetToolKitsDto = _assetService.GetEditToolKits(id,_currentUser.CompanyName);
                viewModel.Categories = _assetService.GetAllCategories(_currentUser.CompanyName);
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
               // ViewBag.Stock = _stock.GetStockInformation(id, _currentUser.CompanyName);
                
             }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return View("_edit",viewModel);
        }


        [HttpPost]
        public ActionResult Edit()//IList<string> entity
        {
            UpdateAssetInput viewModel = new UpdateAssetInput();
            try
            {
               
                viewModel = JsonConvert.DeserializeObject<UpdateAssetInput>(System.Web.HttpContext.Current.Request["AssetsDataSection"]);
                var imageToUpload = System.Web.HttpContext.Current.Request.Files["AssetsImagesSection"];
                var guidid = viewModel.Id;
                var detalles = _assetService.Get(guidid);
                viewModel.NowDate = _date.Now;
                viewModel.Categories = _assetService.GetAllCategories(_currentUser.CompanyName);
                if (ModelState.IsValid)
                {
                    /* if (string.IsNullOrEmpty(viewModel.Code))
                     {
                         ModelState.AddModelError("Code", "* Requerido");
                         viewModel.ErrorCode = ErrorCodeHelper.Error;
                         viewModel.ErrorDescription = "El código es requerido.";
                         viewModel.NowDate = _date.Now;
                         return PartialView("_edit", viewModel);
                     }
                     if (string.IsNullOrEmpty(viewModel.Name))
                     {
                         ModelState.AddModelError("Name", "* Requerido");
                         viewModel.ErrorCode = ErrorCodeHelper.Error;
                         viewModel.ErrorDescription = "El nombre es requerido.";
                         viewModel.NowDate = _date.Now;
                         return PartialView("_edit", viewModel);
                     }
                     if (viewModel.Price <= 0)
                     {
                         ModelState.AddModelError("Price", "* Requerido.");
                         viewModel.ErrorCode = ErrorCodeHelper.Error;
                         viewModel.ErrorDescription = "El precio debe ser mayor que 0.";
                         viewModel.NowDate = _date.Now;
                         return PartialView("_edit", viewModel);
                     }
                     if (viewModel.DepreciationMonthsQty <= 0)
                     {
                         ModelState.AddModelError("DepreciationMonthsQty", "* Requerido.");
                         viewModel.ErrorCode = ErrorCodeHelper.Error;
                         viewModel.ErrorDescription = "La cantidad de meses depreciables debe ser mayor que 0.";
                         viewModel.NowDate = _date.Now;
                         return PartialView("_edit", viewModel);
                     }*/
                    viewModel.MensualDepreciation = detalles.MensualDepreciation;
                    viewModel.CostinBooks = detalles.CostinBooks;
                    if (imageToUpload != null)
                    {
                        viewModel.Image = imageToUpload;
                        if (!CheckImageFormat.IsImage(imageToUpload))
                        {
                            viewModel.ErrorCode = ErrorCodeHelper.Error;
                            viewModel.ErrorDescription = "Error al cargar la imagen seleccionada, la extensión debe ser de tipo *.jpg, *.png, *.gif, *.jpeg, *.PDF";
                            return PartialView("_edit", viewModel);
                        }
                    }

                    viewModel.LastModifierUserId = _currentUser.CurrentUserId;
                    viewModel.CompanyName = _currentUser.CompanyName;
                    _assetService.Update(viewModel);
                    viewModel.CustomFieldsDto = _assetService.GetEditCustomFields(viewModel.Id,_currentUser.CompanyName);
                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.NowDate = _date.Now;
                    viewModel.ErrorDescription = "¡Artículo actualizado exitosamente!";
                    return PartialView("_edit", viewModel);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error en los datos.";
                viewModel.NowDate = _date.Now;
                return PartialView("_edit", viewModel);
            }
            catch (Exception e)
            {
                viewModel.Categories = _assetService.GetAllCategories(_currentUser.CompanyName);
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
                viewModel.NowDate = _date.Now;
                return PartialView("_edit", viewModel);
            }
        }

        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _assetService.Delete(id, _currentUser.CurrentUserId);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }

        //Custom Fields---------------
        [HttpPost]
        public ActionResult AddAndRenderListCustomFields(IList<string> jsonCustomFieldsList, Guid assetId, string name, object value, int type)
        {
            try
            {
                IEnumerable<CustomFieldDto> customList = new JavaScriptSerializer().Deserialize<IList<CustomFieldDto>>(jsonCustomFieldsList[0]);
                if (string.IsNullOrEmpty(name))
                    return Json(new { Error = -1, Message = "Por Favor indique el nombre del campo." });

                object valueParse = ((IEnumerable<string>)value).FirstOrDefault();
                if (string.IsNullOrEmpty(Convert.ToString(valueParse)))
                    return Json(new { Error = -1, Message = "Por Favor ingrese un valor al campo." });

                CustomFieldDto custom = new CustomFieldDto();
                custom.Name = name;
                custom.SetValue((CustomFieldType)type, valueParse);
                custom.Saved = 0;
                custom.Update = 1;
                custom.Delete = 0;
                custom.ErrorDescription = "";
                custom.ErrorCode = 0;
                custom.Id = Guid.Empty;

                IList<CustomFieldDto> newList = new List<CustomFieldDto>();
                int index = 0;
                foreach (var item in customList)
                {
                    item.Index = index;
                    newList.Add(item);
                    index++;
                }
                custom.Index = index;
                newList.Add(custom);
                return PartialView("_customFieldsPartial", newList);
            }
            catch (Exception e)
            {
                return Json(new { Error = -1, Message = "Error al Agregar/Modificar el campo customizable" });
            }
        }

        [HttpPost]
        public ActionResult RenderListCustomFields(IList<string> jsonCustomFieldsList)
        {
            try
            {
                IEnumerable<CustomFieldDto> customList = new JavaScriptSerializer().Deserialize<IList<CustomFieldDto>>(jsonCustomFieldsList[0]);
                IList<CustomFieldDto> newList = new List<CustomFieldDto>();
                int index = 0;
                foreach (var item in customList)
                {
                    if (item.Delete == 1)
                    {
                        if (item.Id.Equals(Guid.Empty))
                            continue;// se brinca el que se borró y no existe en bd
                        item.Index = -1;
                    }
                    else
                    {
                        //if (item.Update == 1)
                        //    return Json(new { Error = -1, Message = "No hay suficientes artículos en Ubicación seleccionada. Por Favor revise las existencias." });
                        item.Index = index;
                        index++;
                    }
                    newList.Add(item);
                }
                return PartialView("_customFieldsPartial", newList);
            }
            catch (Exception)
            {
                return Json(new { Error = -1, Message = "Error al Agregar/Modificar los Campos" });
            }
        }

        //Custom Fields---------------
        [HttpPost]
        public ActionResult AddAndRenderListToolKits(IList<string> jsonToolKitsList, Guid assetId, string name, string code, double quatity)
        {
            try
            {
                IEnumerable<DetailAssetToolKitsDto> customList = new JavaScriptSerializer().Deserialize<IList<DetailAssetToolKitsDto>>(jsonToolKitsList[0]);
                if (string.IsNullOrEmpty(name))
                    return Json(new { Error = -1, Message = "Por Favor indique el nombre del campo." });


                DetailAssetToolKitsDto toolKit = new DetailAssetToolKitsDto();
                toolKit.Name = name;
                toolKit.Code = code;
                toolKit.AssetId = assetId;
                toolKit.Quatity = quatity;
                toolKit.Saved = 0;
                toolKit.Update = 1;
                toolKit.Delete = 0;
                toolKit.ErrorDescription = "";
                toolKit.ErrorCode = 0;
                toolKit.Id = Guid.Empty;

                IList<DetailAssetToolKitsDto> newList = new List<DetailAssetToolKitsDto>();
                int index = 0;
                foreach (var item in customList)
                {
                    item.Index = index;
                    newList.Add(item);
                    index++;
                }
                toolKit.Index = index;
                newList.Add(toolKit);
                return PartialView("_tableToolKits", newList);
            }
            catch (Exception e)
            {
                return Json(new { Error = -1, Message = "Error al Agregar/Modificar el campo customizable" });
            }
        }

        [HttpPost]
        public ActionResult RenderListToolKits(IList<string> jsonToolKitsList)
        {
            try
            {
                IEnumerable<DetailAssetToolKitsDto> customList = new JavaScriptSerializer().Deserialize<IList<DetailAssetToolKitsDto>>(jsonToolKitsList[0]);
                IList<DetailAssetToolKitsDto> newList = new List<DetailAssetToolKitsDto>();
                int index = 0;
                foreach (var item in customList)
                {
                    if (item.Delete == 1)
                    {
                        if (item.Id.Equals(Guid.Empty))
                            continue;// se brinca el que se borró y no existe en bd
                        item.Index = -1;
                    }
                    else
                    {
                        //if (item.Update == 1)
                        //    return Json(new { Error = -1, Message = "No hay suficientes artículos en Ubicación seleccionada. Por Favor revise las existencias." });
                        item.Index = index;
                        index++;
                    }
                    newList.Add(item);
                }
                return PartialView("_tableToolKits", newList);
            }
            catch (Exception)
            {
                return Json(new { Error = -1, Message = "Error al Agregar/Modificar los Campos" });
            }
        }

        [HttpPost]
        public ActionResult ShowArticuloList(int? page, string q)
        {
            ViewBag.Query = q;
            var list = _assetService.SearchAssetToolKits(q, page,_currentUser.CompanyName);
            return PartialView("_showAseetsListPartial", list);
        }

        public ActionResult SearchArticulos(string q, int? page)
        {
            ViewBag.Query = q;
            var list = _assetService.SearchAssetToolKits(q, page,_currentUser.CompanyName);
            return PartialView("_showAseetsListPartial", list);
        }

        [HttpPost]
        public ViewResultBase SearchDebounceAssetList(string query)
        {
            var list = _assetService.SearchAssetToolKits(query, null,_currentUser.CompanyName);
            return PartialView("_showAseetsListPartial", list);
        }

        public ActionResult EnableToolKit()
        {
            IList<DetailAssetToolKitsDto> newList = new List<DetailAssetToolKitsDto>();
            return PartialView("_toolKitPartial", newList);
        }
        //--------------------
    }
}