using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Abp.Localization;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using Abp.Web.Mvc;
using AdministracionActivosSobrantes.Web.Account;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace AdministracionActivosSobrantes.Web
{
    [DependsOn(
        typeof(AbpWebMvcModule),
        typeof(AdministracionActivosSobrantesDataModule), 
        typeof(AdministracionActivosSobrantesApplicationModule), 
        typeof(AdministracionActivosSobrantesWebApiModule))]
    public class AdministracionActivosSobrantesWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Add/remove languages for your application
            Configuration.Localization.Languages.Add(new LanguageInfo("en", "English", "famfamfam-flag-england", true));
            Configuration.Localization.Languages.Add(new LanguageInfo("tr", "Türkçe", "famfamfam-flag-tr"));
            Configuration.Localization.Languages.Add(new LanguageInfo("zh-CN", "简体中文", "famfamfam-flag-cn"));

            //Add/remove localization sources here
            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    AdministracionActivosSobrantesConsts.LocalizationSourceName,
                    new XmlFileLocalizationDictionaryProvider(
                        HttpContext.Current.Server.MapPath("~/Localization/AdministracionActivosSobrantes")
                        )
                    )
                );

            //Configure navigation/menu
            Configuration.Navigation.Providers.Add<AdministracionActivosSobrantesNavigationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }

    //public class MyInstaller : IWindsorInstaller
    //{
    //    public void Install(IWindsorContainer container, IConfigurationStore store)
    //    {
    //        container.Register(Classes.FromThisAssembly().BasedOn<ICurrentUser>().LifestylePerThread().WithServiceSelf());
    //    }
    //}
}
