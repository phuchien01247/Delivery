using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces;

    public interface IPhanloaiService
    {
        Task<Phanloai> Create(Phanloai model);
        Task<Phanloai> Update(Phanloai model);
        Task Delete(string id);
        Task<List<Phanloai>> Get();
        Task<Phanloai> GetById(string id);
        Task<PagingModel<Phanloai>> GetPaging(PagingParam param);
    }

