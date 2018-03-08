using Abp.Web.Mvc.Controllers;

namespace AdministracionActivosSobrantes.Web.Controllers
{
    /// <summary>
    /// Derive all Controllers from this class.
    /// </summary>
    public abstract class AdministracionActivosSobrantesControllerBase : AbpController
    {
        protected AdministracionActivosSobrantesControllerBase()
        {
            LocalizationSourceName = AdministracionActivosSobrantesConsts.LocalizationSourceName;
        }
    }
}