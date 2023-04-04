using SSR.WebAPI.Data;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Extensions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Params;
using MongoDB.Driver;
using EResultResponse = SSR.WebAPI.Helpers.EResultResponse;
using Tag = SSR.WebAPI.Models.Tag;

namespace SSR.WebAPI.Services;

public class TagService : BaseService, ITagService
{
    private DataContext _context;
    private BaseMongoDb<Tag, string> BaseMongoDb;
    private IMongoCollection<Tag> _collection;

    public TagService(DataContext context,
        IHttpContextAccessor contextAccessor)
        : base(context, contextAccessor)
    {
        _context = context;
        BaseMongoDb = new BaseMongoDb<Tag, string>(_context.Tags);
        _collection = context.Tags;
    }

    public async Task<Tag> Create(Tag model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }
        var checkName = _context.Tags.Find(x => x.Name == model.Name && x.IsDeleted != true).FirstOrDefault();

        if (checkName != default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.NAME_EXISTED);
        }
        var entity = new Tag
        {
            Name = model.Name,
            Code = model.Code,
            Color = model.Color,
            Level = model.Level,
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

    public async Task<Tag> Update(Tag model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }

        var entity = _context.Tags.Find(x => x.Id == model.Id).FirstOrDefault();
        if (entity == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_FOUND);
        }

        var checkName = _context.Tags.Find(x => x.Id != model.Id
                                                   && x.Name.ToLower() == model.Name.ToLower()
                                                   && x.IsDeleted != true
        ).FirstOrDefault();
        if (checkName != default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.NAME_EXISTED);
        }

        entity.Name = model.Name;
        entity.Code = model.Code;
        entity.Color = model.Color;
        entity.Sort = model.Sort;
        entity.Level = model.Level;
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


        var entity = _context.Tags.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();
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

    public async Task<List<Tag>> Get()
    {
        return await _context.Tags.Find(x => x.IsDeleted != true).SortBy(x => x.Level)
            .ToListAsync();
    }

    public async Task<Tag> GetById(string id)
    {
        return await _context.Tags.Find(x => x.Id == id && x.IsDeleted != true)
            .FirstOrDefaultAsync();
    }

    public async Task<PagingModel<Tag>> GetPaging(PagingParam param)
    {
        PagingModel<Tag> result = new PagingModel<Tag>();
        var builder = Builders<Tag>.Filter;
        var filter = builder.Empty;
        filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
        if (!String.IsNullOrEmpty(param.Content))
        {
            filter = builder.And(filter,
                builder.Where(x => x.Name.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
        }
        string sortBy = nameof(Tag.Level);
        result.TotalRows = await _collection.CountDocumentsAsync(filter);
        result.Data = await _collection.Find(filter)
            .Sort(param.SortDesc
                ? Builders<Tag>
                    .Sort.Descending(sortBy)
                : Builders<Tag>
                    .Sort.Ascending(sortBy))
            .Skip(param.Skip)
            .Limit(param.Limit)
            .ToListAsync();
        return result;
    }
}