using System;
using SSR.WebAPI.Extensions;
using MongoDB.Bson.Serialization.Attributes;

namespace SSR.WebAPI.Models
{
	public class DonVi : Audit, IIdEntity<string>, IEntity<DonVi>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }

        public string ParentId { get; set; }
        public int CapDV { get; set; }
    }

	public class DonViSort {
        public string Id { get; set; }
        public string Ten { get; set; }
        public string ParentId { get; set; }
        public int CapDV { get; set; }
    }

	public class DonViTreeVM : ITreeVM<DonViTreeVM>
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public bool Selected { get; set; }
        public bool Opened { get; set; }
        public List<DonViTreeVM> Children { get; set; }
    }

   
}

