using MongoDB.Bson.Serialization.Attributes;
namespace SSR.WebAPI.Models;

public class File : Audit, TEntity<string>
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }

    public string FileName { get; set; }
    public string SaveName { get; set; }
    public string Path { get; set; }
    public long Size { get; set; }
    public string Ext { get; set; }
    public string Thumbnail { get; set; }
}

public class FileShort
{
    public string FileId { get; set; }
    public string FileName { get; set; }

    public string Ext { get; set; }
    // Link video for Thumbnail
    public string Path { get; set; }
}
