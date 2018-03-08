using System;
using System.Web.Mvc;
using AdministracionActivosSobrantes.Contractors;
using AdministracionActivosSobrantes.Contractors.Dto;
using AdministracionActivosSobrantes.Users;
using AdministracionActivosSobrantes.Web.Account;
using AdministracionActivosSobrantes.Web.Infrastructure;

namespace AdministracionActivosSobrantes.Web.Controllers
{
    [AuthorizeUser(Roles = UserRoles.SuperAdministrador | UserRoles.SupervisorUca)]
    public class ContractorController : Controller
    {
        private readonly IContractorAppService _contractorService;
        private readonly ICurrentUser _currentUser;

        public ContractorController(IContractorAppService categoryService, ICurrentUser currentUser)
        {
            _contractorService = categoryService;
            _currentUser = currentUser;
        }

        [HttpPost]
        public ViewResultBase Search(SearchContractorInput model)
        {
            try
            {
                model.CompanyName = _currentUser.CompanyName;
                var entities = _contractorService.SearchContractor(model);
                model.Entities = entities;
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Transportistas";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_contractorListPartial", model);
            }

            return View("Index", model);
        }


        [HttpPost]
        public ViewResultBase SearchDebounce(string query)
        {
            SearchContractorInput model = new SearchContractorInput();
            try
            {
                model.CompanyName = _currentUser.CompanyName;
                model.Query = query;
                model.Entities = _contractorService.SearchContractor(model);
                model.Control = "Contractor";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Transportistas";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_contractorListPartial", model);
            }
            return View("Index", model);
        }

        public ActionResult Index(int? page)
        {
            SearchContractorInput model = new SearchContractorInput();
            try
            {
                model.Query = "";
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _contractorService.SearchContractor(model);
                model.Control = "Contractor";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Transportistas";
            }
            return View(model);
        }

        public ActionResult AjaxPage(string query, int? page)
        {
            SearchContractorInput model = new SearchContractorInput();
            model.Page = page;
            model.Query = query;

            try
            {
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _contractorService.SearchContractor(model);
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Transportistas";
            }
            return PartialView("_contractorListPartial", model);
        }

        public ActionResult Details(Guid id)
        {
            var entity = _contractorService.GetDetail(id);
            return PartialView("_detailsPartial", entity);
        }

        public ActionResult Create()
        {
            CreateContractorInput viewModel = new CreateContractorInput();
            try
            {
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
        public ActionResult Create(CreateContractorInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    viewModel.CompanyName = _currentUser.CompanyName;
                    viewModel.CreatorGuidId = _currentUser.CurrentUserId;
                    _contractorService.Create(viewModel);
                    ModelState.Clear();

                    var newVm = new CreateContractorInput();
                    newVm.ErrorCode = ErrorCodeHelper.Ok;
                    newVm.ErrorDescription = "¡Transportista guardado exitosamente!";
                    return PartialView("_createPartial", newVm);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al intentar guardar los datos.";
                return PartialView("_createPartial", viewModel);
            }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = e.Message;
                return PartialView("_createPartial", viewModel);
            }
        }

        public ActionResult Edit(Guid id)
        {
            UpdateContractorInput viewModel = new UpdateContractorInput();
            try
            {
                viewModel = _contractorService.GetEdit(id);
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
        public ActionResult Edit(UpdateContractorInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    viewModel.LastModifierUserId = _currentUser.CurrentUserId;
                    viewModel.CompanyName = _currentUser.CompanyName;
                    _contractorService.Update(viewModel);
                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.ErrorDescription = "¡Transportista guardado exitosamente!";
                    return PartialView("_editPartial", viewModel);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error en los datos.";
                return PartialView("_editPartial", viewModel);
            }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = e.Message;
                return PartialView("_editPartial", viewModel);
            }
        }

        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _contractorService.Delete(id, _currentUser.CurrentUserId);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }
    }
}