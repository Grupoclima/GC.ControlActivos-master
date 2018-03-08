using System.Reflection;
using Abp.Modules;
using AdministracionActivosSobrantes;

namespace GcErpConnection
{
    [DependsOn(typeof(AdministracionActivosSobrantesCoreModule))]
    public class GcErpConnectionApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
