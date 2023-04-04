using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces
{
    public interface IGalleryService
    {
        Task<Gallery> Create(Gallery model);
        Task<Gallery> Update(Gallery model);
        Task Delete(string id);
        Task<Gallery> GetById(string id);
        Task<PagingModel<Gallery>> GetPaging(PagingParam param);
    }
}
