using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces;

public interface IPostService
{
    Task<Post> Create(Post model);
    Task<Post> Update(Post model);
    Task Delete(string id);
    Task<List<Post>> Get();
    Task<Post> GetById(string id);
    Task<Post> GetBySlug(string slug);
    Task<PagingModel<Post>> GetPaging(PostParams param);
}