using SSR.WebAPI.Data;
using SSR.WebAPI.Models;
using MongoDB.Driver;

namespace SSR.WebAPI.Services;

public abstract class BaseService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly DataContext _context;
    private HttpContext _httpContext { get { return _contextAccessor.HttpContext; } }
    private User _appUser;
    protected User CurrentUser => GetCurrentUser();

    protected string CurrentUserName => GetCurrentUser()?.UserName;
    protected string CurrentUserFullName => GetCurrentUser()?.FullName;
    protected string IdUserName => GetCurrentUser()?.Id;

    public BaseService(DataContext context,
        IHttpContextAccessor contextAccessor)
    {
        this._context = context;
        this._contextAccessor = contextAccessor;
    }

    private User GetCurrentUser()
    {
        if (_appUser != null)
            return _appUser;

        var userId = _httpContext.User?.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
        if (userId != default)
            _appUser = _context.Users.FindAsync(x => x.Id == userId).Result.FirstOrDefault();
        return _appUser;
    }
}