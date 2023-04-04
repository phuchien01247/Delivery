
using SSR.WebAPI.Models;
using SSR.WebAPI.ViewModels;

namespace SSR.WebAPI.Helpers 
{ 
  public class AuthResponse
{
    public UserLogin User { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public bool IsRequireVerify { get; set; }
    public bool IsRequireChangePassword { get; set; }

    public AuthResponse(UserLogin user, string token, string refreshToken, DateTime expiryDate)
    {
        Token = token;
        ExpiryDate = expiryDate.ToLocalTime();
        RefreshToken = refreshToken;
        User = user;
        UserId = user.Id;
        UserName = user.UserName;
        IsRequireVerify = user.IsRequireVerify;
        IsRequireChangePassword = user.IsRequireChangePassword;
    }

    public AuthResponse(UserLogin user)
    {
        this.User = user;
    }
}

public class UserLogin
{
    public UserLogin() { }
    public UserLogin(User model)
    {
        this.Id = model.Id;
        this.UserName = model.UserName;

        this.FullName = model.FullName;
        this.LastName = model.LastName;
        this.FirstName = model.FirstName;

        this.Phone = model.PhoneNumber;
        this.Email = model.Email;
        this.Permissions = model.Permissions;
        this.Roles = model.Role;
        this.Menu = model.Menu;
        this.Avatar = model.Avatar;
    }
    public string Id { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public IEnumerable<Role> Roles { get; set; }
    public IEnumerable<string> Permissions { get; set; }
    public List<FileShort> Avatar { get; set; }
    public bool IsRequireVerify { get; set; }
    public bool IsRequireChangePassword { get; set; }
    public List<NavMenuVM> Menu { get; set; }
}

public class TokenResult
{
    public string access_token { get; set; }
    public string scope { get; set; } = "am_application_scope default";
    public string token_type { get; set; } = "Bearer";
    public int expires_in { get; set; }
}
public class TokenResultFail
{
    public string error { get; set; } = "invalid_client";
    public string error_description { get; set; } = "A valid OAuth client could not be found.";
}

public class AuthResponsePaygov : AuthResponse
{
    public int expires_in { get; set; }

    public AuthResponsePaygov(UserLogin user, string token, string refreshToken, DateTime expiryDate) : base(user, token, refreshToken, expiryDate)
    {
    }

    public AuthResponsePaygov(UserLogin user) : base(user)
    {
    }
}
}