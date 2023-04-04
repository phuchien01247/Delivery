using System;
using SSR.WebAPI.Data;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;

namespace SSR.WebAPI.Services
{
	public class DanhMucService : BaseAsyncRepository<DanhMuc, string>, IDanhMucService
    {
        public DanhMucService(DataContext context, IHttpContextAccessor contextAccessor) :
            base(context, contextAccessor)
        {
        }
    }
}

