using SSR.WebAPI.Params;
using SSR.WebAPIViewModels;

namespace SSR.WebAPI.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardVM> GetDashboard();
    }
}
