using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models;

public class Activities : Audit, TEntity<string>
{
    public Activities()
    {
    }
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    public string Action { get; set; }
    public string Content { get; set; }
    public string IssueId { get; set; }
    public string ProjectId { get; set; }
    public List<Label> Label { get; set; }
    public string Color { get; set; }
    public string Step { get; set; }
    public string Icon { get; set; }
}