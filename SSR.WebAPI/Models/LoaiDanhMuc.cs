using System;
using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
	public class LoaiDanhMuc : Audit, IIdEntity<string>
	{
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
		public string Name { get; set; }
		public int ThuTu { get; set; }

	}

	public class LoaiDanhMucShort 
    { 
		public string Id { get; set; }
		public string Ten { get; set; }
    }
}