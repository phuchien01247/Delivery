using System;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace SSR.WebAPI.APIs
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TrangThaiController : BaseAPIController<TrangThai, string>
    {
        private readonly ITrangThaiService _service;
        public TrangThaiController(ITrangThaiService service) : base(service)
        {
            this._service = service;
        }
    }
}
