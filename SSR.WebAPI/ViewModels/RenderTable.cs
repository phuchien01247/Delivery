using SSR.WebAPI.Models;
namespace SSR.WebAPI.ViewModels
{
    public class RenderTable
    {
        public List<Header> Header { get; set; }
        public List<ExportFile> Body { get; set; } 
    }
}