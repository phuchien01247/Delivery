using System;
using SSR.WebAPI.Data;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;

namespace SSR.WebAPI.Services
{
	public class NhomChiTieuService : BaseAsyncRepository<NhomChiTieu, string>, INhomTieuChiService
    {
        public NhomChiTieuService(DataContext context, IHttpContextAccessor contextAccessor) :
            base(context, contextAccessor)
        {
        }
    }
}

