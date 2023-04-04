using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Data;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Extensions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Models;
using MongoDB.Driver;
using EResultResponse = SSR.WebAPI.Exceptions.EResultResponse;
using SSR.WebAPI.Params;


namespace SSR.WebAPI.Services;

public class GroupService : BaseService, IGroupService
{
    private DataContext _context;
    private BaseMongoDb<Group, string> BaseMongoDb;
    private IMongoCollection<Group> _collection;

    public GroupService(DataContext context,
        IHttpContextAccessor contextAccessor)
        : base(context, contextAccessor)
    {
        _context = context;
        BaseMongoDb = new BaseMongoDb<Group, string>(_context.Group);
        _collection = context.Group;
    }

    public async Task<Group> Create(Group model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }

        var entity = new Group
        {
            Name = model.Name,
            CreatedBy = model.CreatedBy,
            Description = model.Description,
            Members = model.Members,
            //Role = model.Role
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

    public async Task<Group> Update(Group model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }

        var entity = _context.Group.Find(x => x.Id == model.Id).FirstOrDefault();
        if (entity == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_FOUND);
        }


        entity.Name = model.Name;
        entity.CreatedBy = model.CreatedBy;
        entity.Description = model.Description;
        entity.Members = model.Members;
        //entity.Role = model.Role;

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


        var entity = _context.Group.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();
        if (entity == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_FOUND);
        }
        entity.IsDeleted = true;
        var result = await BaseMongoDb.DeleteAsync(entity);

        if (!result.Success)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DELETE_FAILURE);
        }
    }

    public async Task<List<Group>> Get()
    {
        var data = await _context.Group.Find(x => x.IsDeleted != true).ToListAsync();
        return data;
    }

    public async Task<Group> GetById(string id)
    {
        return await _context.Group.Find(x => x.Id == id && x.IsDeleted != true)
            .FirstOrDefaultAsync();
    }

    public async Task<PagingModel<Group>> GetPaging(PagingParam param)
    {
        PagingModel<Group> result = new PagingModel<Group>();
        var builder = Builders<Group>.Filter;
        var filter = builder.Empty;
        filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
        if (!String.IsNullOrEmpty(param.Content))
        {
            filter = builder.And(filter,
                builder.Where(x => x.Name.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
        }


        result.TotalRows = await _context.Group.CountDocumentsAsync(filter);
        result.Data = await _context.Group.Find(filter)

            .Skip(param.Skip)
            .Limit(param.Limit)
            .ToListAsync();
        return result;
    }

}

