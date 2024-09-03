using Abp.Runtime.Session;
using Abp.UI;
using HIPMS.Authorization.PO;
using HIPMS.Authorization.PO.Dto;
using HIPMS.Authorization.Users;
using HIPMS.EntityFrameworkCore;
using HIPMS.Models;
using HIPMS.Options;
using HIPMS.Roles;
using HIPMS.Services;
using HIPMS.Services.SendEmail;
using HIPMS.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.Authorization.NCR;

//[AbpAuthorize(PermissionNames.Pages_NCR)]
public class NCRService : INCRService
{

    //private readonly IAbpSession _abpSession;
    private readonly HIPMSDbContext _db;
    private readonly IPOService _iPOService;
    private readonly IAbpSession _abpSession;
    private readonly UserManager _userManager;
    private readonly SAPSettingsOptions _sapSettings;
    private readonly IRoleAppService _roleAppService;
    private readonly ISendEmail _sendEmail;

    public NCRService(HIPMSDbContext db, IPOService iPOService, IAbpSession abpSession, IRoleAppService roleAppService,
                                      UserManager userManager, IOptions<SAPSettingsOptions> sapSettings, ISendEmail sendEmail)
    {
        _db = db;
        _abpSession = abpSession;
        _iPOService = iPOService;
        _userManager = userManager;
        _sapSettings = sapSettings.Value;
        _roleAppService = roleAppService;
        _sendEmail = sendEmail;
    }

    public async Task<List<CrudNCRModel>> CreateAsync(CrudNCRModel input)
    {
        if (input == null)
        {
            throw new UserFriendlyException(HIPMSErrorConst.NotNull);
        }
        POResultRequestDto reqObj = new();
        reqObj.PONumber = input.PONumber;

        var PORes = await _iPOService.GetPODetailAsync(reqObj);
        if (PORes == null || PORes.Id <= 0)
        {
            throw new UserFriendlyException("Invalid PO");
        }
        if (PORes.POType == POType.ZPDM.ToString() || PORes.POType == POType.ZPIM.ToString())
        {
            throw new UserFriendlyException("NCR can raise only for service PO.");
        }
        NCRMaster dbObj = new()
        {
            POMasterId = PORes.Id,
            PONumber = input.PONumber,
            ProjectName = input.ProjectName,
            Location = input.Location,
            Discipline = (int)input.Discipline,
            NCRDescription = input.NCRDescription,
            DateRaised = DateTime.Now,
            CompletionDate = input.CompletionDate,
            Criticality = (int)input.Criticality,
            UserRemark = input.UserRemark,
            OECActionRemark = input.OECActionRemark,
            Status = (int)NCRStatus.OPEN
        };

        await _db.NCRMaster.AddAsync(dbObj);
        await _db.SaveChangesAsync();
        var resobj = await GetAllAsync();


        try
        {
            var listEmails = await _db.SAPUserPOMap.Where(x => x.PONo == input.PONumber).Select(p => p.User.EmailAddress).Distinct().ToListAsync();
            if (listEmails.Any())
            {
                string csvString = string.Join(",", listEmails);

                try
                {
                    await _sendEmail.ExecuteSMTPAsync(csvString, "New measurement request", "You recieve new NCR request on PO: " + input.PONumber, CancellationToken.None);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return resobj;
        //return input;
    }

    public async Task<List<CrudNCRModel>> GetAllAsync()
    {
        string userRole = await _roleAppService.GetUserRoleName();
        List<NCRMaster> resObj = new List<NCRMaster>();
        if (userRole == ClientRoles.hzl.ToString())
        {
            resObj = await _db.NCRMaster
                              //.Include(y => y.ICData)
                              .Include(p => p.POMaster)
                              //.Include(y => y.NCRDocument)
                              .Where(x => x.Id > 0 && x.CreatorUserId == _abpSession.UserId).ToListAsync();
        }
        else if (userRole == ClientRoles.admin.ToString())
        {
            resObj = await _db.NCRMaster
                              //.Include(y => y.ICData)
                              .Include(p => p.POMaster)
                              //.Include(y => y.NCRDocument)
                              .Where(x => x.Id > 0).ToListAsync();
        }
        else
        {
            //logic needs to be check
            var listPO = _db.SAPUserPOMap.Where(x => x.UserId == _abpSession.UserId!).Select(x => x.PONo).Distinct().ToList();
            resObj = await _db.NCRMaster
                             .Include(p => p.POMaster)
                             //.Include(y => y.NCRDocument)
                             .Where(x => x.Id > 0 && listPO.Contains(x.PONumber)).ToListAsync();

            // var result = lista.Where(a => listb.Any(b => string.Compare(a, b, true) == 0));

        }
        List<CrudNCRModel> res = new List<CrudNCRModel>();

        foreach (var item in resObj)
        {
            CrudNCRModel cru = new CrudNCRModel();
            cru.NCRNo = item.NCRNo;
            cru.Id = item.Id;
            cru.NCRDescription = item.NCRDescription;
            cru.PONumber = item.PONumber;
            cru.ProjectName = item.ProjectName;
            cru.Discipline = (Discipline)item.Discipline;
            cru.Criticality = (Criticality)item.Criticality;
            cru.CompletionDate = item.CompletionDate;
            cru.DateRaised = item.DateRaised;
            cru.VendorComments = item.VendorComments;
            cru.UserRemark = item.UserRemark;
            cru.OECActionRemark = item.OECActionRemark;
            cru.Status = (NCRStatus)item.Status;
            cru.Location = item.Location;

            res.Add(cru);
        }
        return res;
    }

    public async Task<CrudNCRModel> GetAsync(long id)
    {
        string userRole = await _roleAppService.GetUserRoleName();

        NCRMaster resObj = new();
        ClientRoles clientRole = ClientRoles.vendor;

        if (userRole == ClientRoles.hzl.ToString())
        {
            clientRole = ClientRoles.hzl;
            resObj = await _db.NCRMaster
                              .Include(y => y.NCRDocument)
                              .Include(p => p.POMaster)
                              .Where(x => x.Id > 0 && x.CreatorUserId == _abpSession.UserId && x.Id == id).SingleOrDefaultAsync();
        }
        if (userRole == ClientRoles.oec.ToString())
        {
            clientRole = ClientRoles.oec;
            var listPO = _db.SAPUserPOMap.Where(x => x.UserId == _abpSession.UserId!).Select(x => x.PONo).Distinct().ToList();
            resObj = await _db.NCRMaster
                             .Include(y => y.NCRDocument)
                             .Include(p => p.POMaster)
                             .Where(x => x.Id > 0 && x.Id == id && listPO.Contains(x.PONumber)).SingleOrDefaultAsync();
        }
        else if (userRole == ClientRoles.admin.ToString())
        {
            clientRole = ClientRoles.admin;
            resObj = await _db.NCRMaster
                              .Include(y => y.NCRDocument)
                              .Include(p => p.POMaster)
                              .Where(x => x.Id > 0 && x.Id == id).SingleOrDefaultAsync();
        }
        else
        {
            clientRole = ClientRoles.vendor;
            //logic needs to be check
            var listPO = _db.SAPUserPOMap.Where(x => x.UserId == _abpSession.UserId!).Select(x => x.PONo).Distinct().ToList();
            resObj = await _db.NCRMaster
                             .Include(y => y.NCRDocument)
                             .Include(p => p.POMaster)
                             .Where(x => x.Id > 0 && x.Id == id && listPO.Contains(x.PONumber)).SingleOrDefaultAsync();

            // var result = lista.Where(a => listb.Any(b => string.Compare(a, b, true) == 0));

        }
        CrudNCRModel res = new();

        CrudNCRModel cru = new CrudNCRModel();
        cru.NCRNo = resObj.NCRNo;
        cru.Id = resObj.Id;
        cru.NCRDescription = resObj.NCRDescription;
        cru.PONumber = resObj.PONumber;
        cru.ProjectName = resObj.ProjectName;
        cru.Discipline = (Discipline)resObj.Discipline;
        cru.Criticality = (Criticality)resObj.Criticality;
        cru.CompletionDate = resObj.CompletionDate;
        cru.DateRaised = resObj.DateRaised;
        cru.VendorComments = resObj.VendorComments;
        cru.UserRemark = resObj.UserRemark;
        cru.OECActionRemark = resObj.OECActionRemark;   
        cru.Status = (NCRStatus)resObj.Status;
        cru.Location = resObj.Location;
        cru.UserRole = clientRole;
        cru.NCRDocuments = resObj.NCRDocument.ToList();
        return cru;
    }

    public async Task<List<CrudNCRModel>> UpdateAsync(CrudNCRModel input)
    {
        var editObj = await _db.NCRMaster.Where(x => x.Id == input.Id).SingleOrDefaultAsync();

        if (editObj == null)
        {
            throw new UserFriendlyException(HIPMSErrorConst.NotNull);
        }
        string userRole = await _roleAppService.GetUserRoleName();
        if (userRole == ClientRoles.hzl.ToString() || userRole == ClientRoles.admin.ToString())
        {
            editObj.UserRemark = input.UserRemark;
        }
        else if (userRole == ClientRoles.vendor.ToString())
        {
            editObj.VendorComments = input.VendorComments;
        }
        else if (userRole == ClientRoles.oec.ToString())
        {
            editObj.OECActionRemark = input.OECActionRemark;
        }
        editObj.Status = (int)input.Status;

        await _db.SaveChangesAsync();
        var resobj = await GetAllAsync();

        try
        {
            var listEmails = await _db.SAPUserPOMap.Where(x => x.PONo == input.PONumber).Select(p => p.User.EmailAddress).Distinct().ToListAsync();
            if (listEmails.Any())
            {
                string csvString = string.Join(",", listEmails);

                try
                {
                    await _sendEmail.ExecuteSMTPAsync(csvString, "Request Modified", "Request ID (" + input.Id + ")  has been updated. Please review this on IPMS portal", CancellationToken.None);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        return resobj;
    }
}