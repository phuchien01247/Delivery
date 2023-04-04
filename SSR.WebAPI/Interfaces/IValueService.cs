using System;
using SSR.WebAPI.Models;

namespace SSR.WebAPI.Interfaces
{
    public interface IValueService : IAsyncRepository<Value, string>
    {

    }
}