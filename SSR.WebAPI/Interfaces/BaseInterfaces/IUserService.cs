using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using SSR.WebAPI.ViewModels;
using SSR.WebAPI.Params;
using SSR.WebAPI.ViewModels;

namespace SSR.WebAPI.Interfaces;

public interface IUserService
{
    Task<User> Create(User model);
    Task<User> Update(User model);
    Task Delete(string id);
    Task<IEnumerable<User>> Get();
    Task<User> GetById(string id);
    Task<PagingModel<User>> GetPaging(PagingParam param);

    //Task<PagingModel<User>> GetPagingHandler(PagingParam param);
    Task<User> GetByUserName(string userName);
    Task<User> ChangePassword(UserVM model);
    Task<User> ChangeProfile(User model);
}