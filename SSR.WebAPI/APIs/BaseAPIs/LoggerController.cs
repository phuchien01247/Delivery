// using System.Threading.Tasks;
// using SSR.WebAPI.Exceptions;
// using SSR.WebAPI.Helpers;
// using SSR.WebAPI.Interfaces;
// using SSR.WebAPI.Models;
// using SSR.WebAPI.Params;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace SSR.WebAPI.APIs.Identity
// {
//     [Route("api/v1/[controller]")]
//     [Authorize]
//     public class LoggerController : ControllerBase
//     {
//         private ILoggingService _logger;
//         public LoggerController(ILoggingService logger)
//         {
//             _logger = logger;
//         }
//         
//                 [HttpGet]
//         [Route("get-by-id/{id}")]
//         public async Task<IActionResult> GetById(string id)
//         {
//             try
//             {
//                 var data = await _logger.GetById(id);
//
//                 return Ok(
//                     new ResultResponse<Logging>()
//                         .WithData(data)
//                         .WithCode(EResultResponse.SUCCESS.ToString())
//                         .WithMessage(DefaultMessage.GET_DATA_SUCCESS)
//                 );
//             }
//             catch (ResponseMessageException ex)
//             {
//                 return Ok(
//                     new ResultMessageResponse().WithCode(ex.ResultCode)
//                         .WithMessage(ex.ResultString)
//                 );
//             }
//         }
//
//         [HttpPost]
//         [Route("get-paging-params")]
//         public async Task<IActionResult> GetPagingParam([FromBody] PagingParam param)
//         {
//             try
//             {
//                 var data = await _logger.GetPaging(param);
//
//                 return Ok(
//                     new ResultResponse<PagingModel<Logging>>()
//                         .WithData(data)
//                         .WithCode(EResultResponse.SUCCESS.ToString())
//                         .WithMessage(DefaultMessage.GET_DATA_SUCCESS)
//                 );
//             }
//             catch (ResponseMessageException ex)
//             {
//                 return Ok(
//                     new ResultMessageResponse().WithCode(ex.ResultCode)
//                         .WithMessage(ex.ResultString)
//                 );
//             }
//         }
//
//     }
// }