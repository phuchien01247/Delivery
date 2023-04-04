using SSR.WebAPI.Data;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Extensions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using MongoDB.Driver;
using EResultResponse = SSR.WebAPI.Exceptions.EResultResponse;

namespace SSR.WebAPI.Services;

public class StatusService : BaseService, IStatusService
{
    private DataContext _context;
    private BaseMongoDb<Status, string> BaseMongoDb;
    private IMongoCollection<Status> _collection;

    public StatusService(DataContext context,
        IHttpContextAccessor contextAccessor)
        : base(context, contextAccessor)
    {
        _context = context;
        BaseMongoDb = new BaseMongoDb<Status, string>(_context.Status);
        _collection = context.Status;
    }

    public async Task<Status> Create(Status model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }

        var entity = new Status
        {
            Name = model.Name,
            Code = model.Code,
            Sort = model.Sort,
            CreatedBy = CurrentUserName,
            ModifiedBy = CurrentUserName,
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now
        };

        var result = await BaseMongoDb.CreateAsync(entity);
        if (result.Entity.Id == default || !result.Success)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.CREATE_FAILURE);
        }

        return entity;
    }

    public async Task<Status> Update(Status model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }

        var entity = _context.Status.Find(x => x.Id == model.Id).FirstOrDefault();
        if (entity == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_FOUND);
        }

        entity.Name = model.Name;
        entity.Code = model.Code;
        entity.Sort = model.Sort;
        entity.ModifiedAt = DateTime.Now;
        entity.ModifiedBy = CurrentUserName;

        var result = await BaseMongoDb.UpdateAsync(entity);
        if (!result.Success)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.UPDATE_FAILURE);
        }

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


        var entity = _context.Status.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();
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

    public async Task<List<Status>> Get()
    {
        return await _context.Status.Find(x => x.IsDeleted != true).SortBy(x => x.Sort)
            .ToListAsync();
    }

    public async Task<Status> GetById(string id)
    {
        return await _context.Status.Find(x => x.Id == id && x.IsDeleted != true)
            .FirstOrDefaultAsync();
    }

    public async Task<PagingModel<Status>> GetPaging(PagingParam param)
    {
        var result = new PagingModel<Status>();
        var builder = Builders<Status>.Filter;
        var filter = builder.Empty;
        filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
        if (!String.IsNullOrEmpty(param.Content))
        {
            filter = builder.And(filter,
                builder.Where(x => x.Name.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
        }

        result.TotalRows = await _collection.CountDocumentsAsync(filter);
        result.Data = await _collection.Find(filter)
            .SortByDescending(x => x.Sort)
            .Skip(param.Skip)
            .Limit(param.Limit)
            .ToListAsync();
        return result;
    }
}