using System;
using System.Web.Mvc;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Projects.Dto;
using AdministracionActivosSobrantes.Users;
using AdministracionActivosSobrantes.Web.Account;
using AdministracionActivosSobrantes.Web.Infrastructure;

namespace AdministracionActivosSobrantes.Web.Controllers
{
    [AuthorizeUser(Roles = UserRoles.SuperAdministrador | UserRoles.SupervisorUca)]
    public class ProjectController : Controller
    {
        private readonly IProjectAppService _projectService;
        private readonly ICurrentUser _currentUser;

        public ProjectController(IProjectAppService projectService, ICurrentUser currentUser)
        {
            _projectService = projectService;
            _currentUser = currentUser;
        }

        [HttpPost]
        public ViewResultBase Search(SearchProjectsInput model)
        {
            try
            {
                model.CompanyName = _currentUser.CompanyName;
                var entities = _projectService.SearchProject(model);
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
                return PartialView("_projectListPartial", model);
            }

            return View("Index", model);
        }

        [HttpPost]
        public ViewResultBase SearchDebounce(string query)
        {
            SearchProjectsInput model = new SearchProjectsInput();
            try
            {
                model.Query = query;
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _projectService.SearchProject(model);
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
                return PartialView("_projectListPartial", model);
            }
            return View("Index", model);
        }

        public ActionResult Index(int? page)
        {
            SearchProjectsInput model = new SearchProjectsInput();
            try
            {
                model.Query = "";
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _projectService.SearchProject(model);
                model.Control = "Project";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Proyectos";
            }
            return View(model);
        }

        public ActionResult AjaxPage(string query, int? page)
        {
            SearchProjectsInput model = new SearchProjectsInput();
            model.Page = page;
            model.Query = query;

            try
            {
                model.CompanyName = _currentUser.CompanyName;
                model.Entities = _projectService.SearchProject(model);
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Proyectos";
            }
            return PartialView("_ProjectListPartial", model);
        }
        

        public ActionResult Details(Guid id)
        {
            var entity = _projectService.GetDetail(id);
            return PartialView("_detailsPartial", entity);
        }

        public ActionResult Create()
        {
            CreateProjectInput viewModel = new CreateProjectInput();
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
        public ActionResult Create(CreateProjectInput viewModel)
        {
            try
            {
                //var currentUserId = ()HttpContext.User
                if (ModelState.IsValid)
                {
                    viewModel.CreatorUserId = _currentUser.CurrentUserId;
                    viewModel.CompanyName = _currentUser.CompanyName;
                    _projectService.Create(viewModel);

                    ModelState.Clear();

                    var newVm = new CreateProjectInput();
                    newVm.ErrorCode = ErrorCodeHelper.Ok;
                    newVm.ErrorDescription = "¡Proyecto guardado exitosamente!";
                    return PartialView("_createPartial", newVm);
                    //return Json(newVm, JsonRequestBehavior.AllowGet);
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
            UpdateProjectInput viewModel = new UpdateProjectInput();
            try
            {
                viewModel.CompanyName = _currentUser.CompanyName;
                viewModel = _projectService.GetEdit(id);
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
        public ActionResult Edit(UpdateProjectInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    viewModel.LastModifierUserId = _currentUser.CurrentUserId;
                    viewModel.CompanyName = _currentUser.CompanyName;
                    _projectService.Update(viewModel);
                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.ErrorDescription = "¡Proyecto guardado exitosamente!";
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
                _projectService.Delete(id,_currentUser.CurrentUserId);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }
    }
}