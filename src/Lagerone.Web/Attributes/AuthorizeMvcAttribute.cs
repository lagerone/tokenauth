using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Lagerone.TokenAuth.Models;
using Lagerone.TokenAuth.Services;

namespace Lagerone.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, 
        Inherited = true, 
        AllowMultiple = false)]
    public class AuthorizeMvcAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorizationService = DependencyResolver.Current.GetService<IAuthorizationService>();
            var authResult = authorizationService.UserIsAutorizedSync(httpContext.Request);
            return authResult.AuthorizationStatus.Equals(AuthorizationStatus.Success);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { controller = "Authentication", action = "Index" })
            );
        }
    }
}
