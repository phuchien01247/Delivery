using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces
{
    public interface IPeopleAskService
    {
        Task<PeopleAsk> Create(PeopleAsk model);
        Task<PeopleAsk> Update(PeopleAsk model);
        Task Delete(string id);
        Task<PeopleAsk> GetById(string id);
        Task<List<PeopleAsk>> Get();
        Task<PagingModel<PeopleAsk>> GetPaging(PagingParam param);
    }
}
