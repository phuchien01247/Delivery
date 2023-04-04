using System;
using SSR.WebAPI.Models;

namespace SSR.WebAPI.Interfaces
{
    public interface IDanhMucService : IAsyncRepository<DanhMuc, string>
    {
    }
}

