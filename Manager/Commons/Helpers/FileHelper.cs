 
using Manager.Commons.Helpers.Interface;
using Manager.Commons.Wrappers.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Manager.Commons.Helpers;

public class FileHelper : IFileHelper
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IPathWrapper _pathWrapper;

    public FileHelper(IWebHostEnvironment webHostEnvironment, IPathWrapper pathWrapper)
    {
        _webHostEnvironment = webHostEnvironment;
        _pathWrapper = pathWrapper;
    }
    public string GetImageUrl(string imageFile)
    {
        var imagePath = _pathWrapper.Combine(_webHostEnvironment.ContentRootPath, imageFile);
        return GetImageUrlBase64(imagePath);
    }

    private static string GetImageUrlBase64(string imagePath)
    {
        var bytes = File.ReadAllBytes(imagePath);
        return Convert.ToBase64String(bytes);
    }

    public async Task<bool> saveTextFile(string content, string path, string file_name)
    {
        try
        {
            string FilePath = Path.Combine(_webHostEnvironment.ContentRootPath, path);
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            var filePath = Path.Combine(FilePath, file_name);
            CreateTextFile(filePath, content);
            return true;
        }
        catch(Exception ex)
        {
            return false;
        }

    }

    public async Task<bool> replaceTextFile(string content, string path)
    {
        try
        {
            string FilePath = Path.Combine(_webHostEnvironment.ContentRootPath, path); 
            ReplaceTextFileContent(FilePath, content);
            return true;
        }
        catch
        {
            return false;
        } 
    }

    public async Task<string> getTextFileContent(string path)
    {
        try
        {
            return ReadTextFileContent(path);
        }
        catch
        {
            return string.Empty;
        }
    }


    private void CreateTextFile(string filePath, string content)
    {
        // Create the file and open it for writing
        using (StreamWriter writer = File.CreateText(filePath))
        {
            // Write the content to the file
            writer.Write(content);
        }
    }

    private void ReplaceTextFileContent(string filePath, string content)
    {
        // Replace the content of the file with the new content
        File.WriteAllText(filePath, content);
    }

    private string ReadTextFileContent(string filePath)
    {
        // Read the content of the file
        string content = File.ReadAllText(filePath);

        return content;
    }

    public async Task<bool> saveFile(string path, IFormFile file)
    {
        try
        { 

            // Combine the target directory with the unique file name
            string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, path);

            // Save the file to the specified path
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return true;
        }
        catch
        {
            return false;
        } 
    }

    public async Task<bool> replaceFile(string path, IFormFile file)
    {

        try
        {
            string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, path);
            File.Delete(filePath);
            // Replace the destination file with the source file
            return await saveFile(path, file);
        }
        catch
        {
            return false;
        } 
    }

    public async Task<bool> deleteFile(string path)
    {
        try
        {
            string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, path);
            File.Delete(filePath);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
