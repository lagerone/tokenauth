using System.Reflection;
using Autofac;

namespace Lagerone.TokenAuth
{
    public class TokenAuthModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsImplementedInterfaces();
        }
    }
}
