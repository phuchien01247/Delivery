using System;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace SSR.WebAPI.APIs
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ValuesController : BaseAPIController<Value, string>
    {
        private readonly IValueService _service;
        public ValuesController(IValueService service) : base(service)
        {
            this._service = service;
        }
    }
}