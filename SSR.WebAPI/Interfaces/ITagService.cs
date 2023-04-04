using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces;

public interface ITagService
{
    Task<Tag> Create(Tag model);
    Task<Tag> Update(Tag model);
    Task Delete(string id);
    Task<Tag> GetById(string id);
    Task<List<Tag>> Get();
    Task<PagingModel<Tag>> GetPaging(PagingParam param);
}