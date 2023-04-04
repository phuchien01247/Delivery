using System;
namespace SSR.WebAPI.Models
{
    public interface IIdEntity<T>
    {
        T Id { get; set; }
        DateTime? CreatedAt { get; set; }
        DateTime? ModifiedAt { get; set; }
        string CreatedBy { get; set; }
        string ModifiedBy { get; set; }
        bool IsDeleted { get; set; }
        string Name { get; set; }
        
    }
}