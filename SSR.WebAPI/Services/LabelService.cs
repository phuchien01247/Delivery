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
    public class LabelService : BaseService, ILabelService
    {
        private DataContext _context;
        private BaseMongoDb<Label, string> BaseMongoDb;
        private IMongoCollection<Label> _collection;
        private IActivitiesService _activitiesService;
        public LabelService(DataContext context,
            IHttpContextAccessor contextAccessor,
            IActivitiesService activitiesService)
            : base(context, contextAccessor)
        {
            _context = context;
            BaseMongoDb = new BaseMongoDb<Label, string>(_context.Label);
            _collection = context.Label;
            _activitiesService = activitiesService;
        }

        public async Task<List<LabelTreeVM>> GetTree()
        {
            var Labels = await _context.Label.Find(x => x.IsDeleted != true && x.IsGlobal != false).SortBy(donVi => donVi.ParentId).ToListAsync();
            var data = MethodExtensions.GetTree<LabelTreeVM, Label>(Labels ?? new List<Label>());

            return data;
        }

        public async Task<List<LabelTreeVM>> GetTreeWithProjId(string id)
        {
            var Labels = await _context.Label.Find(x => (x.IsDeleted != true && x.IsGlobal != false) || (x.IsDeleted != true && x.idProject == id)).SortBy(donVi => donVi.ParentId).ToListAsync();
            var data = MethodExtensions.GetTree<LabelTreeVM, Label>(Labels ?? new List<Label>());

            return data;
        }

        public async Task<Label> Create(Label model)
        {
            if (model == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
            }

            var entity = new Label
            {
                Name = model.Name,
                NameNU = model.Name.ConvertVN().ToLower(),
                Color = model.Color,
                IsGlobal = model.IsGlobal,
                Description = model.Description,
                ParentId = model.ParentId,
                Knowledge = model.Knowledge,
                idProject = model.idProject,
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
            await _activitiesService.WithAction("đã thêm nhãn mới")
            .WithNguoiThucHien(CurrentUserName)
            
            .WithContent(model.Name)
            
            .WithProjectId(model.idProject)
            
            .WithColor(model.Color)
            .SaveChangeActivities();
            return entity;
        }

        public async Task<Label> Update(Label model)
        {
            if (model == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
            }

            var entity = _context.Label.Find(x => x.Id == model.Id).FirstOrDefault();
            if (entity == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_FOUND);
            }

            entity.Name = model.Name;
            entity.NameNU = model.Name.ConvertVN().ToLower();
            entity.Color = model.Color;
            entity.IsGlobal = model.IsGlobal;
            entity.Description = model.Description;
            entity.ParentId = model.ParentId;
            entity.Knowledge = model.Knowledge;
            entity.idProject = model.idProject;
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


            var entity = _context.Label.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();
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

        public async Task<List<Label>> Get()
        {
            return await _context.Label.Find(x => x.IsDeleted != true).ToListAsync();
        }

        public async Task<List<Label>> GetWithProjId(string id)
        {
            return await _context.Label.Find(x => (x.IsDeleted != true && x.IsGlobal !=false) || (x.idProject == id)).ToListAsync();
        }

        public async Task<List<LabelTreeVM>> GetFind(string key)
        {
            var Labels = await _context.Label.Find(x => x.IsDeleted != true && (x.Name == key)).SortBy(donVi => donVi.ParentId).ToListAsync();
            var data = MethodExtensions.GetTree<LabelTreeVM, Label>(Labels ?? new List<Label>());

            return data;            
        }

        public async Task<Label> GetById(string id)
        {
            return await _context.Label.Find(x => x.Id == id && x.IsDeleted != true)
                .FirstOrDefaultAsync();
        }

        public async Task<PagingModel<Label>> GetPaging(PagingParam param)
        {
            var result = new PagingModel<Label>();
            var builder = Builders<Label>.Filter;
            var filter = builder.Empty;
            filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
            if (!String.IsNullOrEmpty(param.Content))
            {
                filter = builder.And(filter,
                    builder.Where(x => x.Name.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
            }
            string sortBy = nameof(Label.CreatedAt);
            result.TotalRows = await _collection.CountDocumentsAsync(filter);
            result.Data = await _collection.Find(filter)
                .Sort(param.SortDesc
                    ? Builders<Label>
                        .Sort.Descending(sortBy)
                    : Builders<Label>
                        .Sort.Ascending(sortBy))
                .Skip(param.Skip)
                .Limit(param.Limit)
                .ToListAsync();
            return result;
        }
    }
}
