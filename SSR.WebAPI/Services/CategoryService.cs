using System;
using SSR.WebAPI.Data;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Extensions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using MongoDB.Driver;
using EResultResponse = SSR.WebAPI.Helpers.EResultResponse;

namespace SSR.WebAPI.Services;

public class CategoryService : BaseService, ICategoryService
{
    private DataContext _context;
    private BaseMongoDb<Category, string> BaseMongoDb;
    private IMongoCollection<Category> _collection;

    public CategoryService(DataContext context,
        IHttpContextAccessor contextAccessor)
        : base(context, contextAccessor)
    {
        _context = context;
        BaseMongoDb = new BaseMongoDb<Category, string>(_context.Categories);
        _collection = context.Categories;
    }

    public async Task<Category> Create(Category model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }
        var checkName = _context.Categories.Find(x => x.Name == model.Name && x.IsDeleted != true).FirstOrDefault();

        if (checkName != default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.NAME_EXISTED);
        }
        var entity = new Category
        {
            Name = model.Name,
            Code = model.Code,
            Sort = model.Sort,
            Thumbnail = model.Thumbnail,
            Summary = model.Summary,
            Level = model.Level,
            CreatedBy = CurrentUserName,
            ModifiedBy = CurrentUserName,
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now,
            Icon = model.Icon
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

    public async Task<Category> Update(Category model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }

        var entity = _context.Categories.Find(x => x.Id == model.Id).FirstOrDefault();
        if (entity == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_FOUND);
        }

        var checkName = _context.Categories.Find(x => x.Id != model.Id
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
        entity.Sort = model.Sort;
        entity.Thumbnail = model.Thumbnail;
        entity.Summary = model.Summary;
        entity.Level = model.Level;
        entity.ModifiedAt = DateTime.Now;
        entity.ModifiedBy = CurrentUserName;
        entity.Icon = model.Icon;

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


        var entity = _context.Categories.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();
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

    public async Task<List<Category>> Get()
    {
        return await _context.Categories.Find(x => x.IsDeleted != true).SortBy(x => x.Level)
            .ToListAsync();
    }

    public async Task<Category> GetById(string id)
    {
        return await _context.Categories.Find(x => x.Id == id && x.IsDeleted != true)
            .FirstOrDefaultAsync();
    }

    public async Task<PagingModel<Category>> GetPaging(PagingParam param)
    {
        PagingModel<Category> result = new PagingModel<Category>();
        var builder = Builders<Category>.Filter;
        var filter = builder.Empty;
        filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
        if (!String.IsNullOrEmpty(param.Content))
        {
            filter = builder.And(filter,
                builder.Where(x => x.Name.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
        }
        string sortBy = nameof(Category.Level);
        result.TotalRows = await _collection.CountDocumentsAsync(filter);
        result.Data = await _collection.Find(filter)
            .Sort(param.SortDesc
                ? Builders<Category>
                    .Sort.Descending(sortBy)
                : Builders<Category>
                    .Sort.Ascending(sortBy))
            .Skip(param.Skip)
            .Limit(param.Limit)
            .ToListAsync();
        return result;
    }
}