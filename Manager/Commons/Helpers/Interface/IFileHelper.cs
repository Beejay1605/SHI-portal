using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Helpers.Interface;

public interface IFileHelper
{
    string GetImageUrl(string imageFile);

    Task<bool> saveTextFile(string content, string path, string file_name);
    Task<bool> replaceTextFile(string content, string path);
    Task<string> getTextFileContent(string path);

    Task<bool> saveFile(string path, IFormFile file);
    Task<bool> replaceFile(string path, IFormFile file);
    Task<bool> deleteFile(string path);
}

