// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using NPOI.HSSF.UserModel;
// using NPOI.SS.UserModel;
// using NPOI.XSSF.UserModel;
// using SSR.WebAPIInterfaces;
// using SSR.WebAPIModels;
//
// namespace SSR.WebAPI.APIs
// {
//     [Route("api/v1/[controller]")]
//     public class SyncDataController : ControllerBase
//     {
//         private readonly IWebHostEnvironment _hostingEnvironment;
//         private IDMHanhChinhService _dmHanhChinhService;
//         public SyncDataController(IDMHanhChinhService dmHanhChinhService, IWebHostEnvironment hostingEnvironment)
//         {
//             _hostingEnvironment = hostingEnvironment;
//             _dmHanhChinhService = dmHanhChinhService;
//         }
//         [HttpPost]
//         [Route("ImportExcel")]
//         public async Task<IActionResult> SyncDMHanhChinh([FromForm]IFormFile file)
//         {
//             List<Huyen> listHuyen = new List<Huyen>();
//             List<Xa> listXa = new List<Xa>();
//             string folderName = "Files";
//             var uploadPath = Path.Combine(_hostingEnvironment.ContentRootPath, folderName);
//             if (!Directory.Exists(uploadPath))
//             {
//                 Directory.CreateDirectory(uploadPath);
//             }            
//             string sFileExtension = Path.GetExtension(file.FileName).ToLower();
//             ISheet sheet;
//             string fullPath = Path.Combine(uploadPath, file.FileName);
//             using (var strem = System.IO.File.Create(fullPath))
//             {
//                 file.CopyTo(strem);
//             }
//             using (var stream = new FileStream(fullPath,FileMode.Open))
//             {
//     
//                 file.CopyTo(stream);
//                 stream.Position = 0;
//                 if (sFileExtension == ".xls")
//                 {
//                     HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
//                     sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
//                 }
//                 else
//                 {
//                     XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
//                     sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
//                 }
//                 IRow headerRow = sheet.GetRow(0); //Get Header Row
//                 int cellCount = headerRow.LastCellNum;
//
//
//                 for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
//                 {
//                     var huyen = new Huyen();
//                     var xa = new Xa();
//                     IRow row = sheet.GetRow(i);
//
//                     huyen.MaHuyen = row.GetCell(2).ToString();
//                     huyen.TenHuyen = row.GetCell(1).ToString();
//                     huyen.CapDonVi = "Huyện";
//
//                     xa.HuyenId = row.GetCell(2).ToString();
//                     xa.MaXa = row.GetCell(4).ToString();
//                     xa.TenXa = row.GetCell(3).ToString();
//                     xa.CapDonVi = row.GetCell(5).ToString();
//
//                     var indexHuyen = listHuyen.FindIndex(x => x.MaHuyen == huyen.MaHuyen);
//                     if (indexHuyen == -1)
//                     {
//                         listHuyen.Add(huyen);
//                     }
//
//                    
//                     listXa.Add(xa);
//                 }
//             }
//
//             for (int i = 0; i < listHuyen.Count; i++)
//             {
//                 var tempXa = listXa.FindAll(x => x.HuyenId == listHuyen[i].MaHuyen);
//                 if (tempXa != default)
//                 {
//                     // listHuyen[i].DanhSachXa = new List<Xa>();
//                                                                                     // listHuyen[i].DanhSachXa.AddRange(tempXa);
//                 }
//             }
//             await _dmHanhChinhService.CreateMultiHuyen(listHuyen);
//             await _dmHanhChinhService.CreateMultiXa(listXa);
//             return Ok(new {listHuyen, listXa});
//         }
//     }
// }