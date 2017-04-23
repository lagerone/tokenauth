using System.Threading.Tasks;
using Lagerone.TokenAuth.Models;

namespace Lagerone.TokenAuth.Repositories
{
    internal interface IAuthenticationRequestRepository
    {
        Task DeleteRequestsByEmail(string email);
        Task AddRequest(AuthenticationRequest authenticationRequest);
        Task<AuthenticationRequest> GetRequestByTokens(string cookieToken, string emailToken);
    }
}