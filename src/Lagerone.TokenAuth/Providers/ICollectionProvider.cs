using Lagerone.TokenAuth.Models;
using MongoDB.Driver;

namespace Lagerone.TokenAuth.Providers
{
    internal interface ICollectionProvider
    {
        IMongoCollection<AuthenticationRequest> AuthenticationRequestCollection { get; }
        IMongoCollection<AuthenticatedUser> AuthenticatedUserCollection { get; }
    }
}
