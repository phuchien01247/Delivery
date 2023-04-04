// using System;
// using System.Threading.Tasks;
// using SSR.WebAPIExceptions;
// using SSR.WebAPIHelpers;
// using SSR.WebAPIInterfaces;
// using SSR.WebAPIModels;
// using SSR.WebAPIParams;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using EResultResponse = SSR.WebAPIHelpers.EResultResponse;
//
// namespace SSR.WebAPIControllers
// {
//     [Route("api/v1/[controller]")]
//     [Authorize]
//     public class NotifyController : ControllerBase
//     {
//         private readonly INotifyService _notifyService;
//
//         public NotifyController(INotifyService notifyService)
//         {
//             this._notifyService = notifyService;
//         }
//         
//         // [HttpPost]
//         // [Route("get-paging-params")]
//         // public async Task<IActionResult> GetPagingParam([FromBody] NotifyParam param)
//         // {
//         //     ResultResponse<PagingModel<User>> resultResponse = new ResultResponse<PagingModel<User>>();
//         //     try
//         //     {
//         //         var data = await  _notifyService.GetPaging(param);
//         //         return Ok(data);
//         //     }
//         //     catch (Exception ex)
//         //     {
//         //         resultResponse.ResultCode = EResultResponse.ERROR.ToString();
//         //         resultResponse.ResultString = "Lỗi." + ex.Message;
//         //         return Ok(resultResponse);
//         //     }
//         // }
//         [HttpPost]
//         [Route("get-paging-params")]
//         public async Task<IActionResult> GetPagingParam([FromBody] NotifyParam param)
//         {
//             try
//             {
//                 var response = await _notifyService.GetPaging(param);
//
//                 return Ok(
//                     new ResultResponse<dynamic>()
//                         .WithData(response)
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
//         [HttpGet]
//         [Route("get-by-id/{id}")]
//         public async Task<IActionResult> GetById(string id)
//         {
//             ResultResponse<CoQuan> resultResponse = new ResultResponse<CoQuan>();
//             try
//             {
//                 if (id == default)
//                 {
//                     resultResponse.ResultCode = EResultResponse.FAIL.ToString();
//                     resultResponse.ResultString = "Dữ liệu không được trống.";
//                     return Ok(resultResponse);
//                 }
//
//                 var data = await _notifyService.GetById(id);
//                 return Ok(data);
//             }
//             catch (Exception ex)
//             {
//                 resultResponse.ResultCode = EResultResponse.ERROR.ToString();
//                 resultResponse.ResultString = "Lỗi." + ex.Message;
//                 return Ok(resultResponse);
//             }
//         }
//
//         [HttpGet]
//         [Route("get-list-notify")]
//         public async Task<IActionResult> GetListNotify()
//         {
//             ResultResponse<Notify> resultResponse = new ResultResponse<Notify>();
//             try
//             {
//                 var data = await _notifyService.GetListNotify();
//                 return Ok(data);
//             }
//             catch (Exception ex)
//             {
//                 resultResponse.ResultCode = EResultResponse.ERROR.ToString();
//                 resultResponse.ResultString = "Lỗi." + ex.Message;
//                 return Ok(resultResponse);
//             }
//         }
//         
//         [HttpPost]
//         [Route("change-status")]
//         public async Task<IActionResult> ChangeStatus([FromBody] NotifyParam model)
//         {
//             try
//             {
//                 var data = await _notifyService.ChangeStatus(model.Id);
//                 return Ok(data);
//             }
//             catch (Exception ex)
//             {
//                 return Ok(
//                     new ResultResponse<String>()
//                     {
//                         ResultCode = EResultResponse.ERROR.ToString(),
//                         ResultString = "Lỗi",
//                         Data = $"{ex.Message}"
//                     }
//                 );
//                 
//             }
//         }
//         
//         
//         [HttpGet]
//         [Route("luu-cong-van-noi-bo/{id}")]
//         public async Task<IActionResult> LuuCVNoiBo(string id)
//         {
//             ResultResponse<CoQuan> resultResponse = new ResultResponse<CoQuan>();
//             try
//             {
//                 if (id == default)
//                 {
//                     resultResponse.ResultCode = EResultResponse.FAIL.ToString();
//                     resultResponse.ResultString = "Dữ liệu không được trống.";
//                     return Ok(resultResponse);
//                 }
//
//                 await _notifyService.LuuCVNoiBo(id);
//                 return Ok(
//                     new ResultMessageResponse().WithCode(EResultResponse.SUCCESS.ToString())
//                         .WithMessage("Lưu công văn thành công!")
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
//         [HttpGet]
//         [Route("luu-cong-van-di-noi-bo/{id}")]
//         public async Task<IActionResult> LuuCVDiNoiBo(string id)
//         {
//             ResultResponse<CoQuan> resultResponse = new ResultResponse<CoQuan>();
//             try
//             {
//                 if (id == default)
//                 {
//                     resultResponse.ResultCode = EResultResponse.FAIL.ToString();
//                     resultResponse.ResultString = "Dữ liệu không được trống.";
//                     return Ok(resultResponse);
//                 }
//
//                 await _notifyService.LuuCVDiNoiBo(id);
//                 return Ok(
//                     new ResultMessageResponse().WithCode(EResultResponse.SUCCESS.ToString())
//                         .WithMessage("Lưu công văn thành công!")
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
//     }
// }