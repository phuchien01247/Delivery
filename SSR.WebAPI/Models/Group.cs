using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
    public class Group : Audit, TEntity<string>
    {
        public Group()
        {
        }
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        //public string CreatedBy { get; set; }
        public string Description { get; set;}
        public List<User> Members { get; set; }
        public string Name { get; set; }
        //public List<Role> Role { get; set; }
        //public bool IsDeleted { get; set; }
    }
}
  