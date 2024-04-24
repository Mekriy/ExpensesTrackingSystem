using Microsoft.AspNetCore.Http;

namespace EST.BL.Interfaces;

public interface IManageImage
{
    Task<string> UploadFile(IFormFile _formfile, Guid userId);
    Task<(byte[], string, string)> DownloadFile(string FileName, CancellationToken token);
}