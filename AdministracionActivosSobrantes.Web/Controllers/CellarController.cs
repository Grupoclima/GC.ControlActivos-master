using System;
using System.Web.Mvc;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Cellars.Dto;
using AdministracionActivosSobrantes.Users;
using AdministracionActivosSobrantes.Web.Account;
using AdministracionActivosSobrantes.Web.Infrastructure;

namespace AdministracionActivosSobrantes.Web.Controllers
{
    [AuthorizeUser(Roles = UserRoles.SuperAdministrador | UserRoles.SupervisorUca )]
    public class CellarController : Controller
    {
        private readonly ICellarAppService _cellarService;
        private readonly ICurrentUser _currentUser;

        public CellarController(ICellarAppService cellarService, ICurrentUser currentUser)
        {
            _cellarService = cellarService;
            _currentUser = currentUser;
        }

        [HttpPost]
        public ViewResultBase Search(SearchCellarsInput model)
        {
            try
            {
                model.CompanyName = _currentUser.CompanyName;
                var entities = _cellarService.SearchCellar(model);
                model.Entities = entities;
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Ubicaciones";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_cellarListPartial", model);
            }

            return View("Index", model);
        }

        [HttpPost]
        public ViewResultBase SearchDebounce(string query)
        {
            SearchCellarsInput model = new SearchCellarsInput();
            try
            {
                model.CompanyName = _currentUser.CompanyName;
                model.Query = query;
                model.Entities = _cellarService.SearchCellar(model);
                model.Control = "Cellar";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Ubicaciones";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_cellarListPartial", model);
            }
            return View("Index", model);
        }

        public ActionResult Index(int? page)
        {
            SearchCellarsInput model = new SearchCellarsInput();
            try
            {
                model.Query = "";
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _cellarService.SearchCellar(model);
                model.Control = "Cellar";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Ubicaciones";
            }
            return View(model);
        }

        public ActionResult AjaxPage(string query, int? page)
        {
            SearchCellarsInput model = new SearchCellarsInput();
            model.Page = page;
            model.Query = query;
            model.CompanyName = _currentUser.CompanyName;
            try
            {
                model.Entities = _cellarService.SearchCellar(model);
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Ubicaciones";
            }
            return PartialView("_cellarListPartial", model);
        }
        

        public ActionResult Details(Guid id)
        {
            var entity = _cellarService.GetDetail(id);
            return PartialView("_detailsPartial", entity);
        }

        public ActionResult Create()
        {
            CreateCellarInput viewModel = new CreateCellarInput();
            try
            {
                viewModel.CompanyName = _currentUser.CompanyName;
                viewModel.WareHouseUsers = _cellarService.GetAllWareHouseUsers(_currentUser.CompanyName);
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return PartialView("_createPartial", viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateCellarInput viewModel)
        {
            try
            {
                //var currentUserId = (CustomPrincipal)HttpContext.User
                if (ModelState.IsValid)
                {
                    //var temp = new Cellar();
                    //Cellar lastCellar = _cellarService.FilterBy(b => b.Active, "").OrderByDescending(b => b.NoCellar).FirstOrDefault();
                    //if (lastCellar != null)
                    //    viewModel.NoCellar = lastCellar.NoCellar + 1;
                    //else
                    //    viewModel.NoCellar = 1;
                    viewModel.CompanyName = _currentUser.CompanyName;
                    viewModel.CreatorGuidId = _currentUser.CurrentUserId;
                    _cellarService.Create(viewModel);

                    ModelState.Clear();

                    var newVm = new CreateCellarInput();
                    newVm.WareHouseUsers = _cellarService.GetAllWareHouseUsers(_currentUser.CompanyName);
                    newVm.ErrorCode = ErrorCodeHelper.Ok;
                    newVm.ErrorDescription = "¡Ubicación guardada exitosamente!";
                    return PartialView("_createPartial", newVm);
                    //return Json(newVm, JsonRequestBehavior.AllowGet);
                }
                viewModel.WareHouseUsers = _cellarService.GetAllWareHouseUsers(_currentUser.CompanyName);
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al intentar guardar los datos.";
                return PartialView("_createPartial", viewModel);
            }
            catch (Exception e)
            {
                viewModel.WareHouseUsers = _cellarService.GetAllWareHouseUsers(_currentUser.CompanyName);
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = e.Message;
                return PartialView("_createPartial", viewModel);
            }
        }

        public ActionResult Edit(Guid id)
        {
            UpdateCellarInput viewModel = new UpdateCellarInput();
            try
            {
                viewModel.CompanyName = _currentUser.CompanyName;
                viewModel = _cellarService.GetEdit(id);
                viewModel.WareHouseUsers = _cellarService.GetAllWareHouseUsers(_currentUser.CompanyName);
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return PartialView("_editPartial", viewModel);
        }

        [HttpPost]
        public ActionResult Edit(UpdateCellarInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    viewModel.CompanyName = _currentUser.CompanyName;
                    viewModel.LastModifierUserId = _currentUser.CurrentUserId;
                    _cellarService.Update(viewModel);
                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.WareHouseUsers = _cellarService.GetAllWareHouseUsers(_currentUser.CompanyName);
                    viewModel.ErrorDescription = "¡Ubicación guardada exitosamente!";
                    return PartialView("_editPartial", viewModel);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.WareHouseUsers = _cellarService.GetAllWareHouseUsers(_currentUser.CompanyName);
                viewModel.ErrorDescription = "Error en los datos.";
                return PartialView("_editPartial", viewModel);
            }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.WareHouseUsers = _cellarService.GetAllWareHouseUsers(_currentUser.CompanyName);
                viewModel.ErrorDescription = e.Message;
                return PartialView("_editPartial", viewModel);
            }
        }

        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _cellarService.Delete(id,_currentUser.CurrentUserId);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ChangeStatus(Guid id)
        {
            try
            {
                _cellarService.ChangeStatus(id, _currentUser.CurrentUserId);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }
    }
}