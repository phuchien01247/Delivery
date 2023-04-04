using System;
using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
	public class DanhMuc : Audit, IIdEntity<string>
	{
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
		public string Name { get; set; }
		public LoaiDanhMucShort LoaiDanhMuc { get; set; }
		public int ThuTu { get; set; }
	}
}

