using SSR.WebAPI.Models;
using MongoDB.Driver;
using File = SSR.WebAPI.Models.File;
using Tag = SSR.WebAPI.Models.Tag;
using SSR.WebAPI.Interfaces;

namespace SSR.WebAPI.Data;

public class DataContext
{
    private readonly IMongoClient _mongoClient = null;
    private readonly IMongoDatabase _context = null;
    private readonly IMongoCollection<User> _users;
    private readonly IMongoCollection<Role> _roles;
    private readonly IMongoCollection<Menu> _menu;
    private readonly IMongoCollection<Module> _module;
    private readonly IMongoCollection<File> _file;
    private readonly IMongoCollection<Logging> _logging;
    private readonly IMongoCollection<RefreshToken> _refreshToken;
    
    // Nghiệp vụ
    private readonly IMongoCollection<Tag> _tag;
    private readonly IMongoCollection<Category> _category;
    private readonly IMongoCollection<Post> _post;

    private readonly IMongoCollection<Video> _video;
    private readonly IMongoCollection<Room> _room;
    private readonly IMongoCollection<Employee> _employee;
    private readonly IMongoCollection<Gallery> _gallery;
    private readonly IMongoCollection<PeopleAsk> _peopleAsk;
    private readonly IMongoCollection<ChucVu> _chucVu;
    private readonly IMongoCollection<Status> _status;

    // SSR

    private readonly IMongoCollection<LoaiDanhMuc> _loaiDanhMuc;
    private readonly IMongoCollection<DanhMuc> _danhMuc;
    private readonly IMongoCollection<LoaiSoLieuKeKhai> _loaiSoLieuKeKhai;
    private readonly IMongoCollection<ChiTieu> _chiTieu;
    private readonly IMongoCollection<NhomChiTieu> _nhomChiTieu;
    private readonly IMongoCollection<MauBieu> _mauBieu;
    private readonly IMongoCollection<KyBaoCao> _kyBaoCao;
    private readonly IMongoCollection<KyBaoCaoValue> _kyBaoCaoValue;
    private readonly IMongoCollection<Value> _value;
    private readonly IMongoCollection<DonVi> _donVi;
    private readonly IMongoCollection<TrangThai> _trangThai;
   // private readonly IMongoCollection<CoQuan> _coQuan;

    private readonly IMongoCollection<Knowledge> _knowledge;
    private readonly IMongoCollection<Group> _group;
    private readonly IDbSettings _settings;
    private readonly IMongoCollection<Activities> _activities;
    private readonly IMongoCollection<Label> _label;
    private readonly IMongoCollection<Project> _project;
    private readonly IMongoCollection<Step> _step;
    private readonly IMongoCollection<Issue> _issue;
    private readonly IMongoCollection<Comment> _comment;
    private readonly IMongoCollection<ExportFile> _export;
    private readonly IMongoCollection<DonViSort> _donViSort;

    public DataContext(IDbSettings settings)
    {
        _settings = settings;
        var client = new MongoClient(_settings.ConnectionString);

        if (client != null)
        {
            _context = client.GetDatabase(_settings.DatabaseName);
        }
        
        _users = _context.GetCollection<User>(_settings.UserCollectionName);
        _roles = _context.GetCollection<Role>(_settings.RoleCollectionName);
        _menu = _context.GetCollection<Menu>(_settings.MenuCollectionName);
        _module = _context.GetCollection<Module>(_settings.ModuleCollectionName);
        _file = _context.GetCollection<File>(_settings.FileCollectionName);
        _logging = _context.GetCollection<Logging>(_settings.LoggingCollectionName);
        _refreshToken = _context.GetCollection<RefreshToken>(_settings.RefreshTokenCollectionName);

        // Nghiệp vụ
        _tag = _context.GetCollection<Tag>(_settings.TagsCollectionName);
        _category = _context.GetCollection<Category>(_settings.CategoriesCollectionName);
        _post = _context.GetCollection<Post>(_settings.PostsCollectionName);

        _video = _context.GetCollection<Video>(_settings.VideosCollectionName);
        _gallery = _context.GetCollection<Gallery>(_settings.GalleriesCollectionName);
        _peopleAsk = _context.GetCollection<PeopleAsk>(_settings.PeopleAsksCollectionName);
        _room = _context.GetCollection<Room>(_settings.RoomsCollectionName);
        _employee = _context.GetCollection<Employee>(_settings.EmployeesCollectionName);
        _chucVu = _context.GetCollection<ChucVu>(_settings.ChucVuCollectionName);
        _status = _context.GetCollection<Status>(_settings.StatusCollectionName);

        // SSR
        _loaiDanhMuc = _context.GetCollection<LoaiDanhMuc>(_settings.LoaiDanhMucCollectionName);
        _danhMuc = _context.GetCollection<DanhMuc>(_settings.DanhMucCollectionName);
        _loaiSoLieuKeKhai = _context.GetCollection<LoaiSoLieuKeKhai>(_settings.LoaiSoLieuKeKhaiCollectionName);
        _chiTieu = _context.GetCollection<ChiTieu>(_settings.ChiTieuCollectionName);
        _nhomChiTieu = _context.GetCollection<NhomChiTieu>(_settings.NhomChiTieuCollectionName);
        _mauBieu = _context.GetCollection<MauBieu>(_settings.MauBieuCollectionName);
        _kyBaoCao = _context.GetCollection<KyBaoCao>(_settings.KyBaoCaoCollectionName);
        _kyBaoCaoValue = _context.GetCollection<KyBaoCaoValue>(_settings.KyBaoCaoValueCollectionName);
        _value = _context.GetCollection<Value>(_settings.ValueCollectionName);
        _donVi = _context.GetCollection<DonVi>(_settings.DonViCollectionName);
        _trangThai = _context.GetCollection<TrangThai>(_settings.TrangThaiCollectionName);
        //_coQuan = _context.GetCollection<CoQuan>("CoQuan");
        
        _knowledge = _context.GetCollection<Knowledge>("Knowledge");
        _group = _context.GetCollection<Group>(_settings.GroupCollectionName);
        _label = _context.GetCollection<Label>("Label");
        _project = _context.GetCollection<Project>("Project");
        _step = _context.GetCollection<Step>("Step");
        _issue = _context.GetCollection<Issue>("Issue");
        _activities = _context.GetCollection<Activities>("Activity");
        _comment = _context.GetCollection<Comment>("Comment");
        _export = _context.GetCollection<ExportFile>("Export");
        _donViSort = _context.GetCollection<DonViSort>("DonViSort");
    }

    public IMongoDatabase Database
    {
        get { return _context; }
    }
    public IMongoClient Client
    {
        get { return _mongoClient; }
    }
    public IMongoCollection<User> Users { get => _users; }
    public IMongoCollection<Role> Role { get => _roles; }
    public IMongoCollection<Menu> Menu { get => _menu; }
    public IMongoCollection<File> Files { get => _file; }
    public IMongoCollection<Module> Modules { get => _module; }
    public IMongoCollection<Logging> Loggings { get => _logging; }
    public IMongoCollection<RefreshToken> RefreshToken { get => _refreshToken; }

    // Nghiệp vụ
    public IMongoCollection<Tag> Tags { get => _tag; }
    public IMongoCollection<Category> Categories { get => _category; }
    public IMongoCollection<Post> Posts { get => _post; }

    public IMongoCollection<Video> Video { get => _video; }
    public IMongoCollection<Gallery> Gallery { get => _gallery; }
    public IMongoCollection<Room> Room { get => _room; }
    public IMongoCollection<PeopleAsk> PeopleAsk { get => _peopleAsk; }
    public IMongoCollection<Employee> Employee { get => _employee; }
    public IMongoCollection<ChucVu> ChucVu { get => _chucVu; }
    public IMongoCollection<Status> Status { get => _status; }

    // SSR
    public IMongoCollection<LoaiDanhMuc> LoaiDanhMuc { get => _loaiDanhMuc; }
    public IMongoCollection<DanhMuc> DanhMuc { get => _danhMuc; }
    public IMongoCollection<ExportFile> ExportFile { get => _export; }
    public IMongoCollection<LoaiSoLieuKeKhai> LoaiSoLieuKeKhai { get => _loaiSoLieuKeKhai; }

    public IMongoCollection<ChiTieu> ChiTieu { get => _chiTieu; }

    public IMongoCollection<NhomChiTieu> NhomChiTieu { get => _nhomChiTieu; }
    public IMongoCollection<Activities> Activities { get => _activities; }
    public IMongoCollection<MauBieu> MauBieu { get => _mauBieu; }
    public IMongoCollection<KyBaoCao> KyBaoCao { get => _kyBaoCao; }
    public IMongoCollection<KyBaoCaoValue> KyBaoCaoValue { get => _kyBaoCaoValue; }
    public IMongoCollection<Value> Value { get => _value; }
    public IMongoCollection<DonVi> DonVi { get => _donVi; }
    public IMongoCollection<TrangThai> TrangThai { get => _trangThai; }
    //public IMongoCollection<CoQuan> CoQuan { get => _coQuan; }
    public IMongoCollection<DonViSort> DonViSort { get=>_donViSort; }
    public IMongoCollection<Knowledge> Knowledge { get => _knowledge; }
    public IMongoCollection<Group> Group { get => _group; }
    public IMongoCollection<Label> Label { get => _label; }
    public IMongoCollection<Project> Project { get => _project; }
    public IMongoCollection<Step> Step { get => _step; }
    public IMongoCollection<Issue> Issue { get => _issue; }
    public IMongoCollection<Comment> Comment { get => _comment; }
}