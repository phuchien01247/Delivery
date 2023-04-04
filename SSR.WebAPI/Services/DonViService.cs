using System;
using SSR.WebAPI.Data;
using SSR.WebAPI.Extensions;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using MongoDB.Driver;

namespace SSR.WebAPI.Services
{
	public class DonViService : BaseAsyncRepository<DonVi, string>, IDonViService
    {
        public DonViService(DataContext context, IHttpContextAccessor contextAccessor) :
            base(context, contextAccessor)
        {
        }

        
        public async Task<List<DonViTreeVM>> GetTree() 
	    {
            var donVis = await _context.DonVi.Find(x => x.IsDeleted != true).SortBy(donVi => donVi.CapDV).ToListAsync();
            var data = MethodExtensions.GetTree<DonViTreeVM, DonVi>(donVis?? new List<DonVi>());

            return data;
	    }

        
    }
}

