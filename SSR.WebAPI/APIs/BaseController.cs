using System;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using Microsoft.AspNetCore.Mvc;
using EResultResponse = SSR.WebAPI.Helpers.EResultResponse;

namespace SSR.WebAPI.APIs
{
    public abstract class BaseAPIController<TEntity, UEntityId> : ControllerBase
       where TEntity : class, IIdEntity<UEntityId>
    {
        protected readonly IAsyncRepository<TEntity, UEntityId> Repository;

        public BaseAPIController(IAsyncRepository<TEntity, UEntityId> repository)
        {
            Repository = repository;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] TEntity model)
        {
            try
            {
                var response = await Repository.CreateAsync(model);
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
                    new ResultMessageResponse().IsError().WithCode(ex.ResultCode)
                        .WithMessage(ex.ResultString)
                );
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] TEntity model)
        {
            try
            {
                var response = await Repository.UpdateAsync(model);

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
                    new ResultMessageResponse().IsError().WithCode(ex.ResultCode)
                          .WithMessage(ex.ResultString)
                );
            }
        }

        [HttpPost]
        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(UEntityId id)
        {
            try
            {
                await Repository.DeleteWithIdAsync(id);

                return Ok(
                    new ResultMessageResponse()
                        .WithCode(EResultResponse.SUCCESS.ToString())
                        .WithMessage(DefaultMessage.DELETE_SUCCESS)
                );
            }
            catch (ResponseMessageException ex)
            {
                return Ok(
                    new ResultMessageResponse().IsError().WithCode(ex.ResultCode)
                          .WithMessage(ex.ResultString)
                );
            }
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromBody] TEntity model)
        {
            try
            {
                await Repository.DeleteAsync(model);

                return Ok(
                    new ResultMessageResponse()
                        .WithCode(EResultResponse.SUCCESS.ToString())
                        .WithMessage(DefaultMessage.DELETE_SUCCESS)
                );
            }
            catch (ResponseMessageException ex)
            {
                return Ok(
                    new ResultMessageResponse().IsError().WithCode(ex.ResultCode)
                          .WithMessage(ex.ResultString)
                );
            }
        }

        [HttpGet]
        [Route("get-by-id/{id}")]
        public async Task<IActionResult> GetById(UEntityId id)
        {
            try
            {
                var response = await Repository.GetByIdAsync(id);

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
                    new ResultMessageResponse().IsError().WithCode(ex.ResultCode)
                          .WithMessage(ex.ResultString)
                );
            }
        }

        [HttpPost]
        [Route("get-paging-params")]
        public async Task<IActionResult> GetPagingParam([FromBody] PagingParam param)
        {
            try
            {
                var response = await Repository.GetPagingAsync(param);

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
                    new ResultMessageResponse().IsError().WithCode(ex.ResultCode)
                          .WithMessage(ex.ResultString)
                );
            }
        }

        [HttpGet]
        [Route("get-all-data")]
        public async Task<IActionResult> GetAllData()
        {
            try
            {
                var response = await Repository.GetAsync();

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
                    new ResultMessageResponse().IsError().WithCode(ex.ResultCode)
                        .WithMessage(ex.ResultString)
                );
            }
        }

    }
}

