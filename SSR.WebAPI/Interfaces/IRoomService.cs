using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces
{
    public interface IRoomService
    {
        Task<Room> Create(Room model);
        Task<Room> Update(Room model);
        Task Delete(string id);
        Task<List<Room>> Get();
        Task<Room> GetById(string id);
        Task<PagingModel<Room>> GetPaging(PagingParam param);
    }
}
