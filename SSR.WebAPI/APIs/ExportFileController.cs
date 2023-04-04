using ClosedXML.Excel;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using Microsoft.AspNetCore.Mvc;
using EResultResponse = SSR.WebAPI.Helpers.EResultResponse;
using SSR.WebAPI.Services;
using DocumentFormat.OpenXml.Office.CustomUI;

namespace SSR.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ExportFileController : ControllerBase
    {
        private IExportFileService _exportservice;
        private IProjectService _projectService;
        public ExportFileController(IExportFileService exportservice)
        {
            _exportservice = exportservice;
        }
        [HttpPost]
        [Route("render-table")]
        public async Task<IActionResult> Get([FromBody] ExportParam param)
        {
            try
            {
                var response = await _exportservice.RenderTable(param);

                return Ok(
                    new ResultResponse<dynamic>()
                        .WithData(response)
                        .WithCode(EResultResponse.SUCCESS.ToString())
                        .WithMessage(DefaultMessage.GET_DATA_SUCCESS)
                );
            }
            catch (ResponseMessageException ex)
            {
                return Ok(
                    new ResultMessageResponse().WithCode(ex.ResultCode)
                        .WithMessage(ex.ResultString)
                );
            }
        }
        [HttpPost]
        [Route("get-paging-params")]
        public async Task<IActionResult> GetPagingParam([FromBody] PagingParam param)
        {
            try
            {
                var response = await _exportservice.GetPaging(param);

                return Ok(
                    new ResultResponse<dynamic>()
                        .WithData(response)
                        .WithCode(EResultResponse.SUCCESS.ToString())
                        .WithMessage(DefaultMessage.GET_DATA_SUCCESS)
                );
            }
            catch (ResponseMessageException ex)
            {
                return Ok(
                    new ResultMessageResponse().WithCode(ex.ResultCode)
                        .WithMessage(ex.ResultString)
                );
            }
        }
        
        [HttpPost]
        [Route("get-export")]
        public async Task<IActionResult> ExportThongKe([FromBody] ExportParam param)
        {
            try
            {
                
                var temp = await _exportservice.RenderTable(param);
                var list = temp;

                using (var workbook = new XLWorkbook())
                {
                    string sheetName = "BaoCaoThongKe";
                    var worksheet = workbook.Worksheets.Add(sheetName);
                    var currentRow = 2;
                    var STT = 0;
                    var col = 1;
                    
                    foreach (var item in temp.Header)
                    {
                        worksheet.Cell(currentRow, col++).Value = item.NameDV;
                    }
                    worksheet.Rows().AdjustToContents();
                    worksheet.Columns().AdjustToContents();
                    worksheet.Columns().Style.Font.FontName = "Times New Roman"; 

                    worksheet.Row(1).Style.Font.Bold = true;

                    worksheet.Row(1).Style.Font.FontColor = XLColor.White;
                    
                    currentRow = 2;
                    foreach (var it in temp.Body)
                    {
                        col = 1;
                        currentRow++;
                        STT++;
                        worksheet.Cell(currentRow, 1).Value = it.NameLB;
                        foreach (var v in it.Values)
                        {
                            worksheet.Cell(currentRow, ++col).Value = v; 
                        }
                    }

                    for (int i = 1; i <= list.Body.Count + 3; i++)
                    {
                        for (int j = 1; j <= worksheet.Columns().Count(); j++)
                        {
                            worksheet.Cell(i, j).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return Ok(File(
                            content,
                            System.Net.Mime.MediaTypeNames.Application.Octet,
                            String.Format("{0} - {1}.xlsx", sheetName, DateTime.Now.ToString("dd-MM-yyyy"))));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Ok("Fail");
        }

    }
}
