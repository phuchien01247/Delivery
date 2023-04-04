using System;
using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
	public class Value : Audit, IIdEntity<string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
		public dynamic SoLieuKeKhai { get; set; }
		
		
		// Ghi chú của tệp tin
		public string GhiChu { get; set; }

		public List<FileShort> TepTinMinhChung { get; set; }
		public double DiemTuDanhGia { get; set; }
		public double DiemDaDuyet { get; set; }
		public string MauBieuId { get; set; }
		public string ChiTieuId { get; set; }
		public string KyBaoCaoValueId { get; set; }
		public int ThuTu { get; set; }
		[BsonIgnore]
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}

