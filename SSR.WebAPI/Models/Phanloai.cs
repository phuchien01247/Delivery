using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models;

    public class Phanloai : Audit, TEntity<string>
    {
        public Phanloai()
        {
        }
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public bool IsGlobal { get; set; }
        public string ParentId { get; set; }
        public string IdProject { get; set; }
        public List<Knowledge> Knowledge { get; set; }
    }

