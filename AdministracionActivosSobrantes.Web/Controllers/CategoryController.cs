using System;
using System.Web.Mvc;
using AdministracionActivosSobrantes.Categories;
using AdministracionActivosSobrantes.Categories.Dto;
using AdministracionActivosSobrantes.Users;
using AdministracionActivosSobrantes.Web.Account;
using AdministracionActivosSobrantes.Web.Infrastructure;

namespace AdministracionActivosSobrantes.Web.Controllers
{
    [AuthorizeUser(Roles = UserRoles.SuperAdministrador | UserRoles.SupervisorUca)]
    public class CategoryController : Controller
    {
        private readonly ICategoryAppService _categoryService;
        private readonly ICurrentUser _currentUser;

        public CategoryController(ICategoryAppService categoryService, ICurrentUser currentUser)
        {
            _categoryService = categoryService;
            _currentUser = currentUser;
        }

        [HttpPost]
        public ViewResultBase Search(SearchCategoryInput model)
        {
            try
            {
                model.CompanyName = _currentUser.CompanyName;
                var entities = _categoryService.SearchCategory(model);
                model.Entities = entities;
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Categorías";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_categoryListPartial", model);
            }
            return View("Index", model);
        }

        [HttpPost]
        public ViewResultBase SearchDebounce(string query)
        {
            SearchCategoryInput model = new SearchCategoryInput();
            try
            {
                model.Query = query;
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _categoryService.SearchCategory(model);
                model.Control = "Category";
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
                return PartialView("_categoryListPartial", model);
            }
            return View("Index", model);
        }

        public ActionResult Index(int? page)
        {
            SearchCategoryInput model = new SearchCategoryInput();
            try
            {
                model.Query = "";
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _categoryService.SearchCategory(model);
                model.Control = "Category";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Categorías";
            }
            return View(model);
        }

        public ActionResult AjaxPage(string query, int? page)
        {
            SearchCategoryInput model = new SearchCategoryInput();
            model.Page = page;
            model.Query = query;
            try
            {
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _categoryService.SearchCategory(model);
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Categorías";
            }
            return PartialView("_categoryListPartial", model);
        }

        public ActionResult Details(Guid id)
        {
            var entity = _categoryService.GetDetail(id);
            return PartialView("_detailsPartial", entity);
        }

        public ActionResult Create()
        {
            CreateCategoryInput viewModel = new CreateCategoryInput();
            try
            {
                viewModel.CompanyName = _currentUser.CompanyName;
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
        public ActionResult Create(CreateCategoryInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    viewModel.CompanyName = _currentUser.CompanyName;
                    viewModel.CreatorGuidId = _currentUser.CurrentUserId;
                    _categoryService.Create(viewModel);
                    ModelState.Clear();

                    var newVm = new CreateCategoryInput();
                    newVm.ErrorCode = ErrorCodeHelper.Ok;
                    newVm.ErrorDescription = "¡Categoría guardada exitosamente!";
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
            UpdatecategoryInput viewModel = new UpdatecategoryInput();
            try
            {
                viewModel.CompanyName = _currentUser.CompanyName;
                viewModel = _categoryService.GetEdit(id);
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
        public ActionResult Edit(UpdatecategoryInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    viewModel.CompanyName = _currentUser.CompanyName;
                    viewModel.LastModifierUserId = _currentUser.CurrentUserId;
                    _categoryService.Update(viewModel);
                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.ErrorDescription = "¡Categoría guardada exitosamente!";
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
                _categoryService.Delete(id, _currentUser.CurrentUserId);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }
    }
}