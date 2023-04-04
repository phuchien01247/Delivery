using SSR.WebAPI.Data;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Extensions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using SSR.WebAPI.ViewModels;
using MongoDB.Driver;
using EResultResponse = SSR.WebAPI.Helpers.EResultResponse;

namespace SSR.WebAPI.Services
{
    public class RoleService : BaseService, IRoleService
    {
        private DataContext _context;
        private BaseMongoDb<Role, string> BaseMongoDb;
        private ILoggingService _logger;
        private IDbSettings _settings;

        IMongoCollection<Module> _collection;
        private IMenuService _menuService;


        public RoleService(
            IMenuService menuService,
            ILoggingService logger,
            IDbSettings settings,
            DataContext context,
            IHttpContextAccessor contextAccessor) :
            base(context, contextAccessor)
        {
            _context = context;
            BaseMongoDb = new BaseMongoDb<Role, string>(_context.Role);
            _settings = settings;
            // _logger = logger.WithCollectionName(_settings.RoleCollectionName)
            //     .WithDatabaseName(_settings.DatabaseName)
            //     .WithUserName(CurrentUserName);

            _collection = context.Modules;
            _menuService = menuService;
        }

        public async Task<Role> Create(Role model)
        {
            if (model == default)
                throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
            var checkName = _context.Role.Find(x => x.Name == model.Name && x.IsDeleted != true).FirstOrDefault();

            if (checkName != default)
                throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_FOUND);

            var entity = new Role
            {
                Name = model.Name,
                // ModifiedBy = CurrentUser.Id,
                // CreatedBy = CurrentUser.Id
            };

            var result = await BaseMongoDb.CreateAsync(entity);

            if (result.Entity.Id == default || !result.Success)
                throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.CREATE_FAILURE);


            await UpdateRolesInUser(entity);
            return entity;
        }

        public async Task<Role> Update(Role model)
        {
            if (model == default)
                throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
            var entity = _context.Role.Find(x => x.Id == model.Id).FirstOrDefault();
            if (entity == default)
                throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_FOUND);

            var checkName = _context.Role.Find(x => x.Id != model.Id && x.Name == model.Name && x.IsDeleted != true)
                .FirstOrDefault();

            if (checkName != default)
                throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.NAME_EXISTED);

            entity.Name = model.Name;

            entity.Permissions = model.Permissions;
            var result = await BaseMongoDb.UpdateAsync(entity);
            if (!result.Success)
                throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.UPDATE_FAILURE);

            // Cập nhật quyền toàn user.
            await UpdateRolesInUser(entity);
            return entity;
        }

        private async Task UpdateRolesInUser(Role role)
        {
            var filter = Builders<User>.Filter
                .ElemMatch(z => z.Role, a => a.Id == role.Id);
            var userUseRole = await _context.Users.Find(filter).ToListAsync();
            if (userUseRole != default)
            {
                foreach (var item in userUseRole)
                {
                    var index = item.Role.FindIndex(x => x.Id == role.Id);
                    if (index != -1)
                    {
                        var filterUser = Builders<User>.Filter.Eq(x => x.Id, item.Id);
                        var update = Builders<User>.Update
                            .Set(s => s.Role[index], role)
                            .Set(s => s.ModifiedAt, DateTime.Now);

                        UpdateResult actionResult
                            = await _context.Users.UpdateOneAsync(filterUser, update);
                    }
                }
            }
        }

        public async Task Delete(string id)
        {
            if (id == default)
                throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
            var entity = _context.Role.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();
            if (entity == default)
                throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_FOUND);

            entity.ModifiedAt = DateTime.Now;
            entity.IsDeleted = true;

            var result = await BaseMongoDb.DeleteAsync(entity);

            if (!result.Success)
                throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DELETE_FAILURE);
        }

        public async Task<IEnumerable<Role>> Get()
        {
            return await _context.Role.Find(x => x.IsDeleted != true).ToListAsync();
        }

        public async Task<Role> GetById(string id)
        {
            return await _context.Role.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefaultAsync();
        }

        public async Task<PagingModel<Role>> GetPaging(PagingParam param)
        {
            PagingModel<Role> result = new PagingModel<Role>();
            var builder = Builders<Role>.Filter;
            var filter = builder.Empty;
            filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
            if (!String.IsNullOrEmpty(param.Content))
            {
                filter = builder.And(filter,
                    builder.Where(x => x.Name.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
            }


            result.TotalRows = await _context.Role.CountDocumentsAsync(filter);
            result.Data = await _context.Role.Find(filter)

                .Skip(param.Skip)
                .Limit(param.Limit)
                .ToListAsync();
            return result;
        }

        public async Task<List<NavMenuVM>> GetMenuForUser(string userName)
        {
            var result = new List<NavMenuVM>();
            var user = _context.Users.Find(x => x.UserName == userName).FirstOrDefault();
            if (user != default)
            {
                var permissionsView = user.Role.SkipWhile(x => x.Permissions == null)
                    .SelectMany(x => x.Permissions)?
                    .Where(x => x.Action != null && x.Action.Contains("viewpage"))
                    .Select(p => p.Action)
                    .Distinct()
                    .ToList();

                if (permissionsView.Count > 0)
                {
                    var listMenus = await _context.Menu.Find(x => permissionsView.Contains(x.Action)).ToListAsync();
                    if (listMenus.Count > 0)
                    {
                        result = await _menuService.GetTreeListMenuForCurrentUser(listMenus);
                    }
                }
            }

            return result;
        }

        public async Task<List<string>> GetPermissionForCurrentUer(string userName)
        {
            var currentUser = await _context.Users.Find(x => x.UserName == userName && x.IsDeleted != true)
                .FirstOrDefaultAsync();
            if (currentUser == null)
                return new List<string>();
            else
            {
                if (currentUser.Role == null)
                    return new List<string>();
                var permissions = currentUser.Role
                    .SkipWhile(x => x.Permissions == null)
                    .SelectMany(x => x.Permissions)
                    .Select(x => x.Action)
                    .Distinct()
                    .ToList();
                return permissions;
            }
        }
    }
}