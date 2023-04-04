using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces
{
    public interface IChucVuService
    {
        Task<ChucVu> Create(ChucVu model);
        Task<ChucVu> Update(ChucVu model);
        Task Delete(string id);
        Task<List<ChucVu>> Get();
        Task<List<ChucVu>> GetAll();
        Task<ChucVu> GetById(string id);
        Task<PagingModel<ChucVu>> GetPaging(PagingParam param);
    }
}
