using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models;

public class Tag : Audit, TEntity<string>
{
    public Tag()
    {
    }
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Color { get; set; }
    public int Level { get; set; }
    public int Sort { get; set; }
}