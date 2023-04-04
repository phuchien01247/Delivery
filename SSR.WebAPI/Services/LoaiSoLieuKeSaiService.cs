using System;
using SSR.WebAPI.Data;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;

namespace SSR.WebAPI.Services
{
	public class LoaiSoLieuKeSaiService : BaseAsyncRepository<LoaiSoLieuKeKhai, string>, ILoaiSoLieuKeKhaiService
    {
        public LoaiSoLieuKeSaiService(DataContext context, IHttpContextAccessor contextAccessor) :
            base(context, contextAccessor)
        {
        }
    }
}

