using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models;

public class Knowledge: Audit, TEntity<string>
    {
        public Knowledge()
        {
        }
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
    }

