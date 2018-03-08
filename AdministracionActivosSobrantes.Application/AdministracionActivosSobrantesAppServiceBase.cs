using Abp.Application.Services;

namespace AdministracionActivosSobrantes
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class AdministracionActivosSobrantesAppServiceBase : ApplicationService
    {
        protected AdministracionActivosSobrantesAppServiceBase()
        {
            LocalizationSourceName = AdministracionActivosSobrantesConsts.LocalizationSourceName;
        }
    }
}