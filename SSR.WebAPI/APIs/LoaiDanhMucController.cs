using System;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace SSR.WebAPI.APIs
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LoaiDanhMucController : BaseAPIController<LoaiDanhMuc, string>
    {
        private readonly ILoaiDanhMucService _service;
        public LoaiDanhMucController(ILoaiDanhMucService service) : base(service)
        {
            this._service = service;
        }
    }
}