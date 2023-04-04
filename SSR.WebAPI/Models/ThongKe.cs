using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models;

public class ThongKe : Audit, TEntity<string>
{
    public ThongKe()
    {
    }
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    public string NameIssue { get; set; }
    public string Donvi { get; set; }
}