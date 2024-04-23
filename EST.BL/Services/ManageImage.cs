using System.ComponentModel.DataAnnotations;
using EST.BL.Interfaces;
using EST.DAL.DataAccess.EF;
using EST.DAL.Models;
using EST.Domain.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace EST.BL.Services;

public class ManageImage : IManageImage
{
    private readonly ExpensesContext _context;
    public ManageImage(ExpensesContext context)
    {
        _context = context;
    }

    public async Task<string> UploadFile(IFormFile _formfile, Guid userId)
    {
        string FileName = "";
        try
        {
            FileInfo _fileInfo = new FileInfo(_formfile.FileName);
            FileName = Path.GetFileNameWithoutExtension(_formfile.FileName) + "_" + DateTime.Now.Ticks.ToString() + _fileInfo.Extension;
            var _getFilePath = await GetFilePath(FileName);
            using var fileStream = new FileStream(_getFilePath, FileMode.Create);
            await _formfile.CopyToAsync(fileStream);
            await CreateFile(FileName, _getFilePath, userId);
            return FileName;
        }
        catch (Exception e)
        {
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = "Can't upload image",
                Detail = "Error occured while uploading file on server"
            };
        }
    }

    public async Task<(byte[], string, string)> DownloadFile(string FileName)
    {
        try
        {
            var file = await _context.PhotoFiles.Where(f => f.FileName == FileName).FirstOrDefaultAsync();

            var getFilePath = file.FilePath;
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(getFilePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var readAllBytesAsync = await File.ReadAllBytesAsync(getFilePath);
            return (readAllBytesAsync, contentType, Path.Combine(getFilePath));
        }
        catch (Exception e)
        {
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = "Can't download file",
                Detail = "Error occured while downloading file from server"
            };
        }
    }

    private async Task<bool> CreateFile(string fileName, string _getFilePath, Guid userId)
    {
        var existingPhoto = await _context.PhotoFiles.FirstOrDefaultAsync(p => p.UserId == userId);

        if (existingPhoto != null)
        {
            existingPhoto.FilePath = _getFilePath;
            _context.PhotoFiles.Update(existingPhoto);
        }
        else
        {
            var photo = new PhotoFile
            {
                FileName = fileName,
                FilePath = _getFilePath,
                UserId = userId
            };
            await _context.PhotoFiles.AddAsync(photo);
        }

        var isSaved = await SaveAsync();
        if (isSaved)
            return true;
        throw new ApiException()
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Title = "Can't save photo",
            Detail = "Error occurred while creating or updating photo on server"
        };
    }
    private async Task<bool> SaveAsync()
    {
        var saved = await _context.SaveChangesAsync();
        return saved > 0 ? true : false;
    }
    private string GetStaticContentDirectory()
    {
        var result = "C:\\Exoft\\ETS.Uploads\\";
        if (!Directory.Exists(result))
        {
            Directory.CreateDirectory(result);
        }

        return result;
    }
    private async Task<string> GetFilePath(string fileName)
    {
        var getStaticContentDirectory = GetStaticContentDirectory();
        return Path.Combine(getStaticContentDirectory, fileName);
    }
}