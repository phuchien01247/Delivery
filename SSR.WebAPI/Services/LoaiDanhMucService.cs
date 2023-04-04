using System;
using SSR.WebAPI.Data;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
namespace SSR.WebAPI.Services
{
	public class LoaiDanhMucService : BaseAsyncRepository<LoaiDanhMuc, string>, ILoaiDanhMucService
    {
        public LoaiDanhMucService(DataContext context, IHttpContextAccessor contextAccessor) :
            base(context, contextAccessor)
        {
        }
	}
}