using System.Reflection;
using Abp.Modules;
using AdministracionActivosSobrantes;

namespace GCMvcMailer
{
    [DependsOn(typeof(AdministracionActivosSobrantesCoreModule))]
    public class GcMailNotificationApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
