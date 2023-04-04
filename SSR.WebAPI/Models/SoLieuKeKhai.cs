using System;
using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{	
   	public class LoaiSoLieuKeKhai : Audit, IIdEntity<string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		/// <summary>
		/// 1. Tính phần trăm : double
		/// 2. Float
		/// 3. Integer
		/// 4. Combobox - danh mục
		/// 5. String
		/// </summary>
		public dynamic SoLieuKeKhai { get; set; }
		public int ThuTu { get; set; }
    }

	public class LoaiSoLieuKeKhaiShort
	{ 
		public string Id { get; set; }
		public string Code { get; set; }
		public string Ten { get; set; }
    }
}

