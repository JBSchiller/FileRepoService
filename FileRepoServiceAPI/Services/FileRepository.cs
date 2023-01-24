using FileRepoServiceApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Security.Cryptography;

namespace FileRepoServiceApi.Services
{
    public class FileRepository : IFileRepository
    {
        private readonly FileRepoDC _context;
        
        public FileRepository(FileRepoDC context) => _context = context;

        public async Task<Result<bool>> UploadFileAsync(IFormFile file, string path)
        {
            try
            {
                string fullFilename = CreateFullFileName(file.FileName, path);

                MemoryStream fileStream = await CreateFileMemoryStream(file);

                //check to see if this is a new file
                //get checksum from new file
                string checksum = CalcChecksum(fileStream.ToArray());

                //check to see if this file is already in the repo
                var fileItem = await GetFileItem(fullFilename);

                if (fileItem.Data != null)
                {
                    //check to see if exixting file is identical to the new file 
                    if (fileItem.Data.Checksum == checksum)
                    {
                        return new Result<bool>(false, "Identical file is already in repo");
                    }
                }

                //add new file to the repo
                var result = await AddAsync(new FileItem
                {
                    FileName = fullFilename,
                    Contents = fileStream.ToArray(),
                    Size = file.Length,
                    FileType = file.ContentType,
                    Checksum = checksum
                });

                return result;
            }

            catch (Exception ex)
            {
                return new Result<bool>(false, ex.ToString());
            }
        }


        public async Task<Result<bool>> AddAsync(FileItem fileItem)
        {
            try
            {
                var currentHeadItem = await GetFileItem(fileItem.FileName);
                float version = 1.0F;

                //check for previous version of filename and increment version number for this entry
                if (currentHeadItem.Data != null)
                {
                    version = currentHeadItem.Data.Version + .10F;
                }

                fileItem.Version = version;
                await _context.Files!.AddAsync(fileItem);
                await _context.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                return new Result<bool>(false, ex.ToString());
            }

            return new Result<bool>(true);
        }

        public async Task<Result<bool>> DeleteAsync(string fullFileName, float version)
        {
            try
            {
                //get specific revision for deletion
                var fileItem = await _context.Files!.Where(x => x.FileName == fullFileName && x.Version == version).FirstOrDefaultAsync();

                if (fileItem == null)
                {
                    return new Result<bool>(false, string.Format("File record not found for {0} version:{1}.", fullFileName, version));
                }

                _context.Files.Remove(fileItem);
                await _context.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                return new Result<bool>(false, ex.ToString());
            }
            return new Result<bool>(true);
        }

        public async Task<Result<bool>> SoftDeleteAsync(string fullFileName, float version)
        {
            try
            {
                var fileItem = await GetFileItem(fullFileName, version);
                if (fileItem.Data == null)
                {
                    return new Result<bool>(false, string.Format("File {0} version {1} not found", fullFileName, version));
                }

                fileItem.Data.IsDeleted = true;
                _context.Entry(fileItem.Data).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                return new Result<bool>(false, ex.ToString());
            }
            return new Result<bool>(true);
        }

        public async Task<Result<int>> PurgeDeletionsAsync()
        {
            int deletions = 0;
            try
            {
                deletions = await _context.Files.Where(x => x.IsDeleted).CountAsync();
                await _context.Files.Where(x => x.IsDeleted).ExecuteDeleteAsync();
            }

            catch (Exception ex)
            {
                return new Result<int>(false, ex.ToString());
            }

            return new Result<int>(deletions);

        }

        public async Task<Result<bool>> RestoreSoftDeleteAsync(string fullFileName, float version)
        {
            try
            {
                var fileItem = await GetFileItem(fullFileName, version);
                if (fileItem.Data == null)
                {
                    return new Result<bool>(false, string.Format("File {0} version {1} not found", fullFileName, version));
                }

                fileItem.Data.IsDeleted = false;
                _context.Entry(fileItem.Data).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                return new Result<bool>(false, ex.ToString());
            }

            return new Result<bool>(true);
        }

        public async Task<Result<IEnumerable<FileItem>>> GetSoftDeletionsAsync()
        {
            try
            {
                var files = await _context.Files!.Where(x => x.IsDeleted).ToListAsync();

                return new Result<IEnumerable<FileItem>>(files);
            }

            catch (Exception ex)
            {
                return new Result<IEnumerable<FileItem>>(false, ex.ToString());
            }
        }


        public async Task<Result<IEnumerable<FileItem>>> GetAllFilesAsync()
        {
            try
            {
                var files = await _context.Files!.Where(x => !x.IsDeleted).ToListAsync();

                return new Result<IEnumerable<FileItem>>(files);
            }

            catch (Exception ex)
            {
                return new Result<IEnumerable<FileItem>>(false, ex.ToString());
            }
        }

        public async Task<Result<bool>> PutAsync(IFormFile file, string path, float version)
        {
            try
            {
                string fullFilename = CreateFullFileName(file.FileName, path);

                //
                var fileItem = await GetFileItem(fullFilename, version);
                if (fileItem.Data == null)
                {
                    return new Result<bool>(false, string.Format("File {0} version {1} not found", fullFilename, version));
                }

                var contentStream = await CreateFileMemoryStream(file);
                fileItem.Data.Contents = contentStream.ToArray();
                _context.Entry(fileItem.Data).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new Result<bool>(false, ex.Message);
            }

            catch (Exception ex)
            {
                return new Result<bool>(false, ex.ToString());
            }

            return new Result<bool>(true);
        }

        public async Task<Result<FileItem>> GetFileItem(string fileName)
        {
            try
            {
                //Get latest version of file
                var fileItem = await _context.Files!.Where(x => x.FileName == fileName && !x.IsDeleted).OrderByDescending(x => x.Version).FirstOrDefaultAsync();

                if (fileItem == null)
                {
                    return new Result<FileItem>(false, string.Format("File {0} not found", fileName));
                }

                return new Result<FileItem>(fileItem);
            }

            catch (Exception ex)
            {
                return new Result<FileItem>(false, ex.Message);
            }
        }

        public async Task<Result<FileItem>> GetFileItem(string fileName, float version)
        {
            try
            {
                //Get specific version of file
                var fileItem = await _context.Files!.Where(x => x.FileName == fileName && x.Version == version && !x.IsDeleted).FirstOrDefaultAsync();

                if (fileItem == null)
                {
                    return new Result<FileItem>(false, string.Format("File {0} version {1} not found", fileName, version));
                }

                return new Result<FileItem>(fileItem);
            }

            catch (Exception ex)
            {
                return new Result<FileItem>(false, ex.ToString()); ;
            }
        }

        //private methods

        private bool FileItemExists(string fileItem, float version)
        {
            return _context.Files.Any(x => x.FileName == fileItem && x.Version == version && !x.IsDeleted);
        }

        private string CalcChecksum(byte[] fileBytes)
        {
            using (var md5Instance = MD5.Create())
            {
                var hashResult = md5Instance.ComputeHash(fileBytes);
                return BitConverter.ToString(hashResult).Replace("-", "").ToLowerInvariant();
            }

        }

        private static string CreateFullFileName(string fileName, string path)
        {
            return string.Concat($"{path}\\", fileName);
        }

        private static async Task<MemoryStream> CreateFileMemoryStream(IFormFile file)
        {
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            return stream;
        }

    }
}
