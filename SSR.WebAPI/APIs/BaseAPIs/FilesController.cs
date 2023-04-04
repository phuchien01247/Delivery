using System.Net;
using System.Net.Http.Headers;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using EResultResponse = SSR.WebAPI.Helpers.EResultResponse;

namespace SSR.WebAPI.APIs.Identity
{
    [Route("api/[controller]")]
    public class FilesController : Controller
    {
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FilesController(IFileService fileService, IWebHostEnvironment hostingEnvironment)
        {
            _fileService = fileService;
            _hostingEnvironment = hostingEnvironment;
        }

        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [DisableRequestSizeLimit]
        [HttpPost]
        [Route("~/api/v1/files/upload-ckeditor")]
        public async Task<IActionResult> UploadFileCKEditor()
        {
            try
            {
                var uploadDirecotroy = "files/";
                var uploadPath = Path.Combine(_hostingEnvironment.ContentRootPath, uploadDirecotroy);

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);
                var files = HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files : null;
                var dateTime = DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss");
                var path = Path.Combine(_hostingEnvironment.ContentRootPath, uploadDirecotroy, dateTime);
                IFormFile file = null;
                if (files != null && files.Count > 0)
                {
                    file = files[0];
                }

                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    FileInfo fileInfo = new FileInfo(fileName);
                    var extFile = fileInfo.Extension;
                    if (fileName.Length > 100)
                    {
                        throw new Exception("Tên tệp tin quá dài.");
                    }

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var newFileName = Guid.NewGuid().ToString() + extFile;
                    var relativePath = Path.Combine("", dateTime, newFileName);
                    var filePath = Path.Combine(uploadDirecotroy, relativePath);
                    var pathFile = Path.Combine(_hostingEnvironment.ContentRootPath, filePath);
                    using (var strem = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(strem);
                    }

                    var result =
                        await _fileService.SaveFileAsync(filePath, fileName, newFileName, extFile, file.Length);
                    return Ok(
                        new { url = "https://localhost:5001/api/v1/files/view/" + result.Id }
                    );
                }

                return Ok(
                    new ResultMessageResponse().WithCode(EResultResponse.FAIL.ToString())
                        .WithMessage("Tải tệp tin thất bại.")
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


        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)] 
        [DisableRequestSizeLimit] 
        [HttpPost]
        [Route("~/api/v1/files/upload")]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                var uploadDirecotroy = "files/";
                var uploadPath = Path.Combine(_hostingEnvironment.ContentRootPath, uploadDirecotroy);

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);
                var files = HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files : null;
                var dateTime = DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss");
                var path = Path.Combine(_hostingEnvironment.ContentRootPath, uploadDirecotroy, dateTime);
                IFormFile file = null;
                if (files != null && files.Count > 0)
                {
                    file = files[0];
                }

                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    FileInfo fileInfo = new FileInfo(fileName);
                    var extFile = fileInfo.Extension;
                    if (fileName.Length > 100)
                    {
                        throw new Exception("Tên tệp tin quá dài.");
                    }

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var newFileName = Guid.NewGuid().ToString() + extFile;
                    var relativePath = Path.Combine("", dateTime, newFileName);
                    var filePath = Path.Combine(uploadDirecotroy, relativePath);
                    var pathFile = Path.Combine(_hostingEnvironment.ContentRootPath, filePath);
                    using (var strem = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(strem);
                    }

                    var result =
                        await _fileService.SaveFileAsync(filePath, fileName, newFileName, extFile, file.Length);
                    return Ok(
                        new ResultResponse<Models.File>()
                            .WithData(result)
                            .WithCode(EResultResponse.SUCCESS.ToString())
                            .WithMessage("Tải tệp tin thành công.")
                    );
                }

                return Ok(
                    new ResultMessageResponse().WithCode(EResultResponse.FAIL.ToString())
                        .WithMessage("Tải tệp tin thất bại.")
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
        [Route("~/api/v1/files/uploadMulti")]
        public async Task<IActionResult> UploadFileMulti()
        {
            try
            {
                var uploadDirecotroy = "files/";
                var uploadPath = Path.Combine(_hostingEnvironment.ContentRootPath, uploadDirecotroy);
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);
                var files = HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files : null;
                var dateTime = DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss");
                var path = Path.Combine(_hostingEnvironment.ContentRootPath, uploadDirecotroy, dateTime);
                IFormFile file = null;
                if (files != null && files.Count > 0)
                {
                    file = files[0];
                }

                List<Models.File> modelFiles = new List<Models.File>();
                foreach (var item in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        FileInfo fileInfo = new FileInfo(fileName);
                        var extFile = fileInfo.Extension;
                        if (fileName.Length > 100)
                        {
                            throw new Exception("Tên tệp tin quá dài.");
                        }

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        var newFileName = Guid.NewGuid().ToString() + extFile;
                        var relativePath = Path.Combine("", dateTime, newFileName);
                        var filePath = Path.Combine(uploadDirecotroy, relativePath);
                        var pathFile = Path.Combine(_hostingEnvironment.ContentRootPath, filePath);
                        using (var strem = System.IO.File.Create(filePath))
                        {
                            file.CopyTo(strem);
                        }

                        var result =
                            await _fileService.SaveFileAsync(filePath, fileName, newFileName, extFile, file.Length);
                        modelFiles.Add(result);
                    }
                }

                return Ok(
                    new ResultResponse<List<Models.File>>()
                        .WithData(modelFiles)
                        .WithCode(EResultResponse.SUCCESS.ToString())
                        .WithMessage(DefaultMessage.CREATE_SUCCESS)
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

        [HttpGet]
        [Route("~/api/v1/files/view/{id}")]
        public async Task<IActionResult> ViewFile(string id)
        {
            HttpResponseMessage result = null;
            MemoryStream memory = new MemoryStream();
            var pathFile = "";
            try
            {
                var data = _fileService.GetById(id);
                if (data != null)
                {
                    var localFilePath = Path.Combine(data.Path);
                    pathFile = Path.Combine(_hostingEnvironment.ContentRootPath, localFilePath);
                    var filename = data.FileName;
                    pathFile = pathFile.Replace("\\", "/");
                    if (!System.IO.File.Exists(pathFile))
                    {
                        result = new HttpResponseMessage(HttpStatusCode.Gone);
                    }
                    else
                    {
                        var info = System.IO.File.GetAttributes(pathFile);
                        result = new HttpResponseMessage(HttpStatusCode.OK);
                        result.Content = new StreamContent(new FileStream(pathFile, FileMode.Open, FileAccess.Read));
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        result.Content.Headers.Add("x-filename", filename);
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                        result.Content.Headers.ContentDisposition.FileName = filename;
                        using (FileStream stream = new FileStream(pathFile, FileMode.Open, FileAccess.Read))
                        {
                            await stream.CopyToAsync(memory);
                        }

                        memory.Position = 0;
                        return File(memory, "application/octet-stream", filename);
                    }
                }

                return File(memory, "application/octet-stream", Path.GetFileName(pathFile));
            }
            catch (Exception ex)
            {
                return Ok(new HttpResponseMessage(HttpStatusCode.BadRequest));
            }
        }

        [HttpGet]
        [Route("~/api/v1/files/download/{id}")]
        public async Task<IActionResult> Download(string id)
        {
            HttpResponseMessage result = null;
            MemoryStream memory = new MemoryStream();
            var pathFile = "";
            try
            {
                var data = _fileService.GetById(id);
                if (data != null)
                {
                    var localFilePath = Path.Combine(data.Path);
                    pathFile = Path.Combine(_hostingEnvironment.ContentRootPath, localFilePath);
                    var filename = data.FileName;
                    if (!System.IO.File.Exists(pathFile))
                    {
                        result = new HttpResponseMessage(HttpStatusCode.Gone);
                    }
                    else
                    {
                        var info = System.IO.File.GetAttributes(pathFile);
                        result = new HttpResponseMessage(HttpStatusCode.OK);
                        result.Content = new StreamContent(new FileStream(pathFile, FileMode.Open, FileAccess.Read));
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        result.Content.Headers.Add("x-filename", filename);
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                        result.Content.Headers.ContentDisposition.FileName = filename;
                        using (FileStream stream = new FileStream(pathFile, FileMode.Open, FileAccess.Read))
                        {
                            await stream.CopyToAsync(memory);
                        }

                        memory.Position = 0;
                    }
                }

                return File(memory, "application/octet-stream", Path.GetFileName(pathFile));
            }
            catch (Exception ex)
            {
                return Ok(new HttpResponseMessage(HttpStatusCode.BadRequest));
            }
        }

        [HttpGet]
        [Route("~/api/v1/files/video/{id}")]
        public async Task GetStream(string id)
        {
            var localFilePath = "";
            var query = _fileService.GetById(id);


            localFilePath = Path.Combine(query.Path);
            if (System.IO.File.Exists(localFilePath))
            {
                byte[] buffer = new byte[1024 * 1024 * 4]; // 'Chunks' of 4MB
                long startPosition = 0;

                if (!string.IsNullOrEmpty(Request.Headers["Range"]))
                {
                    string[] range = Request.Headers["Range"].ToString().Split(new char[] {'=', '-'});
                    startPosition = long.Parse(range[1]);
                }

                using FileStream inputStream = new(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                {
                    Position = startPosition
                };
                int chunkSize = await inputStream.ReadAsync(buffer.AsMemory(0, buffer.Length));
                long fileSize = inputStream.Length;

                if (chunkSize > 0)
                {
                    Response.StatusCode = 206;
                    Response.Headers["Accept-Ranges"] = "bytes";
                    Response.Headers["Content-Range"] =
                        string.Format($" bytes {startPosition}-{fileSize - 1}/{fileSize}");
                    Response.ContentType = "application/octet-stream";

                    using Stream outputStream = Response.Body;
                    await outputStream.WriteAsync(buffer.AsMemory(0, chunkSize));
                }
            }
        }
    }
}