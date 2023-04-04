using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces;

public interface IGroupService
{
    Task<Group> Create(Group model);
    Task<Group> Update(Group model);
    Task Delete(string id);
    Task<List<Group>> Get();
    Task<Group> GetById(string id);
    Task<PagingModel<Group>> GetPaging(PagingParam param);
}