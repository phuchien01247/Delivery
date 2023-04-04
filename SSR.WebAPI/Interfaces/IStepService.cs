using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces
{
    public interface IStepService
    {
        Task<Step> Create(Step model);
        Task<Step> Update(Step model);
        Task Delete(string id);
        Task<Step> GetById(string id);
        Task<List<Step>> GetWithProjId(string id);
        Task<List<Step>> Get();
        Task<PagingModel<Step>> GetPaging(PagingParam param);
    }
}
