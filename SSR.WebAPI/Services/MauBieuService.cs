using System;
using SSR.WebAPI.Data;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;

namespace SSR.WebAPI.Services
{
	public class MauBieuService : BaseAsyncRepository<MauBieu, string>, IMauBieuService
    {
        public MauBieuService(DataContext context, IHttpContextAccessor contextAccessor) :
            base(context, contextAccessor)
        {
        }
    }
}

