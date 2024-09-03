using Abp.Runtime.Session;
using Abp.UI;
using HIPMS.Authorization.PO;
using HIPMS.Authorization.Users;
using HIPMS.EntityFrameworkCore;
using HIPMS.File.Dto;
using HIPMS.Models;
using HIPMS.Options;
using HIPMS.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.File;

//[AbpAuthorize(PermissionNames.Pages_IC)]
public class FileService : IFileService
{

    //private readonly IAbpSession _abpSession;
    private readonly HIPMSDbContext _db;
    private readonly SAPSettingsOptions _sapSettings;

    public FileService(HIPMSDbContext db, IOptions<SAPSettingsOptions> sapSettings)
    {
        _db = db;
        _sapSettings = sapSettings.Value;
    }

    public async Task<bool> CreateAsync(CreateFileRequestDto input, CancellationToken cancellationToken)
    {
        if (input == null)
        {
            throw new UserFriendlyException(HIPMSErrorConst.NotNull);
        }
        string tempfilepath = null;
        if (input.Module == (int)DocumentModuleType.IC)
        {
            List<ICDocument> documents = new List<ICDocument>();
            foreach (var docPath in input.DocumentLocation)
            {
                ICDocument obj = new ICDocument();
                tempfilepath = "";
                tempfilepath = docPath.Replace(@"\", @"/");
                //tempfilepath = "https://ipmsvimqa.hzlmetals.com/" + tempfilepath;
                tempfilepath = _sapSettings.FilePath + tempfilepath;
                obj.InspectionClearanceId = input.ModuleRequestId;
                obj.ICDataId = input.RequestedItemId;
                obj.DocumentLocation = tempfilepath;
                obj.DocumentType = (int)DocumentType.Default;
                documents.Add(obj);
            }
            await _db.ICDocuments.AddRangeAsync(documents);
        }
        else if (input.Module == (int)DocumentModuleType.RFI)
        {
            List<RFIDocument> documents = new List<RFIDocument>();

            foreach (var docPath in input.DocumentLocation)
            {
                RFIDocument obj = new RFIDocument();
                tempfilepath = "";
                tempfilepath = docPath.Replace(@"\", @"/");
                //tempfilepath = "https://ipmsvimqa.hzlmetals.com/" + tempfilepath;
                tempfilepath = _sapSettings.FilePath + tempfilepath;
                obj.RFIId = input.ModuleRequestId;
                obj.RFIDataId = input.RequestedItemId;
                obj.DocumentLocation = tempfilepath;
                obj.DocumentType = (int)DocumentType.Default;
                documents.Add(obj);
            }
            await _db.RFIDocuments.AddRangeAsync(documents);
        }
        else if (input.Module == (int)DocumentModuleType.NCR)
        {
            List<NCRDocument> documents = new List<NCRDocument>();

            foreach (var docPath in input.DocumentLocation)
            {
                NCRDocument obj = new NCRDocument();
                tempfilepath = "";
                tempfilepath = docPath.Replace(@"\", @"/");
                //tempfilepath = "https://ipmsvimqa.hzlmetals.com/" + tempfilepath;
                tempfilepath = _sapSettings.FilePath + tempfilepath;
                obj.NCRMasterId = input.ModuleRequestId;
                obj.DocumentLocation = tempfilepath;
                obj.DocumentType = (int)DocumentType.Default;
                documents.Add(obj);
            }
            await _db.NCRDocuments.AddRangeAsync(documents);
        }
        else
        {

        }

        _db.SaveChanges();
        return true;
    }

    public async Task<List<FileRequest>> GetReqItemDocsAsync(long RequestId, int type)
    {
        if (RequestId <= 0)
        {
            throw new UserFriendlyException("Invalid request Id");
        }
        List<FileRequest> returnResObj = new();
        if (type == (int)DocumentModuleType.IC)
        {
            var resOj = await _db.InspectionClearance.Include(y => y.ICData)
                          .ThenInclude(z => z.ICDocuments)
                          .Where(x => x.Id == RequestId).SingleOrDefaultAsync();

            foreach (var t in resOj.ICData)
            {
                FileRequest resFileObj = new();
                resFileObj.ModuleRequestId = RequestId;
                resFileObj.Module = type;
                resFileObj.ModuleName = DocumentModuleType.IC.ToString();
                resFileObj.RequestedItemId = t.Id;

                foreach (var s in t.ICDocuments)
                {
                    ItemDocuments itemDocuments = new ItemDocuments();
                    itemDocuments.Id = s.Id;
                    itemDocuments.DocumentLocation = s.DocumentLocation;

                    resFileObj.ItemDocuments.Add(itemDocuments);
                }
                returnResObj.Add(resFileObj);
            }
        }
        else if (type == (int)DocumentModuleType.RFI)
        {
            var resOj = await _db.RFI.Include(y => y.RFIData)
                          .ThenInclude(z => z.RFIDocuments)
                          .Where(x => x.Id == RequestId).SingleOrDefaultAsync();
            foreach (var t in resOj.RFIData)
            {
                FileRequest resFileObj = new();
                resFileObj.ModuleRequestId = RequestId;
                resFileObj.Module = type;
                resFileObj.ModuleName = DocumentModuleType.RFI.ToString();
                resFileObj.RequestedItemId = t.Id;

                foreach (var s in t.RFIDocuments)
                {
                    ItemDocuments itemDocuments = new ItemDocuments();
                    itemDocuments.Id = s.Id;
                    itemDocuments.DocumentLocation = s.DocumentLocation;

                    resFileObj.ItemDocuments.Add(itemDocuments);
                }
                returnResObj.Add(resFileObj);
            }
        }
        else if (type == (int)DocumentModuleType.NCR)
        {
            var resOj = await _db.NCRMaster
                          .Include(z => z.NCRDocument)
                          .Where(x => x.Id == RequestId).SingleOrDefaultAsync();
            FileRequest resFileObj = new();
            resFileObj.ModuleRequestId = RequestId;
            resFileObj.Module = type;
            resFileObj.ModuleName = DocumentModuleType.NCR.ToString();
            resFileObj.RequestedItemId = 0;
            foreach (var t in resOj.NCRDocument)
            {
                ItemDocuments itemDocuments = new ItemDocuments();
                itemDocuments.Id = t.Id;
                itemDocuments.DocumentLocation = t.DocumentLocation;
                resFileObj.ItemDocuments.Add(itemDocuments);
                returnResObj.Add(resFileObj);
            }
        }
        return returnResObj;
    }
}