using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces
{
    public interface IKnowledgeService
    {
        Task<Knowledge> Create(Knowledge model);
        Task<Knowledge> Update(Knowledge model);
        Task Delete(string id);
        Task<List<Knowledge>> Get();
        Task<Knowledge> GetById(string id);
        Task<PagingModel<Knowledge>> GetPaging(PagingParam param);
    }
}
