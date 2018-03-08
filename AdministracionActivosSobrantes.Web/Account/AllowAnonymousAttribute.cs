using System;

namespace AdministracionActivosSobrantes.Web.Account
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AllowAnonymousAttribute : Attribute
    {
    }
}