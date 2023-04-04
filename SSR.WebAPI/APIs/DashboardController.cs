using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EResultResponse = SSR.WebAPI.Helpers.EResultResponse;

namespace SSR.WebAPI.APIs
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [Route("getDashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var response = await _dashboardService.GetDashboard();
                return Ok(
                    new ResultResponse<dynamic>()
                        .WithData(response)
                        .WithCode(EResultResponse.SUCCESS.ToString())
                        .WithMessage("Lấy dữ liệu thành công")
                );
            }
            catch (ResponseMessageException ex)
            {
                return Ok(
                    new ResultMessageResponse().WithCode(ex.ResultCode)
                        .WithMessage(ex.ResultString)
                );
            }
            catch (Exception  ex)
            {
                return Ok(
                    new ResultMessageResponse().WithCode(EResultResponse.FAIL.ToString())
                        .WithMessage(ex.Message)
                );
            }
        }
    }
}