using System;
using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
	public class NhomChiTieu : Audit, IIdEntity<string>
	{
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
		public string Name { get; set; }
		public string GhiChu { get; set; }
		public bool Khoa { get; set; }
		public int ThuTu { get; set; }
	}

	public class NhomChiTieuShort
	{ 
		public string Id { get; set; }
		public string Ten { get; set; }
		public string GhiChu { get; set; }
		public bool Khoa { get; set; }
    }
}

