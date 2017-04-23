using System.Net.Http.Headers;
using System.Web;
using Lagerone.TokenAuth.Models;

namespace Lagerone.TokenAuth.Services
{
    public interface IAuthorizationService
    {
        AuthorizationResult UserIsAutorizedSync(HttpRequestHeaders httpRequestHeaders);
        AuthorizationResult UserIsAutorizedSync(HttpRequestBase httpRequest);
    }
}
