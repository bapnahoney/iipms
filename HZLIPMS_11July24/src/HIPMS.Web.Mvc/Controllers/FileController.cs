using Abp.UI;
using HIPMS.Controllers;
using HIPMS.File;
using HIPMS.File.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.Web.Controllers;

public class FileController : HIPMSControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IFileService _fileAppService;
    public FileController(IFileService fileAppService, IWebHostEnvironment environment)
    {
        _environment = environment;
        _fileAppService = fileAppService;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> OnPostUploadAsync()
    {
        var test = Request.Form.Files;
        //if (!request.HasFormContentType)
        //    throw new UserFriendlyException(L("Error"));

        //var form = await request.ReadFormAsync();
        //var formFile = form.Files["File"];
        //if (formFile is null || formFile.Length == 0)
        //    throw new UserFriendlyException(L("Error"));

        //if (!form.ContainsKey("X-Doc-Type"))
        //{
        //    throw new UserFriendlyException(L("Error"));
        //}

        //long size = files.Sum(f => f.Length);

        //foreach (var formFile in files)
        //{
        //    if (formFile.Length > 0)
        //    {
        //        var filePath = Path.GetTempFileName();

        //        using (var stream = System.IO.File.Create(filePath))
        //        {
        //            await formFile.CopyToAsync(stream);
        //        }
        //    }
        //}

        // Process uploaded files
        // Don't rely on or trust the FileName property without validation.

        return Json(" Files Uploaded!");
    }

    //upload files with complete URL
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult UploadFiles()
    {
        try
        {
            var request = Request;
            if (!request.HasFormContentType)
                throw new UserFriendlyException(L("Error"));

            string fileId = HttpContext.Request.Form["Id"][0];
            string[] temp = fileId.Split('_');
            if (temp == null)
            {
                throw new UserFriendlyException(L("Invalid request"));
            }
            string ItemId = temp[0].Replace("fileInput", "");

            string ModuleName = HttpContext.Request.Form["Module"][0];

            //var test = filesToDelete.Items[0];
            var form = request.ReadFormAsync();

            //foreach (string key in form.Result.Keys)
            //{
            //    if (key.Contains("Id"))
            //    {
            //      var val = key;
            //     //int a=  Convert.ToInt32(Request.Form[key]);
            //    }
            //}

            var formFile = form.Result.Files;

            if (formFile is null || formFile.Count == 0)
                return Json("Error!");

            string FileDir = "Files\\" + ModuleName + "\\" + ItemId;

            string FileEnvPath = Path.Combine(_environment.WebRootPath, FileDir);
            //string FilePath = Path.Combine("C:\\inetpub\\wwwroot\\ImagePublish", FileDir);

            if (!Directory.Exists(FileEnvPath))
                Directory.CreateDirectory(FileEnvPath);
            string datetimestring = DateTime.Now.ToString("ddmmyyyyss") + Path.GetExtension(formFile[0].FileName);
            var filePath = Path.Combine(FileEnvPath, datetimestring);

            using (FileStream fs = System.IO.File.Create(filePath))
            {
                formFile.First().CopyTo(fs);
            }
            //string tempfilepath = filePath.Replace(_environment.WebRootPath, "https:\\ipmsvimqa.hzlmetals.com\\");

            var x = temp[1]; //HttpContext.Request.Form["RequestId"][0];
            CreateFileRequestDto fileObj = new CreateFileRequestDto();
            string ftemp = FileDir + "\\" + datetimestring;
            fileObj.DocumentLocation.Add(ftemp);
            fileObj.RequestedItemId = Convert.ToInt16(ItemId);
            fileObj.ModuleName = ModuleName;
            fileObj.ModuleRequestId = Convert.ToInt32(x);
            if (ModuleName == DocumentModuleType.RFI.ToString())
            {
                fileObj.Module = (int)DocumentModuleType.RFI;
            }
            else if (ModuleName == DocumentModuleType.IC.ToString())
            {
                fileObj.Module = (int)DocumentModuleType.IC;
            }
            else if (ModuleName == DocumentModuleType.NCR.ToString())
            {
                fileObj.Module = (int)DocumentModuleType.NCR;
            }
            else
            {

            }

            bool isSuccess = _fileAppService.CreateAsync(fileObj, CancellationToken.None).Result;
            if (isSuccess)
            {
                var resObj = _fileAppService.GetReqItemDocsAsync(Convert.ToInt32(x), fileObj.Module);
                return Json(resObj);
            }

            //foreach (var file in formFile)
            //{
            //    //var filePath = Path.Combine(FilePath, file.FileName);
            //    var filePath = Path.Combine(FilePath, DateTime.Now.ToString()+".png");

            //    using (FileStream fs = System.IO.File.Create(filePath))
            //    {
            //        file.CopyTo(fs);
            //        continue;
            //    }

            //}
            return Json(FileDir);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Json("Error!");

        }
    }


    [HttpGet]
    //[ValidateAntiForgeryToken]
    public IActionResult DownloadFile(string fileName)
    {
       var filePath = Path.Combine("path_to_your_files", fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        var memory = new MemoryStream();
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
             stream.CopyToAsync(memory);
        }
        memory.Position = 0;
        ViewData["StreamContent"] = memory;

        string filetype = GetContentType(fileName);
        ViewData["FileType"] = filetype; // MIME type for text file
        return PartialView("../RFI/_MemoryStreamPartial");
        //return File(memory, GetContentType(filePath), fileName);
    }

    public IActionResult DownloadFileT(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            return Content("Filename is not provided.");
        }

        string filePath = Path.Combine(_environment.WebRootPath, "uploads", filename);
        if (!System.IO.File.Exists(filePath))
        {
            return Content("File not found.");
        }

        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", filename);
    }
    public IActionResult GetMemoryStreamData()
    {
        string data = "This is the data from the memory stream.";
        byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);
        MemoryStream stream = new MemoryStream(byteArray);

        ViewData["MemoryStreamData"] = stream;
        return PartialView("../RFI/_MemoryStreamPartial");
    }

    private string GetContentType(string path)
    {
        var types = GetMimeTypes();
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return types[ext];
    }

    private Dictionary<string, string> GetMimeTypes()
    {
        return new Dictionary<string, string>
        {
            { ".txt", "text/plain" },
            { ".pdf", "application/pdf" },
            { ".doc", "application/vnd.ms-word" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".csv", "text/csv" }
        };
    }

}

