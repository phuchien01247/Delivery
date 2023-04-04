using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
    public class Room : Audit, TEntity<string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string GoogleMap { get; set; }
        public int Sort { get; set; }
        public bool IsShow { get; set; }
    }
}
