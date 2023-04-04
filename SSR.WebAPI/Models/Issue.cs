using MongoDB.Bson.Serialization.Attributes;
namespace SSR.WebAPI.Models
{
    public class Issue : Audit, TEntity<string>
    {
        public Issue()
        {
        }
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public string TitleNU { get; set; }
        public List<Group> Group { get; set; }
        public List<User> User { get; set; }
        public string Description { get; set; }
        public string DonVi { get; set; }
        public string projectId { get; set; }
        public DateTime DueDate { get; set; }
        public List<Label> Label { get; set; }
        public List<FileShort> Files { get; set; }
        public string StepId { get; set; }
    }
}
