using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models;

public class Status : Audit, TEntity<string>
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int Sort { get; set; }
}