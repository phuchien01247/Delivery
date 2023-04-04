using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces;

public interface ICategoryService
{
    Task<Category> Create(Category model);
    Task<Category> Update(Category model);
    Task Delete(string id);
    Task<Category> GetById(string id);
    Task<List<Category>> Get();
    Task<PagingModel<Category>> GetPaging(PagingParam param);
}