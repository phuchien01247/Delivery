using System;
using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
	public class MauBieu : Audit, IIdEntity<string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
		public string Name { get; set; }
		public bool DangSuDung { get; set; }
		public int ThuTu { get; set; }
	}

	public class MauBieuShort
	{
		public string Id { get; set; }
		public string Ten { get; set; }
    }
}

