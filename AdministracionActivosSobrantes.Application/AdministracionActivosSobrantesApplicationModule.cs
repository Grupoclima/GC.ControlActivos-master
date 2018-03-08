using System.Reflection;
using Abp.Modules;
using AdministracionActivosSobrantes.Filters;

namespace AdministracionActivosSobrantes
{
    [DependsOn(typeof(AdministracionActivosSobrantesCoreModule))]
    public class AdministracionActivosSobrantesApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            //Configuration.UnitOfWork.RegisterFilter(FiltersNames.FilterCompany, false);
        }
    }
}
