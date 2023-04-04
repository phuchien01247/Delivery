using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces
{
    public interface IEmployeeService
    {
        Task<Employee> Create(Employee model);
        Task<Employee> Update(Employee model);
        Task Delete(string id);
        Task<Employee> GetById(string id);
        Task<PagingModel<Employee>> GetPaging(PagingParam param);
    }
}
