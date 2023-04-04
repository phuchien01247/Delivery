using System;
using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
	public class ChiTieu : Audit, IIdEntity<string>
	{
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
		public string SoThuTu { get; set; }
		public string Name { get; set; }
		public float DiemToiDa { get; set; }
		public string CachXacDinhVaTinhDiem { get; set; }
		public LoaiSoLieuKeKhaiShort LoaiSoLieuKeKhai { get; set; }
        public dynamic SoLieuKeKhai { get; set; }

        public string ParentId { get; set; }
		public string GhiChu { get; set; }
		public string MauBieuId { get; set; }

		public NhomChiTieuShort NhomChiTieu { get; set; }
		public int ThuTu { get; set; }
	}
}

