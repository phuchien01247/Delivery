using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using SSR.WebAPI.ViewModels;

namespace SSR.WebAPI.Interfaces;

public interface IModuleService
{
    Task<Module> Create(Module model);
    Task<Module> Update(Module model);
    Task Delete(string id);
    Task<IEnumerable<Module>> Get();
    Task<Module> GetById(string id);
    Task<Module> AddPermissionToModule(Permission model);
    Task<Permission> GetPermissionById(Permission model);
    Task DeletePermission(Permission model);

    Task<PagingModel<Module>> GetPaging(PagingParam param);
    Task<List<ModuleTreeVM>> GetTreeModule();
}