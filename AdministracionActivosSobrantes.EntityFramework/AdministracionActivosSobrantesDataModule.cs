using System.Data.Entity;
using System.Reflection;
using Abp.EntityFramework;
using Abp.Modules;
using AdministracionActivosSobrantes.EntityFramework;

namespace AdministracionActivosSobrantes
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(AdministracionActivosSobrantesCoreModule))]
    public class AdministracionActivosSobrantesDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            Database.SetInitializer<AdministracionActivosSobrantesDbContext>(null);
        }
    }
}
