using File = SSR.WebAPI.Models.File;

namespace SSR.WebAPI.Interfaces;

public interface IFileService
{
    File GetById(string id);
    Task<File> SaveFileAsync(string filePath, string fileName, string newFileName, string fileExt, long fileSize);
    Task<File> SaveFileAsync(string fileId, string filePath, string fileName, string newFileName, string fileExt, long fileSize);
}