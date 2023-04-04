using System;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace SSR.WebAPI.APIs
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MauBieuController : BaseAPIController<MauBieu, string>
    {
        private readonly IMauBieuService _service;
        public MauBieuController(IMauBieuService service) : base(service)
        {
            this._service = service;
        }
    }
}