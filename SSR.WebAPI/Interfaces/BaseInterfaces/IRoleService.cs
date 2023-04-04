using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using SSR.WebAPI.ViewModels;

namespace SSR.WebAPI.Interfaces;

public interface IRoleService
{
    Task<Role> Create(Role model);
    Task<Role> Update(Role model);
    Task Delete(string id);
    Task<IEnumerable<Role>> Get();
    Task<Role> GetById(string id);
    Task<PagingModel<Role>> GetPaging(PagingParam param);
    Task<List<NavMenuVM>> GetMenuForUser(string userName);
    Task<List<string>> GetPermissionForCurrentUer(string userName);
}