using Lagerone.TokenAuth.Models;
using Lagerone.TokenAuth.Settings;
using MongoDB.Driver;

namespace Lagerone.TokenAuth.Providers
{
    internal class CollectionProvider : ICollectionProvider
    {
        private readonly IMongoDatabase _database;

        public CollectionProvider(IDatabaseSettings dbSettings)
        {
            var mongoUrl = new MongoUrl(dbSettings.ConnectionString);
            _database = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);
        }
        public IMongoCollection<AuthenticationRequest> AuthenticationRequestCollection => 
            _database.GetCollection<AuthenticationRequest>("AuthenticationRequest");

        public IMongoCollection<AuthenticatedUser> AuthenticatedUserCollection =>
            _database.GetCollection<AuthenticatedUser>("AuthenticatedUser");
    }
}