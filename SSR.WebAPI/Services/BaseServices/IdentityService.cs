using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using SSR.WebAPI.Authorization;
using SSR.WebAPI.Data;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Interfaces.BaseInterfaces;
using SSR.WebAPI.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using PAKN.WebAPI.Extensions;
using PAKN.WebAPI.Services;
using EResultResponse = SSR.WebAPI.Exceptions.EResultResponse;


namespace SSR.WebAPI.Services;

public class IdentityService : BaseService, IIdentityService
{
    private readonly IUserService _userService;
    private readonly IMenuService _menuService;
    private readonly IRoleService _roleService;
    private readonly JwtSettings _jwtSettings;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly IJwtUtils _jwtUtils;
    private readonly ILoggingService _logger;
    private readonly IDbSettings _settings;
    private DataContext _context;

    public IdentityService(
        ILoggingService logger,
        IMenuService menuService,
        IDbSettings settings,
        DataContext context,
    IUserService userService,
        IRoleService roleService,
        JwtSettings jwtSettings,
        TokenValidationParameters tokenValidationParameters,
        IJwtUtils jwtUtils,
        IHttpContextAccessor httpContext
    ) : base(context, httpContext)
    {
        _context = context;
        _userService = userService;
        _roleService = roleService;
        _jwtSettings = jwtSettings;
        _jwtUtils = jwtUtils;
        _tokenValidationParameters = tokenValidationParameters;
        _settings = settings;
        _menuService = menuService;
    }
    
    public async Task<AuthResponse> RegisterAsync(User userModel)
    {
        var createdUser = await _userService.Create(userModel);

        return await GenerateAuthenticationResultForUserAsync(createdUser);
    }

    public async Task<AuthResponse> LoginAsync(AuthRequest model)
    {
        var user = await Authenticate(model);

        return await GenerateAuthenticationResultForUserAsync(user);
    }

    
    public async Task<User> Authenticate(AuthRequest model)
    {
        var user = await _context.Users.Find(x =>
            (x.UserName == model.UserName || x.PhoneNumber == model.UserName || x.Email == model.UserName)).FirstOrDefaultAsync();
        if (user == null)
            throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                .WithMessage("Không tìm thấy tài khoản.");

        if (!PasswordExtensions.VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
            throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                .WithMessage("Mật khẩu không chính xác.");
        // var jwtToken = _jwtUtils.GenerateJwtToken(user);
        var permissions = await _roleService.GetPermissionForCurrentUer(user.UserName);
        var menu = await _roleService.GetMenuForUser(user.UserName);
        var menuItems = await _menuService.Get();
        user.Permissions = permissions;
        user.Menu = menu;
        user.MenuItems = menuItems;
        // var userModel = new UserLogin(user.Id, user.UserName, user.FullName, user.PhoneNumber, user.Email,
        //     permissions, user.Roles, menu, user.Avatar);
        return user;
    }

    private async Task<AuthResponse> GenerateAuthenticationResultForUserAsync(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                new Claim("id", user.Id)
            };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var refreshToken = new RefreshToken
        {
            JwtId = token.Id,
            UserId = user.Id,
            Token = tokenHandler.WriteToken(token),
            CreationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.Add(_jwtSettings.TokenRefreshStore)
        };

        // var createdRefreshToken = await _refreshTokenService.Create(refreshToken);

        var userLogin = new UserLogin(user);

        return new AuthResponse(userLogin,
            tokenHandler.WriteToken(token),
            refreshToken.JwtId,
            tokenDescriptor.Expires.HasValue ? tokenDescriptor.Expires.Value : refreshToken.ExpiryDate
        );
    }

    private ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
            if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
    {
        return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
               jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                   StringComparison.InvariantCultureIgnoreCase);
    }
}