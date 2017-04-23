using System.Threading.Tasks;
using Lagerone.TokenAuth.Models;
using Lagerone.TokenAuth.Providers;
using MongoDB.Driver;

namespace Lagerone.TokenAuth.Repositories
{
    internal class AuthenticatedUserRepository : IAuthenticatedUserRepository
    {
        private readonly IMongoCollection<AuthenticatedUser> _authenticatedUserCollection;

        public AuthenticatedUserRepository(ICollectionProvider collectionProvider)
        {
            _authenticatedUserCollection = collectionProvider.AuthenticatedUserCollection;
        }

        public async Task Add(AuthenticatedUser authenticatedUser)
        {
            await _authenticatedUserCollection.InsertOneAsync(authenticatedUser);
        }

        public async Task Delete(string authenticationId)
        {
            var deleteFilter = Builders<AuthenticatedUser>.Filter.Eq(au => au.Id, authenticationId);
            await _authenticatedUserCollection.DeleteOneAsync(deleteFilter);
        }

        public void DeleteSync(string authenticationId)
        {
            var deleteFilter = Builders<AuthenticatedUser>.Filter.Eq(au => au.Id, authenticationId);
            _authenticatedUserCollection.DeleteOne(deleteFilter);
        }

        public async Task<AuthenticatedUser> GetById(string authenticationId)
        {
            return await _authenticatedUserCollection.Find(a => a.Id == authenticationId).FirstOrDefaultAsync();
        }

        public AuthenticatedUser GetByIdSync(string authenticationId)
        {
            return _authenticatedUserCollection.Find(a => a.Id == authenticationId).FirstOrDefault();
        }

        public async Task UpdateUserId(string authenticationId, string userId)
        {
            var updateFilter = Builders<AuthenticatedUser>.Update.Set(a => a.UserId, userId);
            await _authenticatedUserCollection.UpdateOneAsync(a => a.Id == authenticationId, updateFilter);
        }
    }
}