using System.Threading.Tasks;
using Lagerone.TokenAuth.Models;
using Lagerone.TokenAuth.Providers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Lagerone.TokenAuth.Repositories
{
    internal class AuthenticationRequestRepository : IAuthenticationRequestRepository
    {
        private readonly IMongoCollection<AuthenticationRequest> _authenticationRequestCollection;

        public AuthenticationRequestRepository(ICollectionProvider collectionProvider)
        {
            _authenticationRequestCollection = collectionProvider.AuthenticationRequestCollection;
        }
        public async Task DeleteRequestsByEmail(string email)
        {
            var filter = Builders<AuthenticationRequest>.Filter.Regex(ar => ar.Email, 
                new BsonRegularExpression("/^" + email + "$/i"));
            await _authenticationRequestCollection.DeleteManyAsync(filter);
        }

        public async Task AddRequest(AuthenticationRequest authenticationRequest)
        {
            await _authenticationRequestCollection.InsertOneAsync(authenticationRequest);
        }

        public async Task<AuthenticationRequest> GetRequestByTokens(string cookieToken, string emailToken)
        {
            var builder = Builders<AuthenticationRequest>.Filter;
            var authRequestFilter = builder.Eq(ar => ar.EmailToken, emailToken) 
                & builder.Eq(ar => ar.CookieToken, cookieToken);
            return await _authenticationRequestCollection.Find(authRequestFilter).FirstOrDefaultAsync();
        }
    }
}