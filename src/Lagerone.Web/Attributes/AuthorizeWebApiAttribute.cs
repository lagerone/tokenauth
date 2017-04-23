using System;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Lagerone.TokenAuth.Models;
using Lagerone.TokenAuth.Services;

namespace Lagerone.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
        Inherited = true,
        AllowMultiple = false)]
    public class AuthorizeWebApiAttribute : System.Web.Http.AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var authorizationService = DependencyResolver.Current.GetService<IAuthorizationService>();
            var authResult = authorizationService.UserIsAutorizedSync(actionContext.Request.Headers);
            return authResult.AuthorizationStatus.Equals(AuthorizationStatus.Success);
        }
    }
}
