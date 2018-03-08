using System.Reflection;
using Abp.Modules;

namespace AdministracionActivosSobrantes
{
    public class AdministracionActivosSobrantesCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
