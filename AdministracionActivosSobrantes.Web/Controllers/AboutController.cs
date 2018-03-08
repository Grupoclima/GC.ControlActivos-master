using System.Web.Mvc;

namespace AdministracionActivosSobrantes.Web.Controllers
{
    public class AboutController : AdministracionActivosSobrantesControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}