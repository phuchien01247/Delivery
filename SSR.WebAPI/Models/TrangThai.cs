using System;
using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
	public class TrangThai : Audit, IIdEntity<string>
	{
		public TrangThai()
		{
		}
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string Color { get; set; }
		public int ThuTu { get; set; }
		public List<TrangThaiShort> NextTrangThai { get; set; }
	}

	public class TrangThaiShort
	{
        public string Id { get; set; }
		public string Code { get; set; }
		public string Ten { get; set; }
    }
}

