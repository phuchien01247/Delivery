//using SSR.WebAPI.Data;
//using SSR.WebAPI.Exceptions;
//using SSR.WebAPI.Extensions;
//using SSR.WebAPI.Helpers;
//using SSR.WebAPI.Interfaces;
//using SSR.WebAPI.Models;
//using SSR.WebAPI.Params;
//using MongoDB.Driver;
//using EResultResponse = SSR.WebAPI.Helpers.EResultResponse;

//namespace SSR.WebAPI.Services
//{
//    public class PhanloaiService : BaseService, IPhanloaiService
//    {
//        private DataContext _context;
//        private BaseMongoDb<Phanloai, string> BaseMongoDb;
//        private IMongoCollection<Phanloai> _collection;

//        public PhanloaiService(DataContext context,
//            IHttpContextAccessor contextAccessor)
//            : base(context, contextAccessor)
//        {
//            _context = context;
//            BaseMongoDb = new BaseMongoDb<Phanloai, string>(_context.Label);
//            _collection = context.Label;
//        }

//        public async Task<Phanloai> Create(Phanloai model)
//        {
//            if (model == default)
//            {
//                throw new ResponseMessageException()
//                    .WithCode(EResultResponse.FAIL.ToString())
//                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
//            }
 
//            var entity = new Phanloai
//            {
//                Name = model.Name,
//                Color = model.Color,
//                IsGlobal = model.IsGlobal,
//                Description = model.Description,
//                ParentId = model.ParentId,
//                Knowledge = model.Knowledge
//            };

//            var result = await BaseMongoDb.CreateAsync(entity);
//            if (result.Entity.Id == default || !result.Success)
//            {
//                throw new ResponseMessageException()
//                    .WithCode(EResultResponse.FAIL.ToString())
//                    .WithMessage(DefaultMessage.CREATE_FAILURE);
//            }

//            return entity;
//        }

//        public async Task<Phanloai> Update(Phanloai model)
//        {
//            if (model == default)
//            {
//                throw new ResponseMessageException()
//                    .WithCode(EResultResponse.FAIL.ToString())
//                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
//            }

//            var entity = _context.Label.Find(x => x.Id == model.Id).FirstOrDefault();
//            if (entity == default)
//            {
//                throw new ResponseMessageException()
//                    .WithCode(EResultResponse.FAIL.ToString())
//                    .WithMessage(DefaultMessage.DATA_NOT_FOUND);
//            }

//            entity.Name = model.Name;
//            entity.Color = model.Color;
//            entity.IsGlobal = model.IsGlobal;
//            entity.Description = model.Description;
//            entity.ParentId = model.ParentId;
//            entity.Knowledge = model.Knowledge;

//            var result = await BaseMongoDb.UpdateAsync(entity);
//            if (!result.Success)
//            {
//                throw new ResponseMessageException()
//                    .WithCode(EResultResponse.FAIL.ToString())
//                    .WithMessage(DefaultMessage.UPDATE_FAILURE);
//            }

//            return entity;
//        }

//        public async Task Delete(string id)
//        {
//            if (id == default)
//            {
//                throw new ResponseMessageException()
//                    .WithCode(EResultResponse.FAIL.ToString())
//                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
//            }


//            var entity = _context.Label.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();
//            if (entity == default)
//            {
//                throw new ResponseMessageException()
//                    .WithCode(EResultResponse.FAIL.ToString())
//                    .WithMessage(DefaultMessage.DATA_NOT_FOUND);
//            }

//            entity.ModifiedAt = DateTime.Now;
//            entity.ModifiedBy = CurrentUserName;
//            entity.IsDeleted = true;
//            var result = await BaseMongoDb.DeleteAsync(entity);

//            if (!result.Success)
//            {
//                throw new ResponseMessageException()
//                    .WithCode(EResultResponse.FAIL.ToString())
//                    .WithMessage(DefaultMessage.DELETE_FAILURE);
//            }
//        }

//        public async Task<List<Phanloai>> Get()
//        {
//            return await _context.Label.Find(x => x.IsDeleted != true).ToListAsync();
//        }

//        public async Task<Phanloai> GetById(string id)
//        {
//            return await _context.Label.Find(x => x.Id == id && x.IsDeleted != true)
//                .FirstOrDefaultAsync();
//        }

//        public async Task<PagingModel<Phanloai>> GetPaging(PagingParam param)
//        {
//            var result = new PagingModel<Phanloai>();
//            var builder = Builders<Phanloai>.Filter;
//            var filter = builder.Empty;
//            filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
//            if (!String.IsNullOrEmpty(param.Content))
//            {
//                filter = builder.And(filter,
//                    builder.Where(x => x.Name.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
//            }
//            string sortBy = nameof(Phanloai.CreatedAt);
//            result.TotalRows = await _collection.CountDocumentsAsync(filter);
//            result.Data = await _collection.Find(filter)
//                .Sort(param.SortDesc
//                    ? Builders<Phanloai>
//                        .Sort.Descending(sortBy)
//                    : Builders<Phanloai>
//                        .Sort.Ascending(sortBy))
//                .Skip(param.Skip)
//                .Limit(param.Limit)
//                .ToListAsync();
//            return result;
//        }
//    }
//}
