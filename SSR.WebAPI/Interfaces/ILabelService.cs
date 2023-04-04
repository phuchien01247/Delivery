using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces
{
    public interface ILabelService 
    {
        Task<Label> Create(Label model);
        Task<Label> Update(Label model);
        Task Delete(string id);
        Task<List<Label>> Get();
        Task<List<Label>> GetWithProjId(string id);
        Task<Label> GetById(string id); 
        Task<PagingModel<Label>> GetPaging(PagingParam param);
        Task<List<LabelTreeVM>> GetTree();
        Task<List<LabelTreeVM>> GetTreeWithProjId(string id);
        Task<List<LabelTreeVM>> GetFind(string key);
    }

}
