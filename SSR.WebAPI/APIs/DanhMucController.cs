using System;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace SSR.WebAPI.APIs
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DanhMucController : BaseAPIController<DanhMuc, string>
    {
        private readonly IDanhMucService _service;
        public DanhMucController(IDanhMucService service) : base(service)
        {
            this._service = service;
        }
    }
}

