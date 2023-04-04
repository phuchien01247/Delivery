using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces;

public interface IStatusService
{
    Task<Status> Create(Status model);
    Task<Status> Update(Status model);
    Task Delete(string id);
    Task<Status> GetById(string id);
    Task<List<Status>> Get();
    Task<PagingModel<Status>> GetPaging(PagingParam param);
}