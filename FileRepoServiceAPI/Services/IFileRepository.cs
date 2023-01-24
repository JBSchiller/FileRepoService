using FileRepoServiceApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileRepoServiceApi.Services
{
    public interface IFileRepository
    {
        Task<Result<bool>> UploadFileAsync(IFormFile formFile, string path);
        Task<Result<bool>> PutAsync(IFormFile formFile, string path, float version);
        Task<Result<bool>> DeleteAsync(string fileName, float version);
        Task<Result<bool>> SoftDeleteAsync(string fileName, float version); 
        Task<Result<int>> PurgeDeletionsAsync();
        Task<Result<IEnumerable<FileItem>>> GetAllFilesAsync();
        Task<Result<FileItem>> GetFileItem(string fileName, float version);
        Task<Result<FileItem>> GetFileItem(string fileName);
    }
}