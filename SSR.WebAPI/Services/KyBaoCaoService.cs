using System;
using SSR.WebAPI.Data;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;

namespace SSR.WebAPI.Services
{
	public class KyBaoCaoService : BaseAsyncRepository<KyBaoCao, string>, IKyBaoCaoService
    {
        public KyBaoCaoService(DataContext context, IHttpContextAccessor contextAccessor) :
            base(context, contextAccessor)
        {
        }
    }
}

