using System.Reflection;
using Abp.Application.Services;
using Abp.Modules;
using Abp.WebApi;
using Abp.WebApi.Controllers.Dynamic.Builders;

namespace AdministracionActivosSobrantes
{
    [DependsOn(typeof(AbpWebApiModule), typeof(AdministracionActivosSobrantesApplicationModule))]
    public class AdministracionActivosSobrantesWebApiModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            DynamicApiControllerBuilder
                .ForAll<IApplicationService>(typeof(AdministracionActivosSobrantesApplicationModule).Assembly, "app")
                .Build();
        }
    }
}
