using System.Threading.Tasks;
using Lagerone.TokenAuth.Models;

namespace Lagerone.TokenAuth.Repositories
{
    internal interface IAuthenticatedUserRepository
    {
        Task Add(AuthenticatedUser authenticatedUser);
        Task Delete(string authenticationId);
        void DeleteSync(string authenticationId);
        Task<AuthenticatedUser> GetById(string authenticationId);
        AuthenticatedUser GetByIdSync(string authenticationId);
        Task UpdateUserId(string authenticationId, string userId);
    }
}