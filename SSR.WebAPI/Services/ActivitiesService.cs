using SSR.WebAPI.Data;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Extensions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using MongoDB.Driver;
using EResultResponse = SSR.WebAPI.Exceptions.EResultResponse;
using System.Net.WebSockets;
using MongoDB.Bson;

namespace SSR.WebAPI.Services
{
public class ActivitiesService : BaseService, IActivitiesService
{
        private DataContext _context;
        private IMongoCollection<Activities> _collection;
        private BaseMongoDb<Activities, string> BaseMongoDb;

    public ActivitiesService(DataContext context,
        IHttpContextAccessor contextAccessor)
        : base(context, contextAccessor)
    {
        _context = context;
        BaseMongoDb = new BaseMongoDb<Activities, string>(_context.Activities);
        activities = new Activities();
        activities.CreatedBy = CurrentUserName;
    }

    public async Task<List<Activities>> GetByIssueId(string IssueId)
    {
        return await _context.Activities.Find(x => x.IssueId == IssueId).SortByDescending(x => x.ModifiedAt).SortByDescending(x => x.Id).ToListAsync();
    }
    public async Task<List<Activities>> GetByProjectId(string ProjectId)
    {   
        return await _context.Activities.Find(x => x.ProjectId == ProjectId).SortByDescending(x => x.ModifiedAt).SortByDescending(x => x.Id).ToListAsync();
    }

        public async Task<PagingModel<Activities>> GetPaging(ActivitiesParams param)
        {
            PagingModel<Activities> result = new PagingModel<Activities>();
            var builder = Builders<Activities>.Filter;
            var filter = builder.Empty;
            filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));

            if (!String.IsNullOrEmpty(param.Content))
            {
                filter = builder.And(filter, builder.Where(x => x.IssueId.Contains(param.Content)));
            }


            string sortBy = nameof(Activities.CreatedAt);
            result.TotalRows = await _context.Activities.CountDocumentsAsync(filter);
            result.Data = await _context.Activities.Find(filter)
                 .Sort(param.SortDesc
                    ? Builders<Activities>
                        .Sort.Descending(sortBy)
                    : Builders<Activities>
                        .Sort.Ascending(sortBy))
                .Skip(param.Skip)
                .Limit(param.Limit)
                .ToListAsync();
            return result;
            
        }
        public async Task<bool> SaveChangeActivities()
    {
        if (activities == default)
            return false;
        activities.Id = new BsonObjectId(ObjectId.GenerateNewId()).ToString();
        activities.CreatedAt = DateTime.Now;
        var result = await BaseMongoDb.CreateAsync(activities);
        if (result.Entity.Id == default || !result.Success)
            return false;
        return true;
    }
    public ActivitiesService WithId(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            activities.Id = id;
        }

        return this;
    }
    public ActivitiesService WithNguoiThucHien(string user)
    {
        if (!string.IsNullOrEmpty(user))
        {
            activities.CreatedBy = user;
        }

        return this;
    }
    public ActivitiesService WithAction(string noiDung)
    {
        if (!string.IsNullOrEmpty(noiDung))
        {
            activities.Action = noiDung;
        }
        return this;
    }
        public ActivitiesService WithLabel(List<Label> label)
        {
            activities.Label = label;
            return this;
        }
        public ActivitiesService WithContent(string noiDung)
    {
        if (!string.IsNullOrEmpty(noiDung))
        {
            activities.Content = noiDung;
        }

        return this;
    }
        public ActivitiesService WithIcon(string icon)
        {
            if (!string.IsNullOrEmpty(icon))
            {
                activities.Icon = icon;
            }

            return this;
        }
        public ActivitiesService WithIssueId(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            activities.IssueId = id;
        }
        return this;
    }
       public ActivitiesService WithProjectId(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                activities.ProjectId = id;
            }
            return this;
        }
        public ActivitiesService WithColor(string color)
        {
            if (!string.IsNullOrEmpty(color))
            {
                activities.Color = color;
            }

            return this;
        }
        public ActivitiesService WithStep(string step)
        {
            if (!string.IsNullOrEmpty(step))
            {
                activities.Step = step;
            }

            return this;
        }
        //public ActivitiesService WithFiles(List<FileShort> oldFiles, List<FileShort> newFiles, string oldGhiChu, string newGhiChu)
        //{
        //    acctivities.NewFiles = newFiles;
        //    acctivities.OldFiles = oldFiles;
        //    if (!string.IsNullOrEmpty(oldGhiChu))
        //    {
        //        acctivities.OldGhiChu = oldGhiChu;
        //    }
        //    if (!string.IsNullOrEmpty(newGhiChu))
        //    {
        //        acctivities.NewGhiChu = newGhiChu;
        //    }
        //    return this;
        //}

        //private string FormatData(string value/*, ChiTieuRenderVM ct*/)
        //{
        //    if (ct.LoaiSoLieuKeKhai == null || string.IsNullOrEmpty(value))
        //    {
        //        return value;
        //    }
        //    if (ct.LoaiSoLieuKeKhai.Id == "CATEGORIES")
        //    {
        //        var data = JsonConvert.DeserializeObject<DanhMucVM>(value);
        //        return data.Ten;
        //    }
        //    else

        //        return JsonConvert.DeserializeObject<string>(value ?? string.Empty);

        //}
        #region Properties

        private Activities activities;

        public IMongoCollection<Activities> Collection { get => _collection; set => _collection = value; }

        #endregion
    }
}
