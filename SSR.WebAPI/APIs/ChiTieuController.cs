using System;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace SSR.WebAPI.APIs
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ChiTieuController : BaseAPIController<ChiTieu, string>
    {
        private readonly IChiTieuService _service;
        public ChiTieuController(IChiTieuService service) : base(service)
        {
            this._service = service;
        }
    }
}