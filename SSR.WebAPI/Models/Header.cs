using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
    public class Header : Audit
    {
        public Header()
        {
        }
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string IdDV { get; set; }
        public string NameDV { get; set; }

    }
}
