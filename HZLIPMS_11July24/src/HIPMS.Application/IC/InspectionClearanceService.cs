using Abp.Authorization;
using Abp.Runtime.Session;
using Abp.UI;
using HIPMS.Authorization;
using HIPMS.Authorization.PO;
using HIPMS.Authorization.PO.Dto;
using HIPMS.Authorization.Users;
using HIPMS.EntityFrameworkCore;
using HIPMS.IC.Dto;
using HIPMS.Models;
using HIPMS.Options;
using HIPMS.Roles;
using HIPMS.Services;
using HIPMS.Services.SendEmail;
using HIPMS.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static HIPMS.Shared.SharedEnum;
using POSAPResponse = HIPMS.IC.Dto.POSAPResponse;

namespace HIPMS.IC;

[AbpAuthorize(PermissionNames.Pages_IC)]
public class InspectionClearanceService : IInspectionClearanceService
{
    private readonly HIPMSDbContext _db;
    private readonly IPOService _iPOService;
    private readonly IAbpSession _abpSession;
    private readonly UserManager _userManager;
    private readonly SAPSettingsOptions _sapSettings;
    private readonly IRoleAppService _roleAppService;
    private readonly ISendEmail _sendEmail;

    public InspectionClearanceService(HIPMSDbContext db, IPOService iPOService, IAbpSession abpSession,
                                      IRoleAppService roleAppService,
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

    public async Task<POMaster> SearchPODetailAsync(SearchPORequest input, CancellationToken cancellationToken)
    {
        if (input == null)
        {
            throw new UserFriendlyException("PO number cannot be null.");
        }
        POResultRequestDto pOResultRequestDto = new()
        {
            PONumber = input.PONumber
        };

        var roleName = await _roleAppService.GetUserRoleName();
        //this section only mapped po can view by vendor
        if (roleName == ClientRoles.vendor.ToString())
        {
            var listPO = _db.SAPUserPOMap.Where(x => x.UserId == _abpSession.UserId).Select(x => x.PONo).Distinct().ToList();
            if (listPO.Contains(input.PONumber))
            {

            }
            else
            {
                throw new UserFriendlyException(203, "User is not mapped with this PO.");
            }
        }

        //POMaster pOResponse = await _iPOService.GetPODetailAsync(pOResultRequestDto);
        //if (pOResponse == null || pOResponse.Id == 0)
        //{
          POMaster pOResponse = new();
          PagedPOResultRequestDto inp = new();
          inp.PONo = input.PONumber;
          object obj = await _iPOService.GetPODetailFromSAPV2Async(inp, cancellationToken);

            try
            {
                // { HIPMS.Services.ResponseDataModel}

                ResponseModel resObj = ((HIPMS.Services.MainResponse)obj).Response;
                //JsonConvert.DeserializeObject<ResponseDataModel>(obj);
                if (resObj == null)
                {
                    throw new UserFriendlyException(204, "PO detail not available in SAP.");
                }
                if (resObj.Code == 200)
                {
                    pOResponse = await _iPOService.GetPODetailAsync(pOResultRequestDto);
                    return pOResponse;
                }
                else
                {
                    throw new UserFriendlyException(403, "Unable to fetch PO detail.");
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(403, "Unable to fetch PO detail.");
            }
        //}
        
    }
    public async Task<bool> Update()
    {
        throw new UserFriendlyException("Unable to fetch PO detail.");
    }
    public async Task<CreateICRequestDto> CreateAsync(CreateICRequestDto input, CancellationToken cancellationToken)
    {
        if (input == null)
        {
            throw new UserFriendlyException(HIPMSErrorConst.NotNull);
        }

        if (input.ICItems == null || input.ICItems.Count <= 0)
        {
            throw new UserFriendlyException("PO items not found");
        }
        //if sum of input qty = 0 handle case
        var sum = input.ICItems.Sum(x => x.ICInputQty);
        if (sum <= 0)
        {
            throw new UserFriendlyException("Atleast one input quantity must be greater than 0.");
        }
        InspectionClearance dbObj = new()
        {
            VendorName = input.VendorName,
            VendorNo = input.VendorNo,
            VendorRemark = input.VendorRemark,
            POMasterId = input.ICItems[0].POMasterId,
            ManufacturerName = input.ManufacturerName,
            ManufacturerPlantAddress = input.ManufacturerPlantAddress,
            CreatorUserId = _abpSession.UserId,
            CreationTime = DateTime.Now,
            Status = (int)ICReqStatus.Pending,
            //ICData = input.ICItems,
        };
        foreach (var item in input.ICItems)
        {
            if (item.ICInputQty < 0 || item.ICBalanceQty < 0 || item.ICPreviousQty < 0 || item.POQty < 0)
            {
                throw new UserFriendlyException("Invalid Quantity.");
            }
            if (item.ICInputQty > item.ICBalanceQty)
            {
                throw new UserFriendlyException("Input quantity must be less then or equal to balance quantity.");
            }
            if (item.ICInputQty > 0 && (string.IsNullOrEmpty(item.Make) || string.IsNullOrEmpty(item.Model) || string.IsNullOrEmpty(item.PartNo)))
            {
                throw new UserFriendlyException("Make/Model/Part is missing.");
            }
            if (!string.IsNullOrEmpty(input.PO))
                item.PONo = input.PO;
            dbObj.ICData.Add(item);
        }
        await _db.InspectionClearance.AddAsync(dbObj);
        _db.SaveChanges();

        try
        {
            var listEmails = await _db.SAPUserPOMap.Where(x => x.PONo == input.PO).Select(p => p.User.EmailAddress).Distinct().ToListAsync();
            if(listEmails.Any())
            {
                string csvString = string.Join(",", listEmails);

                try
                {
                    await _sendEmail.ExecuteSMTPAsync(csvString, "New measurement request", "You recieve new measurement request from " + input.VendorName + " . PO: " + input.PO, CancellationToken.None);
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
       

        return input;
    }
    public async Task<ICEditRequest> UpdateAsync(ICEditRequest input, ClaimsPrincipal usrObj, CancellationToken cancellationToken)
    {
        InspectionClearance editObj = await GetReqItemsAsync(input.Id);
        if (editObj == null)
        {
            throw new UserFriendlyException(HIPMSErrorConst.NotNull);
        }
        if (editObj.Status == (int)ICReqStatus.Approve)
        {
            throw new UserFriendlyException("User cannot edit approved request");
        }
        if (input.ICItems == null || input.ICItems.Count <= 0)
        {
            throw new UserFriendlyException("PO items not found");
        }
        //if sum of input qty = 0 handle case
        var sum = input.ICItems.Sum(x => x.ICInputQty);
        if (sum <= 0)
        {
            throw new UserFriendlyException("Atleast one input quantity must be greater than 0.");
        }
        string UserRole = await _roleAppService.GetUserRoleName();
        PagedPOResultRequestDto inp = new();
        //need to check should not blank PO and RFIid
        inp.PONo = input.ICItems.FirstOrDefault().PONo;
        inp.EditId = input.ICItems.FirstOrDefault().InspectionClearanceId;

        if ((UserRole == ClientRoles.hzl.ToString() || UserRole == ClientRoles.admin.ToString()) && input.Status == (int)ICReqStatus.Approve)
        {
            await _iPOService.GetPODetailFromSAPV2OnApproveAsync(inp, cancellationToken);
        }



        bool isApproveAction = false;
        DateTime currentDateTime = DateTime.Now;
        string sessionUserName = _userManager.GetUserName(usrObj);

        SAPICEdit sAPICEdit = new()
        {
            PO_NO = editObj.ICData.FirstOrDefault().PONo,// "8510001294",// "8510001294",// Convert.ToString(input.PONo),
            AUTH_KEY = _sapSettings.Authkey,// "MME5MI9OCDN5VERHD1C0MZYZRVH4",ConfigurationManager.AppSettings["SAPAuthKey"]
            VENDOR_NO = editObj.VendorNo,
            NOTE = editObj.VendorRemark,
            IC_APProval_DATE = currentDateTime.ToString(),
            IC_NO = input.Id,
            IC_SUBMIT_DATE = currentDateTime.ToString()
        };
     

        foreach (var item in input.ICItems)
        {
            if (item.ICInputQty < 0 || item.ICBalanceQty < 0 || item.ICPreviousQty < 0 || item.POQty < 0)
            {
                throw new UserFriendlyException("Invalid Quantity.");
            }
            if (item.ICInputQty > item.ICBalanceQty)
            {
                throw new UserFriendlyException(400, "BalanceQuantity");
            }
            if (item.ICInputQty > 0 && (string.IsNullOrEmpty(item.Make) || string.IsNullOrEmpty(item.Model) || string.IsNullOrEmpty(item.PartNo)))
            {
                throw new UserFriendlyException("Make/Model/Part is missing.");
            }

            //var ICInputQty = await _db.ICData.Where(y => y.Id == item.Id).Select(y => y.ICInputQty).SingleOrDefaultAsync();
            
            //Added y.Id != item.Id validation on 23-07-24
            var POInputQtySum = await _db.ICData.Where(y => y.Id != item.Id && y.POMasterId == item.POMasterId && y.ItemNo == item.ItemNo).SumAsync(x => x.ICInputQty);

            //Why (ICInputQty + item.ICInputQty) if I am going to replace icinput qty to item.ICInputQty then should not add check
            //if (item.POQty < (POInputQty - (ICInputQty + item.ICInputQty)))
            //{
            //    throw new UserFriendlyException(400, "Input quantity cannot be greater than total POQty.");
            //}
            if (item.POQty < (POInputQtySum + item.ICInputQty))
            {
                throw new UserFriendlyException(400, "Input quantity cannot be greater than total POQty.");
            }

            if (UserRole == null)
            {
                throw new UserFriendlyException(405, "Invalid User.");
            }
            //no need as in IC hzl and oec both can do final approve
            //if (UserRole == ClientRoles.oec.ToString())
            //{
            //    item.Status = (int)ICReqStatus.OECApprove;
            //}

            var updateItemObj = editObj.ICData.Where(p => p.Id == item.Id && p.InspectionClearanceId == input.Id).FirstOrDefault();


            if (UserRole == ClientRoles.vendor.ToString())
            {
                //if vendor cannot update after request creation then need to comment this code
                updateItemObj.ICInputQty = item.ICInputQty;
                updateItemObj.Make = item.Make;
                updateItemObj.Model = item.Model;
                updateItemObj.PartNo = item.PartNo;

                //_db.ICData
                //   .Where(p => p.Id == item.Id && p.InspectionClearanceId == input.Id)
                //   .ToList()
                //   .ForEach(x =>
                //   {
                //       x.ICInputQty = item.ICInputQty;
                //   });
            }
            else
            {
                updateItemObj.ICInputQty = item.ICInputQty;
                updateItemObj.Make = item.Make;
                updateItemObj.Model = item.Model;
                updateItemObj.PartNo = item.PartNo;
                updateItemObj.Status = item.Status;
                updateItemObj.StatusValue = ((ICReqStatus)item.Status).ToString();

                if (item.MaterialClass != null)
                {
                    updateItemObj.MaterialClass = item.MaterialClass;
                    updateItemObj.MaterialClassValue = ((POMaterialClass)item.MaterialClass).ToString();

                    //No need of loop correction needed
                    //_db.ICData
                    // .Where(p => p.Id == item.Id && p.InspectionClearanceId == input.Id)
                    // .ToList()
                    // .ForEach(x =>
                    // {
                    //     x.ICInputQty = item.ICInputQty;
                    //     x.Status = item.Status;
                    //     x.StatusValue = ((ICReqStatus)item.Status).ToString();
                    //     x.MaterialClass = item.MaterialClass;
                    //     x.MaterialClassValue = ((POMaterialClass)item.MaterialClass).ToString();
                    // });
                }
                else
                {
                    //updateItemObj.Status = item.Status;
                    //updateItemObj.StatusValue = ((ICReqStatus)item.Status).ToString();
                    //No need of loop correction needed
                    // _db.ICData
                    //.Where(p => p.Id == item.Id && p.InspectionClearanceId == input.Id)
                    //.ToList()
                    //.ForEach(x =>
                    //{
                    //    x.ICInputQty = item.ICInputQty;
                    //    x.Status = item.Status;
                    //    x.StatusValue = ((ICReqStatus)item.Status).ToString();
                    //});
                }

                if (item.Status == (int)ICReqStatus.Approve)
                {
                    //29feb
                    updateItemObj.Approver = sessionUserName;
                    updateItemObj.ApprovedOn = currentDateTime;
                    isApproveAction = true;
                    if(item.ICInputQty > 0)
                    {
                        SAPICItem sAPICItem = new()
                        {
                            ITEM_NO = item.ItemNo,
                            PO_QUAN = item.POQty.ToString(),
                            MATERIAL_NO = item.MaterialNo,
                            APP_IC_QTY = item.ICInputQty.ToString(),
                            APP_EMP_ID = _abpSession.UserId.ToString(),
                            APP_DATE_TIME = currentDateTime.ToString(),
                            APP_REMARK = input.UserActionRemark
                        };

                        sAPICEdit.IC_DATA.item.Add(sAPICItem);
                    }
                }
                else if (item.Status == (int)ICReqStatus.Refer)
                {
                    updateItemObj.Referrer = sessionUserName;
                    updateItemObj.ReferedOn = currentDateTime;
                }
                else if (item.Status == (int)ICReqStatus.Reject)
                {
                    updateItemObj.Rejecter = sessionUserName;
                    updateItemObj.RejectedOn = currentDateTime;
                }
                else
                {

                }
            }
        }

        bool isSAPUpdateSuccess = false;
        if (isApproveAction)
        {
            isSAPUpdateSuccess = await UpdateSAPICDataAsync(sAPICEdit);
            if (!isSAPUpdateSuccess)
            {
                throw new UserFriendlyException("Action Failed.");
            }
        }
        //need to check editObj works no need to icObj 
        //var icObj = _db.InspectionClearance
        //      .Where(p => p.Id == input.Id).SingleOrDefault();
        int actionStatus = input.ICItems.FirstOrDefault().Status.Value;
        editObj.Status = actionStatus;

        if (UserRole == ClientRoles.oec.ToString())
        {
            if (isApproveAction)
            {
                editObj.IsApprovedByOEC = true;
            }
            editObj.OECActionRemark = input.OECActionRemark;
        }
        else if (UserRole == ClientRoles.vendor.ToString())
        {
            editObj.VendorRemark = input.VendorRemark;
        }
        else if (UserRole == ClientRoles.hzl.ToString() || UserRole == ClientRoles.admin.ToString())
        {
            editObj.UserActionRemark = input.UserActionRemark;
        }
        else
        {

        }
        await _db.SaveChangesAsync();

        try
        {
            //check linq gives correct result or not
            var listEmails = await _db.SAPUserPOMap.Where(x => x.PONo == editObj.ICData.FirstOrDefault().PONo).Select(p => p.User.EmailAddress).Distinct().ToListAsync();
            if (listEmails.Any())
            {
                string csvString = string.Join(",", listEmails);

                try
                {
                    await _sendEmail.ExecuteSMTPAsync(csvString, "Request Modified", "Request ID (" + input.Id + ")  has been updated by "+ sessionUserName + ". Please review this on IPMS portal", CancellationToken.None);
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
        return input;
    }

    public async Task<List<InspectionClearance>> GetAllReqItemsAsync(PagedICResultRequestDto input)
    {
        List<InspectionClearance> resObj = new();
        User user = _db.Users.Where(x => x.Id == _abpSession.UserId).SingleOrDefault();
        IList<string> userRoles = _userManager.GetRolesAsync(user).Result;
        if (userRoles != null && userRoles.Count > 0)
        {
            var listPO = _db.SAPUserPOMap.Where(x => x.UserId == _abpSession.UserId).Select(x => x.PONo).Distinct().ToList();

            if (userRoles.Any(str => str.ToLower().Contains("vendor")))
            {
                resObj = await _db.InspectionClearance.Include(y => y.ICData)
                                 .ThenInclude(p => p.POMaster)
                                 .Where(x => x.Id > 0 && (x.CreatorUserId == _abpSession.UserId || x.ICData.Any(f => f.PONo != null && listPO.Contains(f.PONo)))).ToListAsync();
            }
            else if (userRoles.Any(str => str.ToLower().Contains("admin")))
            {
                resObj = await _db.InspectionClearance.Include(y => y.ICData)
                                 .ThenInclude(p => p.POMaster)
                                 .Where(x => x.Id > 0).ToListAsync();
            }
            else
            {
                //logic needs to be check
                resObj = await _db.InspectionClearance.Include(y => y.ICData)
                                 .ThenInclude(p => p.POMaster)
                                 .Where(x => x.Id > 0 && x.ICData.Any(f => f.PONo != null && listPO.Contains(f.PONo))).ToListAsync();

                // var result = lista.Where(a => listb.Any(b => string.Compare(a, b, true) == 0));

            }
        }
        return resObj;
    }
    public async Task<InspectionClearance> GetReqItemsAsync(long id)
    {
        InspectionClearance resObj = new();
        User user = _db.Users.Where(x => x.Id == _abpSession.UserId).SingleOrDefault();
        IList<string> userRoles = _userManager.GetRolesAsync(user).Result;
        if (userRoles != null && userRoles.Count > 0)
        {
            var listPO = _db.SAPUserPOMap.Where(x => x.UserId == _abpSession.UserId).Select(x => x.PONo).Distinct().ToList();

            if (userRoles.Any(str => str.ToLower().Contains("vendor")))
            {
                resObj = await _db.InspectionClearance
                                 .Include(y => y.ICData).ThenInclude(y => y.ICDocuments)
                                 .Include(y => y.ICData).ThenInclude(p => p.POMaster)
                                 .Where(x => x.Id == id && (x.CreatorUserId == _abpSession.UserId || x.ICData.Any(f => f.PONo != null && listPO.Contains(f.PONo)))).SingleOrDefaultAsync();
            }
            else if (userRoles.Any(str => str.ToLower().Contains("admin")))
            {
                resObj = await _db.InspectionClearance
                                  .Include(y => y.ICData).ThenInclude(p => p.POMaster)
                                  .Include(y => y.ICData).ThenInclude(y => y.ICDocuments)
                                  .Where(x => x.Id == id).SingleOrDefaultAsync();
            }
            else
            {
                //logic needs to be check
                resObj = await _db.InspectionClearance.Include(y => y.ICData)
                                 .ThenInclude(p => p.POMaster)
                                 .Include(y => y.ICData).ThenInclude(y => y.ICDocuments)
                                 .Where(x => x.Id == id && x.ICData.Any(f => f.PONo != null && listPO.Contains(f.PONo))).SingleOrDefaultAsync();

                // var result = lista.Where(a => listb.Any(b => string.Compare(a, b, true) == 0));

            }
        }
        return resObj;
    }
    public async Task<float> GetICPreviousQtyAsync(string poNo, string itemNo, CancellationToken cancellationToken)
    {
        //var resObj = await _db.ICData.Where(y => y.PONo == poNo).Select(x => x.ICPreviousQty).ToListAsync();
        //need to check qty logic
        var resObj = await _db.ICData.Where(y => y.PONo == poNo && y.ItemNo == itemNo && y.Status != (int)ICReqStatus.Reject).SumAsync(x => x.ICInputQty);
        return resObj;
    }
    public async Task<bool> UpdateSAPICDataAsync(SAPICEdit input)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                //POSAPResponse sapPOResObj = new();

                httpClient.BaseAddress = new Uri(_sapSettings.SAPICUrl);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(_sapSettings.Username + ":" + _sapSettings.Password)));

                StringContent content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(_sapSettings.SAPICUrl, content).ConfigureAwait(false);

                string apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();
                ICEditResponse objList = JsonConvert.DeserializeObject<ICEditResponse>(apiResponse);

                if (objList != null && objList.MESSAGE_CODE == "200")
                {
                    return true;
                }
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
    public async Task<bool> IsValidUser()
    {
        //need to improve
        User user = _db.Users.Where(x => x.Id == _abpSession.UserId).SingleOrDefault();
        IList<string> userRoles = await _userManager.GetRolesAsync(user);
        if (userRoles != null && userRoles.Count > 0)
        {
            if (userRoles.Any(str => str.ToLower().Contains("admin")) || userRoles.Any(str => str.ToLower().Contains("vendor")))
                return true;
            else
                return false;
        }
        return false;
    }

    //public string GetUserRoleName()
    //{
    //    //need to improve
    //    User user = _db.Users.Where(x => x.Id == _abpSession.UserId).SingleOrDefault();
    //    IList<string> userRoles = _userManager.GetRolesAsync(user).Result;
    //    if (userRoles != null && userRoles.Count > 0)
    //    {
    //        if (userRoles.Any(str => str.ToLower().Contains("vendor")))
    //            return "vendor";
    //        else if (userRoles.Any(str => str.ToLower().Contains("admin")))
    //            return "admin";
    //        else if (userRoles.Any(str => str.ToLower().Contains("hzl")))
    //            return "hzl";
    //        else
    //            return "oec";

    //    }
    //    return null;
    //}
}

