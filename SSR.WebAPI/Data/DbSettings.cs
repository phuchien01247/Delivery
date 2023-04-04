namespace SSR.WebAPI.Data;

public class DbSettings : IDbSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string LoggingCollectionName { get; set; }
    public string UserCollectionName { get; set; }
    public string RoleCollectionName { get; set; }
    public string ModuleCollectionName { get; set; }
    public string MenuCollectionName { get; set; }
    public string FileCollectionName { get; set; }
    public string RefreshTokenCollectionName { get; set; }
    public string GroupCollectionName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string TagsCollectionName { get; set; }
    public string CategoriesCollectionName { get; set; }
    public string PostsCollectionName { get; set; }
    public string EmployeesCollectionName { get; set; }
    public string GalleriesCollectionName { get; set; }
    public string RoomsCollectionName { get; set; }
    public string PeopleAsksCollectionName { get; set; }
    public string VideosCollectionName { get; set; }
    public string ChucVuCollectionName { get; set; }
    public string StatusCollectionName { get; set; }
    public string LoaiDanhMucCollectionName { get; set; }
    public string DanhMucCollectionName { get; set; }
    public string LoaiSoLieuKeKhaiCollectionName { get; set; }
    public string ChiTieuCollectionName { get; set; }
    public string NhomChiTieuCollectionName { get; set; }
    public string MauBieuCollectionName { get; set; }
    public string KyBaoCaoCollectionName { get; set; }
    public string KyBaoCaoValueCollectionName { get; set; }
    public string ValueCollectionName { get; set; }
    public string DonViCollectionName { get; set; }
    public string TrangThaiCollectionName { get; set; }
    public string ActivitiesCollectionName { get; set; }
    public string ExportCollectionName { get; set; }
}

public interface IDbSettings
{
    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
    string LoggingCollectionName { get; set; }
    string UserCollectionName { get; set; }
    string RoleCollectionName { get; set; }
    string ModuleCollectionName { get; set; }
    string MenuCollectionName { get; set; }
    string FileCollectionName { get; set; }
    string RefreshTokenCollectionName { get; set; }
    string ActivitiesCollectionName { get; set; }

    string TagsCollectionName { get; set; }
    string CategoriesCollectionName { get; set; }
    string PostsCollectionName { get; set; }
    string EmployeesCollectionName { get; set; }
    string GalleriesCollectionName { get; set; }
    string RoomsCollectionName { get; set; }
    string PeopleAsksCollectionName { get; set; }
    string VideosCollectionName { get; set; }
    string ChucVuCollectionName { get; set; }
    string StatusCollectionName { get; set; }

    string LoaiDanhMucCollectionName { get; set; }
    string DanhMucCollectionName { get; set; }
    string LoaiSoLieuKeKhaiCollectionName { get; set; }
    string ChiTieuCollectionName { get; set; }
    string NhomChiTieuCollectionName { get; set; }
    string MauBieuCollectionName { get; set; }
    string KyBaoCaoCollectionName { get; set; }
    string KyBaoCaoValueCollectionName { get; set; }
    string ValueCollectionName { get; set; }
    string DonViCollectionName { get; set; }
    string TrangThaiCollectionName { get; set; }
    string GroupCollectionName { get; set; }
    string ExportCollectionName { get; set; }
}