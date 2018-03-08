using Abp.Web.Mvc.Views;

namespace AdministracionActivosSobrantes.Web.Views
{
    public abstract class AdministracionActivosSobrantesWebViewPageBase : AdministracionActivosSobrantesWebViewPageBase<dynamic>
    {

    }

    public abstract class AdministracionActivosSobrantesWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        //protected AdministracionActivosSobrantesWebViewPageBase()
        //{
        //    LocalizationSourceName = AdministracionActivosSobrantesConsts.LocalizationSourceName;
        //}
    }
}