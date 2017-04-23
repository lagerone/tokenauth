using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Lagerone.TokenAuth.Models
{
    internal class MongoEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}