using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using Microsoft.AspNetCore.Mvc;
using EResultResponse = SSR.WebAPI.Exceptions.EResultResponse;

namespace SSR.WebAPI.APIs
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivitiesService _ActivitiesService;

        public ActivitiesController(IActivitiesService ActivitiesService)
        {
            _ActivitiesService = ActivitiesService;
        }

        //[HttpPost]
        //[Route("create")]
        //public async Task<IActionResult> Create([FromBody] Activities model)
        //{
        //    try
        //    {
        //        var response = await _ActivitiesService.Create(model);
        //        return Ok(
        //            new ResultResponse<dynamic>()
        //                .WithData(response)
        //                .WithCode(EResultResponse.SUCCESS.ToString())
        //                .WithMessage(DefaultMessage.CREATE_SUCCESS)
        //        );
        //    }
        //    catch (ResponseMessageException ex)
        //    {
        //        return Ok(
        //            new ResultMessageResponse().WithCode(ex.ResultCode)
        //                .WithMessage(ex.ResultString)
        //        );
        //    }
        //}

        //[HttpPost]
        //[Route("update")]
        //public async Task<IActionResult> Update([FromBody] Activities model)
        //{
        //    try
        //    {
        //        var response = await _ActivitiesService.Update(model);

        //        return Ok(
        //            new ResultResponse<dynamic>()
        //                .WithData(response)
        //                .WithCode(EResultResponse.SUCCESS.ToString())
        //                .WithMessage(DefaultMessage.UPDATE_SUCCESS)
        //        );
        //    }
        //    catch (ResponseMessageException ex)
        //    {
        //        return Ok(
        //            new ResultMessageResponse().WithCode(ex.ResultCode)
        //                .WithMessage(ex.ResultString)
        //        );
        //    }
        //}

        //[HttpPost]
        //[Route("delete/{id}")]
        //public async Task<IActionResult> Delete(string id)
        //{
        //    try
        //    {
        //        await _ActivitiesService.Delete(id);

        //        return Ok(
        //            new ResultMessageResponse()
        //                .WithCode(EResultResponse.SUCCESS.ToString())
        //                .WithMessage(DefaultMessage.DELETE_SUCCESS)
        //        );
        //    }
        //    catch (ResponseMessageException ex)
        //    {
        //        return Ok(
        //            new ResultMessageResponse().WithCode(ex.ResultCode)
        //                .WithMessage(ex.ResultString)
        //        );
        //    }
        //}

        [HttpGet]
        [Route("get-by-project-id/{id}")]
        public async Task<IActionResult> GetByProjectId(string id)
        {
            try
            {
                var response = await _ActivitiesService.GetByProjectId(id);

                return Ok(
                    new ResultResponse<dynamic>()
                        .WithData(response)
                        .WithCode(EResultResponse.SUCCESS.ToString())
                        .WithMessage(DefaultMessage.GET_DATA_SUCCESS)
                );
            }
            catch (ResponseMessageException ex)
            {
                return Ok(
                    new ResultMessageResponse().WithCode(ex.ResultCode)
                        .WithMessage(ex.ResultString)
                );
            }
        }
        [HttpGet]
        [Route("get-by-issue-id/{id}")]
        public async Task<IActionResult> GetByIssueId(string id)
        {
            try
            {
                var response = await _ActivitiesService.GetByIssueId(id);

                return Ok(
                    new ResultResponse<dynamic>()
                        .WithData(response)
                        .WithCode(EResultResponse.SUCCESS.ToString())
                        .WithMessage(DefaultMessage.GET_DATA_SUCCESS)
                );
            }
            catch (ResponseMessageException ex)
            {
                return Ok(
                    new ResultMessageResponse().WithCode(ex.ResultCode)
                        .WithMessage(ex.ResultString)
                );
            }
        }

        //[HttpGet]
        //[Route("get")]
        //public async Task<IActionResult> Get()
        //{
        //    try
        //    {
        //        var response = await _ActivitiesService.Get();

        //        return Ok(
        //            new ResultResponse<dynamic>()
        //                .WithData(response)
        //                .WithCode(EResultResponse.SUCCESS.ToString())
        //                .WithMessage(DefaultMessage.GET_DATA_SUCCESS)
        //        );
        //    }
        //    catch (ResponseMessageException ex)
        //    {
        //        return Ok(
        //            new ResultMessageResponse().WithCode(ex.ResultCode)
        //                .WithMessage(ex.ResultString)
        //        );
        //    }
        //}

        [HttpPost]
        [Route("get-paging-params")]
        public async Task<IActionResult> GetPagingParam([FromBody] ActivitiesParams param)
        {
            try
            {
                var response = await _ActivitiesService.GetPaging(param);

                return Ok(
                    new ResultResponse<dynamic>()
                        .WithData(response)
                        .WithCode(EResultResponse.SUCCESS.ToString())
                        .WithMessage(DefaultMessage.GET_DATA_SUCCESS)
                );
            }
            catch (ResponseMessageException ex)
            {
                return Ok(
                    new ResultMessageResponse().WithCode(ex.ResultCode)
                        .WithMessage(ex.ResultString)
                );
            }
        }
    }
}
