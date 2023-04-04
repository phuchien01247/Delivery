using SSR.WebAPI.Data;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Extensions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using MongoDB.Driver;
using EResultResponse = SSR.WebAPI.Helpers.EResultResponse;
using System.Linq;
using DocumentFormat.OpenXml.Office2010.Excel;
using SSR.WebAPI.ViewModels;

namespace SSR.WebAPI.Services
{
    public class ExportFileService : BaseService, IExportFileService
    {
        private DataContext _context;
        //private BaseMongoDb<ExportFile, string> BaseMongoDb;
        private IMongoCollection<ExportFile> _collection;
        private IMongoCollection<Issue> _issues;
        private IMongoCollection<Label> _labels;
        private IMongoCollection<DonVi> _donvis;

        public ExportFileService(DataContext context,
            IHttpContextAccessor contextAccessor)
            : base(context, contextAccessor)
        {
            _context = context;
            //BaseMongoDb = new BaseMongoDb<ExportFile, string>(_context.ExportFile);
            _issues = context.Issue;
            _labels = context.Label;
            _donvis = context.DonVi;
            _collection = context.ExportFile;
        }
        
        public async Task<List<ExportFile>> GetBody(ExportParam exportParam)
        {
            var index = 1;
            var project = _context.Project.Find(x => x.Slug == exportParam.ProjectId && x.IsDeleted != true).FirstOrDefault();
            if (project == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY); 
            }
            var labellist = await _labels.Find(x => x.IsDeleted != true && (x.idProject == project.Id || x.IsGlobal ) ).ToListAsync();            
            var issuelist = await _issues.Find(x => x.IsDeleted != true  && x.projectId == project.Id).ToListAsync();
            var donvilist = await _donvis.Find(x => exportParam.DonViIds.Contains(x.Id) && x.IsDeleted != true).SortBy(x => x.Name).ToListAsync();


            if (issuelist != null && issuelist.Count > 0)
            {
                var listLabelTemp = issuelist.Select(x => x.Label).ToList();

                foreach (var item in listLabelTemp)
                {
                    if (item != null)
                    {
                        labellist.AddRange(item);
                        // var find = labellist.Where(x => x.Id == item) 
                    }
                    
                }
            }


            var tempLabelDis = new List<Label>();
            
            foreach (var item in labellist)
            {
                var find = tempLabelDis.Where(x => x.Id == item.Id).FirstOrDefault();
                if (find == default)
                {
                    tempLabelDis.Add(item); 
                }
            }
           
            
          //   var donVis =  issuelist.Select(x => x.DonVi).ToList();
          //
          // donVis=  donVis.Distinct().ToList();
            var listDonVis = new List<string>();
            listDonVis = donvilist.Select(x => x.Id).ToList();
            // foreach (var item in donVis)
            // {
            //     var find = donvilist.Where(x => x.Id == item).FirstOrDefault();
            //     if(find != default)
            //     {
            //         listDonVis.Add(item);
            //     }
            // }

            if (listDonVis.Count <= 0)
            {
                listDonVis = donvilist.Select(x => x.Id).ToList();
            }
            // donvilist = donvilist.OrderBy(x => x.Name).ToList();

            var exportFiles = new List<ExportFile>();

            foreach (var label in tempLabelDis)
            {
                var issue = issuelist.Where(x => x.Label != default && x.Label.Any(p => p.Id == label.Id)).ToList();

                var model = new ExportFile()
                {
                    NameLB = label.Name,
                    IdLB = label.Id,
                    Sort = index,
                    Values = new List<long>()
                };

                foreach (var donvi in listDonVis)
                {
                    var findIssueExisted = issue.Where(x => x.DonVi == donvi).ToList();
                    model.Values.Add(findIssueExisted.Count());
                }

                exportFiles.Add(model);

                index++;
            }

            return exportFiles;
        }
        public async Task<List<Header>> GetHeaders(List<string> donViIds)
        {

            var donvilist = await _donvis.Find(x => donViIds.Contains(x.Id) && x.IsDeleted != true).ToListAsync();
            donvilist = donvilist.OrderBy(x => x.Name).ToList();

            var headers = new List<Header>();
            headers.Add(new Header());
            foreach(var donvi in donvilist)
            {
                headers.Add(new Header
                {
                    IdDV = donvi.Id,
                    NameDV = donvi.Name,
                });
            }
            return headers;
        }


        public async Task<PagingModel<ExportFile>> GetPaging(PagingParam param)
        {
            PagingModel<ExportFile> result = new PagingModel<ExportFile>();
            var builder = Builders<ExportFile>.Filter;
            var filter = builder.Empty;
            filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
            if (!String.IsNullOrEmpty(param.Content))
            {
                filter = builder.And(filter,
                    builder.Where(x => x.NameLB.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
            }
            result.TotalRows = await _context.ExportFile.CountDocumentsAsync(filter);
            result.Data = await _context.ExportFile.Find(filter)

                .Skip(param.Skip)
                .Limit(param.Limit)
                .ToListAsync();
            return result;
        }

        public async Task<RenderTable> RenderTable(ExportParam model)
        {
            if (model == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
            }

            if (model.ProjectId == default)
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY); 
            }
            
            if (model.DonViIds == default || (model.DonViIds != default && model.DonViIds.Count <= 0))
            {
                throw new ResponseMessageException()
                    .WithCode(EResultResponse.FAIL.ToString())
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY); 
            }
            var data = new RenderTable();
            data.Body = await this.GetBody(model);
            data.Header = await this.GetHeaders(model.DonViIds);

            

            return data;
        }
    }

   
}
