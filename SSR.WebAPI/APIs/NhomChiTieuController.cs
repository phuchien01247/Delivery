using System;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace SSR.WebAPI.APIs
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NhomChiTieuController : BaseAPIController<NhomChiTieu, string>
    {
        private readonly INhomTieuChiService _service;
        public NhomChiTieuController(INhomTieuChiService service) : base(service)
        {
            this._service = service;
        }
    }
}
