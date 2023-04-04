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

public class PostService : BaseService, IPostService
{
    private DataContext _context;
    private BaseMongoDb<Post, string> BaseMongoDb;
    private IMongoCollection<Post> _collection;

    public PostService(DataContext context,
        IHttpContextAccessor contextAccessor)
        : base(context, contextAccessor)
    {
        _context = context;
        BaseMongoDb = new BaseMongoDb<Post, string>(_context.Posts);
        _collection = context.Posts;
    }

    public async Task<Post> Create(Post model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }

        var code = CommonExtensions.GenerateNewRandomDigit();
        var slug = CommonExtensions.ProgressSlug(model.Slug);
        if(slug == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage("Slug không được để trống!");
        }
        var findSlug = _context.Posts.Find(x => x.Slug == slug && x.IsDeleted != true).FirstOrDefault();
        if (findSlug != null)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage("Slug đã tồn tại");
        }

        var entity = new Post
        {
            Code = code,
            Title = model.Title,
            Content = model.Content,
            Thumbnail = model.Thumbnail,
            Summary = model.Summary,
            Published = model.Published,
            PublishedAt = model.PublishedAt,
            Slug = model.Slug,
            Category = model.Category,
            Tags = model.Tags,
            Status = model.Status,
            Files = model.Files,
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

    public async Task<Post> Update(Post model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }

        var entity = _context.Posts.Find(x => x.Id == model.Id).FirstOrDefault();
        if (entity == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_FOUND);
        }

        var slug = CommonExtensions.ProgressSlug(model.Slug);
        if (slug == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage("Slug không được để trống!");
        }
        var findSlug = _context.Posts.Find(x => x.Id != model.Id && x.Slug == slug && x.IsDeleted != true).FirstOrDefault();
        if (findSlug != null)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage("Slug đã tồn tại");
        }

        entity.Title = model.Title;
        entity.Content = model.Content;
        entity.Thumbnail = model.Thumbnail;
        entity.Summary = model.Summary;
        entity.Published = model.Published;
        entity.PublishedAt = model.PublishedAt;
        entity.Slug = model.Slug;
        entity.Category = model.Category;
        entity.Tags = model.Tags;
        entity.Status = model.Status;
        entity.Files = model.Files;

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


        var entity = _context.Posts.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();
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

    public async Task<List<Post>> Get()
    {
        return await _context.Posts.Find(x => x.IsDeleted != true).SortBy(x => x.ModifiedAt)
            .ToListAsync();
    }

    public async Task<Post> GetById(string id)
    {
        return await _context.Posts.Find(x => x.Id == id && x.IsDeleted != true)
            .FirstOrDefaultAsync();
    }

    public async Task<Post> GetBySlug(string slug)
    {
        return await _context.Posts.Find(x => x.Slug == slug && x.IsDeleted != true)
            .FirstOrDefaultAsync();
    }

    public async Task<PagingModel<Post>> GetPaging(PostParams param)
    {
        PagingModel<Post> result = new PagingModel<Post>();
        var builder = Builders<Post>.Filter;
        var filter = builder.Empty;
        filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
        if (!string.IsNullOrEmpty(param.Code))
            filter = builder.And(filter, builder.Where(x => x.Category != default && x.Category.Code == param.Code));
        if (!String.IsNullOrEmpty(param.Content))
        {
            filter = builder.And(filter,
                builder.Where(x => x.Title.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
        }
        string sortBy = nameof(Post.ModifiedAt);
        result.TotalRows = await _collection.CountDocumentsAsync(filter);
        result.Data = await _collection.Find(filter)
            .Sort(param.SortDesc
                ? Builders<Post>
                    .Sort.Descending(sortBy)
                : Builders<Post>
                    .Sort.Ascending(sortBy))
            .Skip(param.Skip)
            .Limit(param.Limit)
            .ToListAsync();
        return result;
    }
}
