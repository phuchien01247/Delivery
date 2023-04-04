using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
    public class ExportFile : Audit
    {
        public ExportFile() 
        {
        }
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        //public string Id { get; set; }
        public string IdLB { get; set; }
        public string NameLB { get; set; }
        public List<long> Values { get; set; }
        public int Sort { get; set; }
    }
}
