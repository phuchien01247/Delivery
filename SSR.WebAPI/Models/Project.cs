using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models;

public class Project : Audit, TEntity<string>
{
    public Project()
    {
    }
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public string NameNUni { get; set; }
    public List<Label> Label { get; set; }
    public List<Group> Group { get; set; } 
    public List<User> Member { get; set; }
    public string Slug { get; set; }
    public List<FileShort> Files { get; set; }
    

}