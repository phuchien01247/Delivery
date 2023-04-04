using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using SSR.WebAPI.ViewModels;

namespace SSR.WebAPI.Interfaces;

public interface IMenuService
{
    Task<Menu> Create(Menu model);
    Task<Menu> Update(Menu model);
    Task Delete(string id);
    Task<IEnumerable<Menu>> Get();
    Task<Menu> GetById(string id);
    Task<PagingModel<Menu>> GetPaging(PagingParam param);
    Task<List<MenuTreeVM>> GetTree();
    Task<List<MenuTreeVM>> GetTreeList();
    Task<List<NavMenuVM>> GetTreeListMenuForCurrentUser(List<Menu> listMenu);
}