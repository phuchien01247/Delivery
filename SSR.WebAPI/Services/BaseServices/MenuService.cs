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

namespace SSR.WebAPI.Services;

public class MenuService : BaseService, IMenuService
{
    private DataContext _context;
    private BaseMongoDb<Menu, string> BaseMongoDb;
    private IDbSettings _settings;

    public MenuService(
        IDbSettings settings,
        DataContext context,
        IHttpContextAccessor contextAccessor) :
        base(context, contextAccessor)
    {
        _context = context;
        BaseMongoDb = new BaseMongoDb<Menu, string>(_context.Menu);
        _settings = settings;
    }

    public async Task<Menu> Create(Menu model)
    {
        if (model == default)
            throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        var checkName = _context.Menu.Find(x => x.Ten == model.Ten && x.IsDeleted != true).FirstOrDefault();

        if (checkName != default)
            throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.NAME_EXISTED);

        var entity = new Menu
        {
            Ten = model.Ten,
            Path = model.Path,
            Icon = model.Icon,
            Active = model.Active,
            Action = model.Action,
            ParentId = model.ParentId,
            Level = model.Level,
            Sort = model.Sort,
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now,
            IsDeleted = false,
            CreatedBy = CurrentUserName,
            ModifiedBy = CurrentUserName,
        };

        var result = await BaseMongoDb.CreateAsync(entity);
        if (result.Entity.Id == default || !result.Success)
            throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.CREATE_FAILURE);
        return entity;
    }
    public async Task<Menu> Update(Menu model)
    {
        if (model == default)
            throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        var entity = _context.Menu.Find(x => x.Id == model.Id).FirstOrDefault();
        if (entity == default)
            throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_FOUND);

        var checkName = _context.Menu.Find(x => x.Id != model.Id && x.Ten == model.Ten && x.IsDeleted != true).FirstOrDefault();

        if (checkName != default)
            throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.NAME_EXISTED);


        entity.Ten = model.Ten;
        entity.Path = model.Path;
        entity.Icon = model.Icon;
        entity.Active = model.Active;
        entity.Action = model.Action;
        entity.ParentId = model.ParentId;
        entity.Level = model.Level;
        entity.Sort = model.Sort;
        entity.ModifiedAt = DateTime.Now;
        entity.ModifiedBy = CurrentUserName;

        var result = await BaseMongoDb.UpdateAsync(entity);
        if (!result.Success)
            throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.UPDATE_FAILURE);
        return entity;
    }

    public async Task Delete(string id)
    {
        if (id == default)
            throw new ResponseMessageException().WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        var entity = _context.Menu.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();
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

    public async Task<IEnumerable<Menu>> Get()
    {
        return await _context.Menu.Find(x => x.IsDeleted != true).ToListAsync();
    }

    public async Task<Menu> GetById(string id)
    {
        return await _context.Menu.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefaultAsync();
    }

    public async Task<PagingModel<Menu>> GetPaging(PagingParam param)
    {
        PagingModel<Menu> result = new PagingModel<Menu>();
        var builder = Builders<Menu>.Filter;
        var filter = builder.Empty;
        filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
        if (!String.IsNullOrEmpty(param.Content))
        {
            filter = builder.And(filter,
                builder.Where(x => x.Ten.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
        }
        string sortBy = nameof(Menu.Ten);
        result.TotalRows = await _context.Menu.CountDocumentsAsync(filter);
        result.Data = await _context.Menu.Find(filter)
            .Sort(param.SortDesc
                ? Builders<Menu>
                    .Sort.Ascending(sortBy)
                : Builders<Menu>
                    .Sort.Descending(sortBy))
            .Skip(param.Skip)
            .Limit(param.Limit)
            .ToListAsync();
        return result;
    }

    public async Task<List<MenuTreeVM>> GetTree()
    {
        var listDonVi = await _context.Menu.Find(_ => _.IsDeleted != true).SortBy(donVi => donVi.Level).ToListAsync();
        var parents = listDonVi.Where(x => x.ParentId == null).ToList();
        List<MenuTreeVM> list = new List<MenuTreeVM>();
        foreach (var item in parents)
        {
            MenuTreeVM donVi = new MenuTreeVM(item);
            list.Add(donVi);
            GetLoopItem(ref list, listDonVi, donVi);
        }
        return list;
    }
    public async Task<List<MenuTreeVM>> GetTreeList()
    {
        var listDonVi = await _context.Menu.Find(_ => _.IsDeleted != true).SortBy(donVi => donVi.Level).ToListAsync();
        var parents = listDonVi.Where(x => x.ParentId == null).ToList();
        List<MenuTreeVM> list = new List<MenuTreeVM>();
        foreach (var item in parents)
        {
            MenuTreeVM donVi = new MenuTreeVM(item);
            list.Add(donVi);
            GetLoopItem(ref list, listDonVi, donVi);
        }
        return list;
    }
    private List<MenuTreeVM> GetLoopItem(ref List<MenuTreeVM> list, List<Menu> items, MenuTreeVM target)
    {
        var coquan = items.FindAll((item) => item.ParentId == target.Id).ToList();
        if (coquan.Count > 0)
        {
            target.Children = new List<MenuTreeVM>();
            foreach (var item in coquan)
            {
                MenuTreeVM itemDV = new MenuTreeVM(item);
                target.Children.Add(itemDV);
                // target.SubItems.Add(itemDV);
                GetLoopItem(ref list, items, itemDV);
            }
        }
        return null;
    }
    private List<MenuTreeVM> GetLoopItemSubItems(ref List<NavMenuVM> list, List<Menu> items, NavMenuVM target, int level)
    {
        var coquan = items.FindAll((item) => item.ParentId == target.Id).ToList();
        if (coquan.Count > 0)
        {
            level++;
            target.SubItems = new List<NavMenuVM>();

            foreach (var item in coquan)
            {
                NavMenuVM itemDV = new NavMenuVM(item);
                itemDV.Level = level;
                // target.Children.Add(itemDV);
                target.SubItems.Add(itemDV);
                GetLoopItemSubItems(ref list, items, itemDV, level);
            }

        }


        return null;
    }
    public async Task<List<NavMenuVM>> GetTreeListMenuForCurrentUser(List<Menu> listMenu)
    {
        var listTotalMenu = listMenu.Select(x => x.Id).ToList();
        var parentIds = listMenu.Select(x => x.ParentId).ToList();
        listTotalMenu.AddRange(parentIds);
        var listDonVi = await _context.Menu.Find(_ => listTotalMenu.Contains(_.Id) && _.IsDeleted != true).SortBy(donVi => donVi.Level).ToListAsync();
        var parents = listDonVi.Where(x => x.ParentId == null).ToList();
        List<NavMenuVM> list = new List<NavMenuVM>();
        foreach (var item in parents)
        {
            var index = 1;
            NavMenuVM donVi = new NavMenuVM(item);
            donVi.Level = index;
            if (donVi.Level == 1)
                donVi.IsTitle = true;
            list.Add(donVi);
            GetLoopItemSubItems(ref list, listDonVi, donVi, index);
        }
        return list;
    }
}