using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces.BaseInterfaces;
using SSR.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SSR.WebAPI.APIs.Identity
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
  
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AuthRequest user)
        {
            try
            {
                var response = await _identityService.LoginAsync(user);

                return Ok(
                    new ResultResponse<AuthResponse>()
                        .WithData(response)
                        .WithCode(Exceptions.EResultResponse.SUCCESS.ToString())
                        .WithMessage("Đăng nhập thành công")
                );
            }
            catch (ResponseMessageException ex)
            {
                return Ok(
                    new ResultMessageResponse().WithCode(ex.ResultCode)
                        .WithMessage(ex.ResultString)
                );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}