using System;
using SSR.WebAPI.Data;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using MongoDB.Driver;
using EResultResponse = SSR.WebAPI.Helpers.EResultResponse;

namespace SSR.WebAPI.Services
{
    public class BaseAsyncRepository<TEntity, UEntityId> : IAsyncRepository<TEntity, UEntityId>
    where TEntity : class, IIdEntity<UEntityId>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        protected readonly DataContext _context;

        protected IMongoCollection<TEntity> _collection;

        private HttpContext _httpContext { get { return _contextAccessor.HttpContext; } }
        private User _appUser;
        protected User CurrentUser => GetCurrentUser();
        protected string CurrentUserName => GetCurrentUser()?.UserName;

        public string CollectionName { get; set; }

        private User GetCurrentUser()
        {
            if (_appUser != null)
                return _appUser;

            var userId = _httpContext.User?.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            if (userId != default)
                _appUser = _context.Users.FindAsync(x => x.Id == userId).Result.FirstOrDefault();
            return _appUser;
        }

        public BaseAsyncRepository(DataContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            var entityName = typeof(TEntity).Name;
            _collection = context.Database.GetCollection<TEntity>(entityName, null);
            _contextAccessor = contextAccessor;
        }

        public virtual async Task<long> CountAsync()
        {
            return await _collection.Find(x => true).CountDocumentsAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(UEntityId id)
        {
            return await _collection.Find(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            if (entity == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
            }

            entity.CreatedAt = DateTime.Now;
            entity.ModifiedAt = DateTime.Now;
            entity.CreatedBy = CurrentUser != default ? CurrentUser.UserName : string.Empty;
            entity.ModifiedBy = CurrentUser != default ? CurrentUser.UserName : string.Empty;

            await _collection.InsertOneAsync(entity);

            if (entity.Id == null)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.CREATE_FAILURE);
            }

            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity model)
        {
            if (model == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
            }

            var entity = _collection.Find(x => x.Id.Equals(model.Id)).FirstOrDefault();
            if (entity == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_FOUND);
            }
            /*
                        var checkName = _collection.Find(x => x.Id.Equals(model.Id) 
                                                              && x.Name.ToLower() == model.Name.ToLower()
                                                              && x.IsDeleted != true
                        ).FirstOrDefault();
                        if (checkName != default)
                        {
                            throw new ResponseMessageException()
                                .WithCode(EResultResponse.FAIL.ToString())
                                .WithMessage(DefaultMessage.NAME_EXISTED);
                        }*/
            model.ModifiedAt = DateTime.Now;
            model.ModifiedBy = CurrentUserName;
            ReplaceOneResult actionResult
                = await _collection.ReplaceOneAsync(x => x.Id.Equals(model.Id)
                    , model
                    , new ReplaceOptions { IsUpsert = true });
            var result = actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
            if (!result)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.UPDATE_FAILURE);

            }

            return entity;
        }

        public virtual async Task DeleteWithIdAsync(UEntityId id)
        {
            if (id == null)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
            }

            var entity = _collection.Find(x => x.Id.Equals(id) && x.IsDeleted != true).FirstOrDefault();
            if (entity == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_FOUND);
            }
            entity.ModifiedAt = DateTime.Now;
            entity.ModifiedBy = CurrentUserName;
            entity.IsDeleted = true;
            ReplaceOneResult actionResult
                = await _collection.ReplaceOneAsync(x => x.Id.Equals(entity.Id)
                    , entity
                    , new ReplaceOptions { IsUpsert = true });
            var result = actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;

            if (!result)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DELETE_FAILURE);
            }
        }

        public virtual async Task DeleteAsync(TEntity model)
        {
            if (model.Id == null)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
            }

            var entity = _collection.Find(x => x.Id.Equals(model.Id) && x.IsDeleted != true).FirstOrDefault();
            if (entity == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_FOUND);
            }
            entity.ModifiedAt = DateTime.Now;
            entity.ModifiedBy = CurrentUserName;
            entity.IsDeleted = true;
            ReplaceOneResult actionResult
                = await _collection.ReplaceOneAsync(x => x.Id.Equals(entity.Id)
                    , entity
                    , new ReplaceOptions { IsUpsert = true });
            var result = actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;

            if (!result)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DELETE_FAILURE);
            }
        }

        public virtual async Task<List<TEntity>> GetAsync()
        {
            var list = await _collection.Find(x => x.IsDeleted != true && x.Id != null).ToListAsync();
            return list;
        }

        public virtual async Task<PagingModel<TEntity>> GetPagingAsync(PagingParam param)
        {
            PagingModel<TEntity> result = new PagingModel<TEntity>();
            var builder = Builders<TEntity>.Filter;
            var filter = builder.Empty;

            filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
            if (!String.IsNullOrEmpty(param.Content))
            {
                filter = builder.And(filter,
                    builder.Where(x => x.Name.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
            }

            
            result.TotalRows = await _collection.CountDocumentsAsync(filter);
            result.Data = await _collection.Find(filter)
               
                .Skip(param.Skip)
                .Limit(param.Limit)
                .ToListAsync();

            return result;
        }
    }

}

