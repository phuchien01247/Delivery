using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
    public class Employee : Audit, TEntity<string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsShow { get; set; }
        public ChucVuShort ChucVu { get; set; }
        public int Sort { get; set; }
    }
}
