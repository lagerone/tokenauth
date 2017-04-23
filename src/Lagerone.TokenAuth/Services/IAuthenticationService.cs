using System.Threading.Tasks;
using System.Web;
using Lagerone.TokenAuth.Models;

namespace Lagerone.TokenAuth.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> AuthenticateUser(HttpRequestBase httpRequest, HttpResponseBase httpResponse, string emailToken);
        Task DeleteAuthenticatedUser(HttpRequestBase httpRequest, HttpResponseBase httpResponse);
        Task SetAuthenticatedUserId(HttpRequestBase httpRequest, string userId);
    }
}