using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces;

public interface IProjectService
{
    Task<Project> Create(Project model);
    Task<Project> Update(Project model);
    Task Delete(string id);
    Task<List<Project>> Get();
    Task<Project> GetById(string id);
    Task<Project> GetBySlug(string slug);
    Task<PagingModel<Project>> GetPaging(ProjectParams param);
}