using SSR.WebAPI.Data;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Services;
using Microsoft.Extensions.Options;
using SSR.WebAPI.Models;

namespace SSR.WebAPI.Installers;

public class DbInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DbSettings>(configuration.GetSection(nameof(DbSettings)));
        services.AddSingleton<IDbSettings>(sp => sp.GetRequiredService<IOptions<DbSettings>>().Value);
        services.AddSingleton<DataContext>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IModuleService, ModuleService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ILoggingService, LoggingService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IExportFileService, ExportFileService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IGalleryService, GalleryService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IPeopleAskService, PeopleAskService>();
        services.AddScoped<IVideoService, VideoService>();
        services.AddScoped<IChucVuService, ChucVuService>();
        services.AddScoped<IStatusService, StatusService>();
        services.AddScoped<IDashboardService, DashboardService>();

        // SSR
        services.AddScoped<IChiTieuService, ChiTieuService>();
        services.AddScoped<IDanhMucService, DanhMucService>();
        services.AddScoped<IDonViService, DonViService>();
        services.AddScoped<IKyBaoCaoService, KyBaoCaoService>();
        services.AddScoped<ILoaiDanhMucService, LoaiDanhMucService>();
        services.AddScoped<ILoaiSoLieuKeKhaiService, LoaiSoLieuKeSaiService>();
        services.AddScoped<IMauBieuService, MauBieuService>();
        services.AddScoped<INhomTieuChiService, NhomChiTieuService>();
        services.AddScoped<ITrangThaiService, TrangThaiService>();
        services.AddScoped<IValueService, ValueService>();
        services.AddScoped<IActivitiesService, ActivitiesService>();
        //services.AddScoped<IPhanloaiService, PhanloaiService>();
        services.AddScoped<IKnowledgeService, KnowledgeService>();
        services.AddScoped<ILabelService, LabelService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IStepService, StepService>();
        services.AddScoped<IIssueService, IssueService>();
        services.AddScoped<ICommentService, CommentService>();
    }
}