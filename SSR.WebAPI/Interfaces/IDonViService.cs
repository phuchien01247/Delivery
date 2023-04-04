using System;
using SSR.WebAPI.Models;

namespace SSR.WebAPI.Interfaces
{
	public interface IDonViService : IAsyncRepository<DonVi, string>
    {
        Task<List<DonViTreeVM>> GetTree();
    }
}