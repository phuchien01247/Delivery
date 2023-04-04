using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces;

public interface IIssueService
{
    Task<Issue> Create(Issue model);
    Task<Issue> Update(Issue model);
    Task Delete(string id);
    Task<List<Issue>> Get();
    Task<Issue> GetById(string id);
    Task<List<Issue>> GetWithProjId(string id);
    Task<List<Issue>> GetOpenWithProjId(string id);
    Task<List<Issue>> GetCloseWithProjId(string id);
    Task<PagingModel<Issue>> GetPaging(PagingParam param);
    Task<PagingModel<Issue>> GetPagingLoiTrongNgay(PagingParam param);
    Task<PagingModel<Issue>> GetPagingLoiNgay(PagingParam param);
    Task<PagingModel<Issue>> GetPagingLoiDaGiaiQuyet(PagingParam param);
}