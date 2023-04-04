using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;

using Microsoft.AspNetCore.Mvc;
using EResultResponse = SSR.WebAPI.Exceptions.EResultResponse;
using SSR.WebAPI.Params;
using SSR.WebAPI.Services;

namespace DTI.WebAPI.APIs;

[Route("api/v1/[controller]")]
//[ApiController]
public class GroupController : ControllerBase
{

    private IGroupService _service;

    public GroupController(IGroupService service)
    {
        _service = service;
    }

    [HttpPost]
    [Route("get-paging-params")]
    public async Task<IActionResult> GetPagingParam([FromBody] PagingParam param)
    {
        try
        {
            var data = await _service.GetPaging(param);

            return Ok(
                new ResultResponse<PagingModel<Group>>()
                    .WithData(data)
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
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromBody] Group model)
    {
        try
        {
            var response = await _service.Create(model);
            return Ok(
                new ResultResponse<dynamic>()
                    .WithData(response)
                    .WithCode(EResultResponse.SUCCESS.ToString())
                    .WithMessage(DefaultMessage.CREATE_SUCCESS)
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

    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> Update([FromBody] Group model)
    {
        try
        {
            var response = await _service.Update(model);

            return Ok(
                new ResultResponse<dynamic>()
                    .WithData(response)
                    .WithCode(EResultResponse.SUCCESS.ToString())
                    .WithMessage(DefaultMessage.UPDATE_SUCCESS)
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

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _service.Delete(id);

            return Ok(
                new ResultMessageResponse()
                    .WithCode(EResultResponse.SUCCESS.ToString())
                    .WithMessage(DefaultMessage.DELETE_SUCCESS)
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
    [Route("get-by-id/{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var response = await _service.GetById(id);

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
    [Route("get")]
    public async Task<IActionResult> Get()
    {
        try
        {
            var response = await _service.Get();

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