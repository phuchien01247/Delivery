using System;
using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
	public class KyBaoCao : Audit, IIdEntity<string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
		public string Name { get; set; }
		public DateTime NgayBatDau { get; set; }
		public DateTime NgayKetThuc { get; set; }
		public List<DonViSort> DonViThamGia { get; set; }
		public TrangThaiShort TrangThai { get; set; }
		public MauBieuShort MauBieu { get; set; }
		public int ThuTu { get; set; }
    }

	public class KyBaoCaoShort
	{ 
		public string Id { get; set; }
		public string Name { get; set; }
		public DateTime NgayBatDau { get; set; }
		public DateTime NgayKetThuc { get; set; }
		public List<DonViSort> DonViThamGia { get; set; }
		public TrangThaiShort TrangThai { get; set; }
		public MauBieuShort MauBieu { get; set; }
    }
}

