using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models;

public class Audit
{
    public Audit()
    {
    }

    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; } = DateTime.Now;
    public string CreatedBy { get; set; }
    public string ModifiedBy { get; set; }

    [BsonDefaultValue(false)]
    public bool IsDeleted { get; set; }
    [BsonIgnore]
    public string CreatedAtShow
    {
        get { return CreatedAt.HasValue ? CreatedAt.Value.ToLocalTime().ToString(FormatDate) : ""; }
    }
    [BsonIgnore]
    public string LastModifiedShow
    {
        get { return ModifiedAt.HasValue ? ModifiedAt.Value.ToLocalTime().ToString(FormatDate) : ""; }
    }
    [BsonIgnore]
    public string FormatDate
    {
        get { return "hh:mm dd/MM/yyyy"; }
    }
}
