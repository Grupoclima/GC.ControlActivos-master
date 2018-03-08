using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using AdministracionActivosSobrantes.Users;
using AdministracionActivosSobrantes.Web.Account;
using AdministracionActivosSobrantes.Web.Models.Account;
using GcErpConnection;

namespace AdministracionActivosSobrantes.Web.Controllers
{
    public class AccountController : AdministracionActivosSobrantesControllerBase
    {
        private readonly IUserAppService _userService;
        private readonly ICurrentUser _currentUser;
        private readonly IGcErpConnectionService _erpConnection;
        string _user = ConfigurationManager.AppSettings["DbUser"];
        string _password = ConfigurationManager.AppSettings["DbPassword"];

        // GET: Account
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "OutRequests");
            AccountViewModel model = new AccountViewModel();

            _erpConnection.ConnectErp(_user, _password, "");
            model.Companies = _erpConnection.GetErpCompanies();
            _erpConnection.DisconnectErp();

            model.ErrorCode = 0;
            model.ErrorDescription = "";
            return View(model);
        }

        public AccountController(IUserAppService userService, ICurrentUser currentUser, IGcErpConnectionService erpConnection)
        {
            _userService = userService;
            _currentUser = currentUser;
            _erpConnection = erpConnection;
        }

        //[Throttle(Name = "TestThrottle", Message = "Necesitas esperar {n} segundos para accesar esta dirección nuevamente", Seconds = 5)]
        [HttpPost]
        public ActionResult Login(AccountViewModel model, string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "OutRequests");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    bool isPersistent = model.RememberMe;
                    var userInfo = _userService.Get(model.UserName, model.CompanyName);
                    bool userErpExactus = _erpConnection.CheckUserDatabase(model.UserName, model.Password, model.CompanyName);
                    //  serializeModel.Department = userInfo.Department;
                  //  model.Department = userInfo.Department;

                    if (userInfo.UserName.ToLower().Equals(model.UserName.ToLower()) && userErpExactus)
                    {
                        var serializeModel = new CustomPrincipalSerializeModel();
                        serializeModel.Department = userInfo.Department;

                        serializeModel = new CustomPrincipalSerializeModel
                        {
                            UserId = userInfo.Id,
                            CompleteName = userInfo.UserName,
                            UserName = userInfo.CompleteName,
                            Rol = userInfo.Rol,
                            CompanyName = userInfo.CompanyName,
                        };
                        var serializer = new JavaScriptSerializer();
                 
                        string userData = serializer.Serialize(serializeModel);
                        var authTicket = new FormsAuthenticationTicket(
                            1,
                            model.UserName,
                            DateTime.UtcNow,
                            DateTime.UtcNow.AddHours(3),
                            isPersistent,
                            userData);
                        //FormsAuthentication.SetAuthCookie(userexample.UserName, true);
                        string encTicket = FormsAuthentication.Encrypt(authTicket);
                        var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                        Response.Cookies.Add(faCookie);

                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        //await SignInAsync(userData, model.RememberMe);
                        return RedirectToAction("Index", "OutRequests");
                    }
                    model.ErrorCode = -1;
                    model.Password = "";
                    _erpConnection.ConnectErp(_user, _password, "");
                    model.Companies = _erpConnection.GetErpCompanies();
                    _erpConnection.DisconnectErp();
                    model.ErrorDescription = "El usuario o contraseña son incorrectos";
                    ModelState.AddModelError("", "El usuario o contraseña son incorrectos.");
                }
                catch (Exception)
                {

                    model.ErrorCode = -1;
                    model.Password = "";
                    _erpConnection.ConnectErp(_user, _password, "");
                    model.Companies = _erpConnection.GetErpCompanies();
                    _erpConnection.DisconnectErp();
                    model.ErrorDescription = "El usuario o contraseña son incorrectos";
                    ModelState.AddModelError("", "El usuario o contraseña son incorrectos.");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("Index", model);
            }
            // If we got this far, something failed, redisplay form
            return View("Index", model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Account");
        }

        //private async Task SignInAsync(User user, ClaimsIdentity identity = null, bool rememberMe = false)
        //{
        //    if (identity == null)
        //    {
        //        identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        //    }

        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        //    AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = rememberMe },identity);
        //}

    }
}