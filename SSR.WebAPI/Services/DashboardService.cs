using SSR.WebAPI.Data;
using SSR.WebAPI.Interfaces;
using SSR.WebAPIViewModels;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Services
{
    public class DashboardService : BaseService, IDashboardService
    {

        private DataContext _context;
        DateTime currentDate = DateTime.Now;
       

        public DashboardService(DataContext context,
            IHttpContextAccessor contextAccessor)
            : base(context, contextAccessor)
        {
            _context = context;
        }

        public async Task<DashboardVM> GetDashboard()
        {
            var ngayTao = DateTime.Now;
            var start = new DateTime(ngayTao.Year, ngayTao.Month, ngayTao.Day).AddTicks(-1);
            var end = start.AddDays(1).AddTicks(-1);


            DashboardVM vm = new DashboardVM();  
            var soProject = _context.Project.Find(x=> (x.IsDeleted != true) && ((x.CreatedBy == CurrentUserName) || x.Group.Any(c => c.Members.Any(b => b.UserName == CurrentUserName)) || x.Member.Any(b => b.UserName == CurrentUserName))).CountDocumentsAsync();
            var soLoiTrongNgay = _context.Issue.Find(x => (start < x.CreatedAt && x.CreatedAt <= end) && x.StepId == "Open" && (x.User.Any(b => b.UserName == CurrentUserName) || x.Group.Any(c => c.Members.Any(b => b.UserName == CurrentUserName)) || x.CreatedBy == CurrentUserName)).CountDocumentsAsync();
            var soLoiCanGiaiQuyet = _context.Issue.Find(x => x.StepId == "Open" && (x.User.Any(b => b.UserName == CurrentUserName) || x.Group.Any(c => c.Members.Any(b => b.UserName == CurrentUserName)) || x.CreatedBy == CurrentUserName)).CountDocumentsAsync();
            var soLoiDaGiaiQuyet = _context.Issue.Find(x => x.StepId == "Close" && (start < x.CreatedAt && x.CreatedAt <= end) && (x.User.Any(b => b.UserName == CurrentUserName) || x.Group.Any(c => c.Members.Any(b => b.UserName == CurrentUserName)) || x.CreatedBy == CurrentUserName)).CountDocumentsAsync();
            await Task.WhenAll(soProject, soLoiCanGiaiQuyet, soLoiTrongNgay, soLoiDaGiaiQuyet);
            vm.soProject = soProject.Result;
            vm.soLoiCanGiaiQuyet = soLoiCanGiaiQuyet.Result;
            vm.soLoiTrongNgay = soLoiTrongNgay.Result;
            vm.soLoiDaGiaiQuyet = soLoiDaGiaiQuyet.Result;
            return vm;
        }
    }
}
