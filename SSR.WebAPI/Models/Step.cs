using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
    public class Step : Audit, TEntity<string>
    {
        public Step()
        {
        }
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string projectId { get; set; }
        public bool Block { get; set; }
    }
}
