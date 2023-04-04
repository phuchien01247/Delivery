using Microsoft.AspNetCore.Mvc;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using SSR.WebAPI.ViewModels;

namespace SSR.WebAPI.Interfaces
{
    public interface IExportFileService
    {
        Task<PagingModel<ExportFile>> GetPaging(PagingParam param);
        Task<RenderTable> RenderTable(ExportParam model);
    }
}
