using FileRepoServiceApi.Models;
using FileRepoServiceApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FileRepoServiceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RepoController : ControllerBase
    {
        private readonly IFileRepository _repoService;

        public RepoController(IFileRepository repoService) => _repoService = repoService;

        [HttpPost("upload")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadFileAsync(IFormFile file, string path)
        {
            var result = await _repoService.UploadFileAsync(file, path);

            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteFileAsync(string fullFileName, float version)
        {
            var result = await _repoService.DeleteAsync(fullFileName, version);

            if (!result.Success) return BadRequest(result);
           
            return Ok(result);
        }

        [HttpDelete("softdelete")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SoftDeleteFileAsync(string fullFileName, float version)
        {
            var result = await _repoService.SoftDeleteAsync(fullFileName, version);

            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("purgedeletions")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PurgeDeletionsFileAsync()
        {
            var result = await _repoService.PurgeDeletionsAsync();

            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("getall")]
        [ProducesResponseType(typeof(Result<IEnumerable<FileItem>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<IEnumerable<FileItem>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFilesAsync()
        {
            var result = await _repoService.GetAllFilesAsync();

            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("getfile")]
        [ProducesResponseType(typeof(Result<FileItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<FileItem>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFile(string fullFileName)
        {
            var result = await _repoService.GetFileItem(fullFileName);

            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("getfileVersion")]
        [ProducesResponseType(typeof(Result<FileItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<FileItem>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFile(string fullFileName, float version)
        {
            var result = await _repoService.GetFileItem(fullFileName, version);

            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("modify")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutFileAsync(IFormFile file, string path, float version)
        {
            var result = await _repoService.PutAsync(file, path, version);

            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }
    }
}