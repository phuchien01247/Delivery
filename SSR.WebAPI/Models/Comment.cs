using MongoDB.Bson.Serialization.Attributes;
namespace SSR.WebAPI.Models
{
    public class Comment : Audit, TEntity<string>
    {
        public Comment()
        {
        }
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Content { get; set; }
        public List<FileShort> Files { get; set; }
        public string issueId { get; set; }
        public string parentId { get; set; }
    }
}
