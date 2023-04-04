using System;
using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
	public class KyBaoCaoValue : Audit, IIdEntity<string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
		public string KyBaoCaoId { get; set; }
		public string DonVi { get; set; }
		public TrangThaiShort TrangThai { get; set; }
		public int ThuTu { get; set; }
		[BsonIgnore]
		public string Name { get; set; }
	}
}

