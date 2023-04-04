using SSR.WebAPI.Data;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Extensions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using MongoDB.Driver;
using EResultResponse = SSR.WebAPI.Helpers.EResultResponse;

namespace SSR.WebAPI.Services
{
    public class RoomService : BaseService, IRoomService
    {
        private DataContext _context;
        private BaseMongoDb<Room, string> BaseMongoDb;
        private IMongoCollection<Room> _collection;

        public RoomService(DataContext context,
            IHttpContextAccessor contextAccessor)
            : base(context, contextAccessor)
        {
            _context = context;
            BaseMongoDb = new BaseMongoDb<Room, string>(_context.Room);
            _collection = context.Room;
        }

        public async Task<Room> Create(Room model)
        {
            if (model == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
            }
            var entity = new Room
            {
                Name = model.Name,
                Address = model.Address,
                Lat = model.Lat,
                Long = model.Long,
                Image = model.Image,
                GoogleMap = model.GoogleMap,
                IsShow = model.IsShow,
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

        public async Task<Room> Update(Room model)
        {
            if (model == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
            }

            var entity = _context.Room.Find(x => x.Id == model.Id).FirstOrDefault();
            if (entity == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_FOUND);
            }

            entity.Name = model.Name;
            entity.Address = model.Address;
            entity.Lat = model.Lat;
            entity.Long = model.Long;
            entity.Image = model.Image;
            entity.GoogleMap = model.GoogleMap;
            entity.IsShow = model.IsShow;
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


            var entity = _context.Room.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();
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

        public async Task<List<Room>> Get()
        {
            return await _context.Room.Find(x => x.IsShow == true && x.IsDeleted != true).SortBy(x => x.Sort)
                .ToListAsync();
        }

        public async Task<Room> GetById(string id)
        {
            return await _context.Room.Find(x => x.Id == id && x.IsDeleted != true)
                .FirstOrDefaultAsync();
        }

        public async Task<PagingModel<Room>> GetPaging(PagingParam param)
        {
            var result = new PagingModel<Room>();
            var builder = Builders<Room>.Filter;
            var filter = builder.Empty;
            filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
            if (!String.IsNullOrEmpty(param.Content))
            {
                filter = builder.And(filter,
                    builder.Where(x => x.Name.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
            }
            string sortBy = nameof(Room.Sort);
            result.TotalRows = await _collection.CountDocumentsAsync(filter);
            result.Data = await _collection.Find(filter)
                .Sort(param.SortDesc
                    ? Builders<Room>
                        .Sort.Descending(sortBy)
                    : Builders<Room>
                        .Sort.Ascending(sortBy))
                .Skip(param.Skip)
                .Limit(param.Limit)
                .ToListAsync();
            return result;
        }
    }
    
}
