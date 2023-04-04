using MongoDB.Bson.Serialization.Attributes;
using SSR.WebAPI.Extensions;

namespace SSR.WebAPI.Models
{
    public class Label : Audit, TEntity<string>, IEntity<Label>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string NameNU { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public bool IsGlobal { get; set; }
        public string ParentId { get; set; }
        public string idProject { get; set; }
        public List<Knowledge> Knowledge { get; set; }
    }

    public class LabelShort
    {
        public string Id { get; set; }
        public string Ten { get; set; }
        public string ParentId { get; set; }
        public int CapDV { get; set; }
    }

    public class LabelTreeVM : ITreeVM<LabelTreeVM>
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public bool Selected { get; set; }
        public bool Opened { get; set; }
        public List<LabelTreeVM> Children { get; set; }
    }
}
