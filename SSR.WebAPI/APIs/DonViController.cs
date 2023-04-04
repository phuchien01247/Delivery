using SSR.WebAPI.Data;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace SSR.WebAPI.APIs
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DonViController : BaseAPIController<DonVi, string>
    {
        private readonly IDonViService _service;
        private DataContext _context;
        public DonViController(IDonViService service, DataContext context) : base(service)
        {
            this._service = service;
            this._context = context;
        }

        [HttpGet]
        [Route("get-tree")]
        public async Task<IActionResult> GetTree()
        {
            try
            {
                var response = await _service.GetTree();

                return Ok(
                    new ResultResponse<dynamic>()
                        .WithData(response)
                        .WithCode(Exceptions.EResultResponse.SUCCESS.ToString())
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

        //[HttpGet]
        //[Route("sync")]
        //public async Task<IActionResult> Sync()
        //{
        //    try
        //    {
        //        var data = _context.CoQuan.Find(x => x.IsDeleted != true).ToList();
        //        foreach (var item in data)
        //        {
        //           await _context.DonVi.InsertOneAsync(new DonVi() { Id = item.Id, MaDonVi = item.MaCoQuan, Ten = item.Ten, DonViCha = item.DonViCha, CapDV = item.CapDV });
        //        }
        //        var response = "";

        //        return Ok(
        //            new ResultResponse<dynamic>()
        //                .WithData(response)
        //                .WithCode(Exceptions.EResultResponse.SUCCESS.ToString())
        //                .WithMessage(DefaultMessage.GET_DATA_SUCCESS)
        //        );
        //    }
        //    catch (ResponseMessageException ex)
        //    {
        //        return Ok(
        //            new ResultMessageResponse().IsError().WithCode(ex.ResultCode)
        //                  .WithMessage(ex.ResultString)
        //        );
        //    }
        //}
    }
}