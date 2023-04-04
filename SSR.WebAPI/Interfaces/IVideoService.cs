using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces
{
    public interface IVideoService
    {
        Task<Video> Create(Video model);
        Task<Video> Update(Video model);
        Task Delete(string id);
        Task<Video> GetById(string id);
        Task<List<Video>> Get();
        Task<PagingModel<Video>> GetPaging(PagingParam param);
    }
}
