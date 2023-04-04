using System;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace SSR.WebAPI.APIs
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LoaiSoLieuKeKhaiController : BaseAPIController<LoaiSoLieuKeKhai, string>
    {
        private readonly ILoaiSoLieuKeKhaiService _service;
        public LoaiSoLieuKeKhaiController(ILoaiSoLieuKeKhaiService service) : base(service)
        {
            this._service = service;
        }
    }
}

