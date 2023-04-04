using SSR.WebAPI.Data;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Extensions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using MongoDB.Driver;
using EResultResponse = SSR.WebAPI.Exceptions.EResultResponse;
using Microsoft.VisualBasic;

namespace SSR.WebAPI.Services;

public class IssueService : BaseService, IIssueService
{
    private DataContext _context;
    private BaseMongoDb<Issue, string> BaseMongoDb;
    private IMongoCollection<Issue> _collection;
    private IActivitiesService _activitiesService;
    public IssueService(DataContext context,
        IHttpContextAccessor contextAccessor,
        IActivitiesService activitiesService)
        : base(context, contextAccessor)
    {
        _context = context;
        BaseMongoDb = new BaseMongoDb<Issue, string>(_context.Issue);
        _collection = context.Issue;
        _activitiesService = activitiesService;
    }

    public async Task<Issue> Create(Issue model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }

        var code = CommonExtensions.GenerateNewRandomDigit();
        
        var entity = new Issue
        {        
            Description = model.Description,
            Title = model.Title,
            TitleNU = model.Title.ConvertVN().ToLower(),
            Group = model.Group,
            User = model.User,
            DonVi = model.DonVi,
            projectId = model.projectId,
            DueDate = model.DueDate,
            Label = model.Label,
            Files = model.Files,
            StepId = model.StepId,
            CreatedBy = CurrentUserName,
            CreatedAt = DateTime.Now,
        };

        var result = await BaseMongoDb.CreateAsync(entity);
        if (result.Entity.Id == default || !result.Success)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.CREATE_FAILURE);
        }
        await _activitiesService.WithAction("đã thêm yêu cầu lỗi")
            .WithNguoiThucHien(CurrentUserName)
            .WithIssueId(result.Entity.Id)
            .WithContent(model.Title)
            .WithStep("Open")
            .WithIcon("mdi mdi-circle-slice-8")
            .WithProjectId(model.projectId)
            .WithLabel(model.Label)
            .WithColor("#cf2b02")
            .SaveChangeActivities();
        return entity;
        
    }

    public async Task<Issue> Update(Issue model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }

        var entity = _context.Issue.Find(x => x.Id == model.Id).FirstOrDefault();
        if (entity == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_FOUND);
        }

        entity.Description = model.Description;
        entity.Title = model.Title;
        entity.TitleNU = model.Title.ConvertVN().ToLower();
        entity.Group = model.Group;
        entity.User = model.User;
        entity.DonVi = model.DonVi;
        entity.projectId = model.projectId;
        entity.DueDate = model.DueDate;
        entity.Label = model.Label;
        entity.DueDate = model.DueDate;
        entity.StepId = model.StepId;
        entity.ModifiedAt = DateTime.Now;
        entity.ModifiedBy = CurrentUserName;

        var result = await BaseMongoDb.UpdateAsync(entity);
        if (!result.Success)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.UPDATE_FAILURE);
        }
        await _activitiesService.WithAction("đã cập nhật yêu cầu")
            .WithNguoiThucHien(CurrentUserName)
            .WithIssueId(result.Entity.Id)
            .WithContent(model.Title)
            .WithIcon("mdi mdi-check-circle")
            .WithProjectId(model.projectId)
            .WithLabel(model.Label)
            
            .WithColor("#f7a752")
            .SaveChangeActivities();
        return entity;
    }

    public async Task Delete(string id)
    {
        if (id == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }


        var entity = _context.Issue.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();
        if (entity == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_FOUND);
        }

        entity.ModifiedAt = DateTime.Now;
        entity.ModifiedBy = CurrentUserName;
        entity.IsDeleted = true;
        var result = await BaseMongoDb.DeleteAsync(entity);

        if (!result.Success)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DELETE_FAILURE);
        }
    }

    public async Task<List<Issue>> Get()
    {
        return await _context.Issue.Find(x => x.IsDeleted != true).SortBy(x => x.ModifiedAt)
            .ToListAsync();
    }

    public async Task<Issue> GetById(string id)
    {
        return await _context.Issue.Find(x => x.Id == id && x.IsDeleted != true)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Issue>> GetWithProjId(string id)
    {
        return await _context.Issue.Find(x => (x.IsDeleted != true && x.projectId == id)).ToListAsync();
    }

    public async Task<List<Issue>> GetOpenWithProjId(string id)
    {
        return await _context.Issue.Find(x =>  (x.IsDeleted != true && x.projectId == id && x.StepId == "Open")).ToListAsync();
    }

    public async Task<List<Issue>> GetCloseWithProjId(string id)
    {
        return await _context.Issue.Find(x => (x.IsDeleted != true && x.projectId == id && x.StepId == "Close")).ToListAsync();
    }

    public async Task<PagingModel<Issue>> GetPaging(PagingParam param)
    {
        PagingModel<Issue> result = new PagingModel<Issue>();
        var builder = Builders<Issue>.Filter;
        var filter = builder.Empty;
        filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
        if (!String.IsNullOrEmpty(param.Content))
        {
            filter = builder.And(filter,
               builder.Where(x => x.TitleNU.Trim().ToLower().Contains(param.Content.ConvertVN().Trim().ToLower())));
            filter = builder.Or(filter,
               builder.Where(x => (x.Label.Any(b=> b.NameNU.Contains(param.Content.ConvertVN().Trim().ToLower())))));
        }   
        string sortBy = nameof(Issue.ModifiedAt);
        result.TotalRows = await _collection.CountDocumentsAsync(filter);
        result.Data = await _collection.Find(filter)
            .Sort(param.SortDesc
                ? Builders<Issue>
                    .Sort.Descending(sortBy)
                : Builders<Issue>
                    .Sort.Ascending(sortBy))
            .Skip(param.Skip)
            .Limit(param.Limit)
            .ToListAsync();
        return result;
    }
    public async Task<PagingModel<Issue>> GetPagingLoiTrongNgay(PagingParam param)
    {
        PagingModel<Issue> result = new PagingModel<Issue>();
        var builder = Builders<Issue>.Filter;
        var filter = builder.Empty;
        filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
        if (!String.IsNullOrEmpty(param.Content))
        {
            filter = builder.And(filter,
                builder.Where(x => x.Title.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
        }       
        filter = builder.And(filter, builder.Where(x => x.StepId == "Open" && (x.User.Any(b => b.UserName == CurrentUserName) || x.Group.Any(c => c.Members.Any(b => b.UserName == CurrentUserName)) || x.CreatedBy == CurrentUserName)));
        string sortBy = nameof(Issue.ModifiedAt);
        result.TotalRows = await _collection.CountDocumentsAsync(filter);
        result.Data = await _collection.Find(filter)
            .Sort(param.SortDesc
                ? Builders<Issue>
                    .Sort.Descending(sortBy)
                : Builders<Issue>
                    .Sort.Ascending(sortBy))
            .Skip(param.Skip)
            .Limit(param.Limit)
            .ToListAsync();
        return result;
    }
    public async Task<PagingModel<Issue>> GetPagingLoiNgay(PagingParam param)
    {
        PagingModel<Issue> result = new PagingModel<Issue>();
        var builder = Builders<Issue>.Filter;
        var filter = builder.Empty;
        filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
        if (!String.IsNullOrEmpty(param.Content))
        {
            filter = builder.And(filter,
                builder.Where(x => x.Title.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
        }
        if (param.NgayTao != default)
        {
            var ngayTao = param.NgayTao.Value;
            var start = new DateTime(ngayTao.Year, ngayTao.Month, ngayTao.Day).AddTicks(-1);
            var end = start.AddDays(1).AddTicks(-1);
            filter = builder.And(filter, builder.Where(x => (start < x.CreatedAt && x.CreatedAt < end ) && x.StepId == "Open" && (x.User.Any(b => b.UserName == CurrentUserName) || x.Group.Any(c => c.Members.Any(b => b.UserName == CurrentUserName)) || x.CreatedBy == CurrentUserName)));
        }
        string sortBy = nameof(Issue.ModifiedAt);
        result.TotalRows = await _collection.CountDocumentsAsync(filter);
        result.Data = await _collection.Find(filter)
            .Sort(param.SortDesc
                ? Builders<Issue>
                    .Sort.Descending(sortBy)
                : Builders<Issue>
                    .Sort.Ascending(sortBy))
            .Skip(param.Skip)
            .Limit(param.Limit)
            .ToListAsync();
        return result;
    }
    public async Task<PagingModel<Issue>> GetPagingLoiDaGiaiQuyet(PagingParam param)
    {
        PagingModel<Issue> result = new PagingModel<Issue>();
        var builder = Builders<Issue>.Filter;
        var filter = builder.Empty;
        filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
        if (!String.IsNullOrEmpty(param.Content))
        {
            filter = builder.And(filter,
                builder.Where(x => x.Title.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
        }
        if (param.NgayTao != default)
        {
            var ngayTao = param.NgayTao.Value;
            var start = new DateTime(ngayTao.Year, ngayTao.Month, ngayTao.Day).AddTicks(-1);
            var end = start.AddDays(1).AddTicks(-1);
            filter = builder.And(filter, builder.Where(x => (start < x.CreatedAt && x.CreatedAt <= end) && x.StepId == "Close" && (x.User.Any(b => b.UserName == CurrentUserName) || x.Group.Any(c => c.Members.Any(b => b.UserName == CurrentUserName)) || x.CreatedBy == CurrentUserName)));
        }
        string sortBy = nameof(Issue.ModifiedAt);
        result.TotalRows = await _collection.CountDocumentsAsync(filter);
        result.Data = await _collection.Find(filter)
            .Sort(param.SortDesc
                ? Builders<Issue>
                    .Sort.Descending(sortBy)
                : Builders<Issue>
                    .Sort.Ascending(sortBy))
            .Skip(param.Skip)
            .Limit(param.Limit)
            .ToListAsync();
        return result;
    }
}
