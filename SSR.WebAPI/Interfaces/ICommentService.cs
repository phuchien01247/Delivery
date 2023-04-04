using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces;

public interface ICommentService
{
    Task<Comment> Create(Comment model);
    Task<Comment> Update(Comment model);
    Task Delete(string id);
    Task<List<Comment>> Get();
    Task<Comment> GetById(string id);
    Task<List<Comment>> GetWithIssId(string id);
    Task<PagingModel<Comment>> GetPaging(CommentParams param);
}