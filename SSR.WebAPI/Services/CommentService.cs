using SSR.WebAPI.Data;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Extensions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using MongoDB.Driver;
using EResultResponse = SSR.WebAPI.Exceptions.EResultResponse;

namespace SSR.WebAPI.Services
{
public class CommentService : BaseService, ICommentService
{
    private DataContext _context;
    private BaseMongoDb<Comment, string> BaseMongoDb;
    private IMongoCollection<Comment> _collection;
    private IActivitiesService _activitiesService;

        public CommentService(DataContext context,
        IHttpContextAccessor contextAccessor,
        IActivitiesService activitiesService)
        : base(context, contextAccessor)
    {
        _context = context;
        BaseMongoDb = new BaseMongoDb<Comment, string>(_context.Comment);
        _collection = context.Comment;
        _activitiesService = activitiesService;
        }

        public async Task<Comment> Create(Comment model)
        {
            if (model == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
            }

            var code = CommonExtensions.GenerateNewRandomDigit();

            var entity = new Comment
            {

                Content = model.Content,
                Files = model.Files,
                issueId = model.issueId,
                parentId = model.parentId,
                CreatedBy = CurrentUserName,
                ModifiedBy = CurrentUserName,
                CreatedAt = DateTime.Now,
            };

        var result = await BaseMongoDb.CreateAsync(entity);
        if (result.Entity.Id == default || !result.Success)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.CREATE_FAILURE);
        }
            await _activitiesService.WithAction("đã thêm bình luận")
               .WithNguoiThucHien(CurrentUserName)
               .WithIssueId(model.issueId)
               .WithContent(model.Content)
               .WithStep("Bình luận")
               .WithIcon("mdi mdi-comment")
               .WithColor("#00bf5c")
               .SaveChangeActivities();
            return entity;
    }

        public async Task<Comment> Update(Comment model)
        {
            if (model == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
            }

            var entity = _context.Comment.Find(x => x.Id == model.Id).FirstOrDefault();
            if (entity == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_FOUND);
            }

        entity.Content = model.Content;
        entity.Files = model.Files;
        entity.parentId = model.parentId;
        entity.issueId = model.issueId;  
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


        var entity = _context.Comment.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();
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

        public async Task<List<Comment>> Get()
        {
            return await _context.Comment.Find(x => x.IsDeleted != true).SortBy(x => x.ModifiedAt)
                .ToListAsync();
        }

        public async Task<Comment> GetById(string id)
        {
            return await _context.Comment.Find(x => x.Id == id && x.IsDeleted != true)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Comment>> GetWithIssId(string id)
        {
            return await _context.Comment.Find(x => (x.IsDeleted != true && x.issueId == id)).ToListAsync();
        }
        //public async Task<Comment> GetBySlug(string slug)
        //{
        //    return await _context.Comment.Find(x => x.Slug == slug && x.IsDeleted != true)
        //        .FirstOrDefaultAsync();
        //}

        public async Task<PagingModel<Comment>> GetPaging(CommentParams param)
        {
            PagingModel<Comment> result = new PagingModel<Comment>();
            var builder = Builders<Comment>.Filter;
            var filter = builder.Empty;
            filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));

            if (!String.IsNullOrEmpty(param.Content))
            {
                filter = builder.And(filter, builder.Where(x => x.issueId.Contains(param.Content)));
            }

            string sortBy = nameof(Comment.CreatedAt);
            result.TotalRows = await _collection.CountDocumentsAsync(filter);
            result.Data = await _collection.Find(filter)
                .Sort(param.SortDesc
                    ? Builders<Comment>
                        .Sort.Descending(sortBy)
                    : Builders<Comment>
                        .Sort.Ascending(sortBy))
                .Skip(param.Skip)
                .Limit(param.Limit)
                .ToListAsync();
            return result;
        }



    }
}