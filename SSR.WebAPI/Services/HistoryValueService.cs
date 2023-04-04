//using DTI.WebAPI.Data;
//using DTI.WebAPI.Extensions;
//using DTI.WebAPI.Models;
//using DTI.WebAPI.Params;
//using DTI.WebAPI.ViewModels;
//using MongoDB.Bson;
//using MongoDB.Driver;
//using Newtonsoft.Json;
//namespace DTI.WebAPI.Services
//{
//    public class HistoryValueService : BaseService
//    {
//        private DataContext _context;
//        private BaseMongoDb<HistoryValue, string> BaseMongoDb;

//        public HistoryValueService( DataContext context,
//            IHttpContextAccessor contextAccessor)
//            : base(context, contextAccessor)
//        {
//            _context = context;
//            BaseMongoDb = new BaseMongoDb<HistoryValue, string>(_context.HistoryValue);
//            history = new HistoryValue();
//            history.CreatedBy = CurrentUserName;
//        }

//        public async Task<List<HistoryValue>> GetHistoryByKyBaoCaoValueId(string kyBaoCaoValueId)
//        {
//            return await _context.HistoryValue.Find(x => x.KyBaoCaoValueId == kyBaoCaoValueId).SortByDescending(x => x.ModifiedAt).SortByDescending(x => x.Id).ToListAsync();
//        }

//        public async Task<PagingModel<HistoryValue>> GetPaging(HistoryParam param)
//        {
//            PagingModel<HistoryValue> result = new PagingModel<HistoryValue>();
//            var builder = Builders<HistoryValue>.Filter;
//            var filter = builder.Empty;
//            filter = builder.And(filter, builder.Where(x => x.IsDeleted == false && x.KyBaoCaoValueId == param.KyBaoCaoValueId));

//            string sortBy = nameof(HistoryValue.CreatedBy);
//            result.TotalRows = await _context.HistoryValue.CountDocumentsAsync(filter);
//            result.Data = await _context.HistoryValue.Find(filter)
//                .SortByDescending(x => x.ModifiedAt).SortByDescending(x => x.Id)
//                .Skip(param.Skip)
//                .Limit(param.Limit)
//                .ToListAsync();
//            return result;
//        }


//        public async Task<bool> SaveChangeHistoryValue()
//        {
//            if (history == default)
//                return false;
//            history.Id = BsonObjectId.GenerateNewId().ToString();
//            history.ThoiGian = DateTime.Now;
//            var result = await BaseMongoDb.CreateAsync(history);
//            if (result.Entity.Id == default || !result.Success)
//                return false;
//            return true;
//        }

//        public HistoryValueService WithNguoiThucHien(UserShort user)
//        {
//            if (user != default)
//            {
//                history.NguoiThucHien = user;
//            }

//            return this;
//        }
//        public HistoryValueService WithChiTieu(string chiTieu)
//        {
//            if (!string.IsNullOrEmpty(chiTieu))
//            {
//                history.ChiTieu = chiTieu;
//            }

//            return this;
//        }
        
//        public HistoryValueService WithNoiDung(string noiDung, ChiTieuRenderVM ct)
//        {
//            if (!string.IsNullOrEmpty(noiDung))
//            {
//                history.NoiDung = FormatData(noiDung,ct);
//            }

//            return this;
//        }
        
//        public HistoryValueService WithNoiDungThayDoi(string noiDung, ChiTieuRenderVM ct)
//        {
//            if (!string.IsNullOrEmpty(noiDung))
//            {
//                history.NoiDungThayDoi = FormatData(noiDung,ct);
//            }

//            return this;
//        }
//        public HistoryValueService WithKyBaoCaoValue(string kyBaoCaoValue)
//        {
//            if (!string.IsNullOrEmpty(kyBaoCaoValue))
//            {
//                history.KyBaoCaoValueId = kyBaoCaoValue;
//            }

//            return this;
//        }
//        public HistoryValueService WithValueId(string valueId)
//        {
//            if (!string.IsNullOrEmpty(valueId))
//            {
//                history.ValueId = valueId;
//            }

//            return this;
//        } 
//        public HistoryValueService WithFiles(List<FileShort> oldFiles, List<FileShort> newFiles, string oldGhiChu, string newGhiChu)
//        {
//            history.NewFiles = newFiles;
//            history.OldFiles = oldFiles;
//            if (!string.IsNullOrEmpty(oldGhiChu))
//            {
//                history.OldGhiChu = oldGhiChu;
//            }
//            if (!string.IsNullOrEmpty(newGhiChu))
//            {
//                history.NewGhiChu = newGhiChu;
//            } 
//            return this;
//        }
        
//        private string FormatData(string value, ChiTieuRenderVM ct)
//        {
//            if (ct.LoaiSoLieuKeKhai == null || string.IsNullOrEmpty(value))
//            {
//                return value;
//            }
//            if (ct.LoaiSoLieuKeKhai.Id == "CATEGORIES")
//            {
//                var data = JsonConvert.DeserializeObject<DanhMucVM>(value);
//                return data.Ten;
//            }
//            else
//            {
//                return JsonConvert.DeserializeObject<string>(value ?? string.Empty); 
//            } 
//        }
//        #region Properties

//        private HistoryValue history;

//        #endregion
//    }
//}