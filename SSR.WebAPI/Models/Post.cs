using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models;

public class Post : Audit, TEntity<string>
{
    public Post()
    {
    }
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    public string Title { get; set; }
    public string Summary { get; set; }
    public string Content { get; set; }
    public string Thumbnail { get; set; }
    public bool Published { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string Slug { get; set; }
    
    public string Code { get; set; }
    
    public Status Status { get; set; }
    public Category Category { get; set; }
    public List<Tag> Tags { get; set; }

    public List<FileShort> Files { get; set; }
}