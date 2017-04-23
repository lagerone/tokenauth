using System.Threading.Tasks;
using System.Web;
using Lagerone.TokenAuth.Models;

namespace Lagerone.TokenAuth.Services
{
    public interface ITokenService
    {
        Task<RequestTokenResult> CreateRequestToken(string email, HttpResponseBase response);
    }
}