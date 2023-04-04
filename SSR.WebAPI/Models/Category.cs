using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models;

public class Category : Audit, TEntity<string>
{
    public Category()
    {
    }
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Summary { get; set; }
    public string Thumbnail { get; set; }
    public string Icon { get; set; }
    public int Level { get; set; }
    public int Sort { get; set; }
}