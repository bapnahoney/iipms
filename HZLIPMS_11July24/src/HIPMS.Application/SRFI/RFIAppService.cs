using Abp.Authorization.Users;
using Abp.Runtime.Session;
using Abp.UI;
using HIPMS.Authorization.NCR;
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
using HIPMS.SRFI.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.SRFI;

public class RFIAppService : IRFIAppService
{
    //private readonly IAbpSession _abpSession;
    private readonly HIPMSDbContext _db;
    private readonly IPOService _iPOService;
    private readonly INCRService _iNCRService;
    private readonly IAbpSession _abpSession;
    private readonly UserManager _userManager;
    private readonly IRoleAppService _roleAppService;
    private readonly SAPSettingsOptions _sapSettings;
    private readonly ISendEmail _sendEmail;
    public RFIAppService(HIPMSDbContext db, IPOService iPOService, INCRService iNCRService,
                         IAbpSession abpSession, UserManager userManager,
                         IRoleAppService roleAppService, IOptions<SAPSettingsOptions> sapSettings, ISendEmail sendEmail)
    {
        _db = db;
        _abpSession = abpSession;
        _iPOService = iPOService;
        _userManager = userManager;
        _roleAppService = roleAppService;
        _sapSettings = sapSettings.Value;
        _iNCRService = iNCRService;
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

        //this section only mapped po can view by vendor
        var rolename = await _roleAppService.GetUserRoleName();
        if (rolename == ClientRoles.vendor.ToString())
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
            if (obj != null)
            {
               ResponseModel resObj =  ((HIPMS.Services.MainResponse)obj).Response;
                if (resObj.Code == 200)
                {
                    pOResponse = await _iPOService.GetPODetailAsync(pOResultRequestDto);
                    return pOResponse;
                }
                else
                {
                    throw new UserFriendlyException("Unable to fetch PO detail.");
                }
            }
            else
            { throw new UserFriendlyException("Unable to fetch PO detail."); }

            //ResponseModel resObj = ((HIPMS.Services.ResponseDataModel)obj).Response;//JsonConvert.DeserializeObject<ResponseDataModel>(obj);
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException("Unable to fetch PO detail.");
        }

        //}
        //else
        //{
        //    return pOResponse;
        //}
    }

    public async Task<bool> Update()
    {
        throw new UserFriendlyException("Unable to fetch PO detail.");
    }
    public async Task<CreateRFIRequestDto> CreateAsync(CreateRFIRequestDto input, CancellationToken cancellationToken)
    {
        if (input == null)
        {
            throw new UserFriendlyException(HIPMSErrorConst.NotNull);
        }
        if (string.IsNullOrEmpty(input.PONo))
        {
            input.PONo = input.RFIItems.FirstOrDefault().PONo;
        }
        if (input.RFIItems == null || input.RFIItems.Count <= 0)
        {
            throw new UserFriendlyException("PO items not found");
        }

        var sum = input.RFIItems.Sum(x => x.InputQty);
        if(sum <= 0)
        {
            throw new UserFriendlyException("Atleast one input quantity must be greater than 0.");
        }

        RFI dbObj = new()
        {
            VendorName = input.VendorName,
            VendorNo = input.VendorNo,
            VendorRemark = input.VendorRemark,
            POMasterId = input.RFIItems[0].POMasterId,
            //ManufacturerName = input.ManufacturerName,
            //ManufacturerPlantAddress = input.ManufacturerPlantAddress,
            CreatorUserId = _abpSession.UserId,
            CreationTime = DateTime.Now,
            Status = (int)RFIReqStatus.Pending,

            //ICData = input.ICItems,
        };
        foreach (var item in input.RFIItems)
        {
            if (item.InputQty < 0 || item.BalanceQty < 0 || item.PreviousQty < 0 || item.POQty < 0)
            {
                throw new UserFriendlyException("Invalid Quantity.");
            }
            if (item.InputQty > item.BalanceQty)
            {
                throw new UserFriendlyException("Input quantity must be less then or equal to balance quantity.");
            }

            if (!string.IsNullOrEmpty(input.PONo))
                item.PONo = input.PONo;
            dbObj.RFIData.Add(item);
        }
        await _db.RFI.AddAsync(dbObj);
        _db.SaveChanges();
        try
        {
            var listEmails = await _db.SAPUserPOMap.Where(x => x.PONo == input.PONo).Select(p => p.User.EmailAddress).Distinct().ToListAsync();
            if (listEmails.Any())
            {
                string csvString = string.Join(",", listEmails);

                try
                {
                    await _sendEmail.ExecuteSMTPAsync(csvString, "New service request", "You recieve new service inspection request from " + input.VendorName + " . PO: " + input.PONo, CancellationToken.None);
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
    public async Task<RFIEditRequest> UpdateAsync(RFIEditRequest input, ClaimsPrincipal usrObj, CancellationToken cancellationToken)
    {
        RFI editObj = await GetReqItemsAsync(input.Id);

        if (editObj == null)
        {
            throw new UserFriendlyException(HIPMSErrorConst.NotNull);
        }
        if (editObj.Status == (int)RFIReqStatus.Approve)
        {
            throw new UserFriendlyException("User cannot edit approved request");
        }
        if (input.RFIItems == null || input.RFIItems.Count <= 0)
        {
            throw new UserFriendlyException("PO items not found");
        }
        var sum = input.RFIItems.Sum(x => x.InputQty);
        if (sum <= 0)
        {
            throw new UserFriendlyException("Atleast one input quantity must be greater than 0.");
        }

        string UserRole = await _roleAppService.GetUserRoleName();
        if (UserRole == null)
        {
            throw new UserFriendlyException("User do not have sufficient permission.");
        }

        PagedPOResultRequestDto inp = new();
        //need to check should not blank PO and RFIid
        inp.PONo = input.RFIItems.FirstOrDefault().PONo;
        inp.EditId = input.RFIItems.FirstOrDefault().RFIId;

        if ((UserRole == ClientRoles.hzl.ToString() || UserRole == ClientRoles.admin.ToString()) && input.Status == (int)RFIReqStatus.Approve)
        {
          await  _iPOService.GetPODetailFromSAPV2OnApproveAsync(inp,cancellationToken);
        }
          
        bool isApproveAction = false;
        bool isOECApproveAction = false;
        DateTime currentDateTime = DateTime.Now;
        string sessionUserName = _userManager.GetUserName(usrObj);

        SAPRFIEdit sAPRFIEdit = new()
        {
            PO_NO = editObj.RFIData.FirstOrDefault().PONo,// "8510001294",// "8510001294",// Convert.ToString(input.PONo),
            AUTH_KEY = _sapSettings.Authkey,// "MME5MI9OCDN5VERHD1C0MZYZRVH4",ConfigurationManager.AppSettings["SAPAuthKey"]
            VENDOR_NO = editObj.VendorNo,
            NOTE = editObj.VendorRemark,
            RFI_APP_DATE = currentDateTime.ToString(),
            RFI_NO = input.Id,
            RFI_SUBMIT_DATE = currentDateTime.ToString()
        };
    
        //var rfiObj = _db.RFI
        //   .Where(p => p.Id == input.Id).SingleOrDefault();

        if (UserRole == ClientRoles.oec.ToString())
        {
            editObj.OECActionRemark = input.OECActionRemark;

            if (input.Status == (int)RFIReqStatus.Approve)
            {
                editObj.IsApprovedByOEC = true;
                editObj.Status = (int)RFIReqStatus.OECApprove;
                isOECApproveAction = true;
            }
            else
            {
                editObj.Status = input.Status;
            }
        }
        else
        {
            editObj.Status = input.Status;
        }

        foreach (var item in input.RFIItems)
        {
            //todo first check data comes in service qty / po qty logic not correct
            if (item.InputQty < 0 || item.BalanceQty < 0 || item.PreviousQty < 0 || item.POQty < 0)
            {
                throw new UserFriendlyException("Invalid Quantity.");
            }
            if (item.InputQty > item.BalanceQty)
            {
                throw new UserFriendlyException(400, "BalanceQuantity");
            }


            //Commented on 23-07-2024 because add condition( y.Id != item.Id) in next query which already exclude below qty
            // var RFIInputQty = await _db.RFIData.Where(y => y.Id == item.Id).Select(y => y.InputQty).SingleOrDefaultAsync();

            var POInputQtySum = await _db.RFIData.Where(y => y.Id != item.Id && y.POMasterId == item.POMasterId && y.ItemNo == item.ItemNo && y.ServiceNo == item.ServiceNo).SumAsync(x => x.InputQty);

            //Commented on 22-07-2024
            //if (item.POQty < (POInputQty - (RFIInputQty + item.InputQty)))
            //{
            //    throw new UserFriendlyException(400, "Input quantity cannot be greater than total POQty.");
            //}

            if (item.ServiceQty < (POInputQtySum + item.InputQty))
            {
                //throw new UserFriendlyException(400, "Input quantity cannot be greater than total POQty.");
                throw new UserFriendlyException("Input quantity cannot be greater than total POQty.");

            }

            if (editObj.IsApprovedByOEC && isOECApproveAction)
            {
                item.Status = (int)RFIReqStatus.OECApprove;
            }
            var updateItemObj = editObj.RFIData.Where(p => p.Id == item.Id && p.RFIId == input.Id).FirstOrDefault();

            if (UserRole == ClientRoles.vendor.ToString())
            {
                updateItemObj.InputQty = item.InputQty;
                //_db.RFIData
                //   .Where(p => p.Id == item.Id && p.RFIId == input.Id)
                //   .ToList()
                //   .ForEach(x =>
                //   {
                //       x.InputQty = item.InputQty;
                //   });
            }
            else
            {
                updateItemObj.InputQty = item.InputQty;
                updateItemObj.Status = item.Status;
                updateItemObj.StatusValue = ((RFIReqStatus)item.Status).ToString();
                //No need of loop correction needed
                // _db.RFIData
                //.Where(p => p.Id == item.Id && p.RFIId == input.Id)
                //.ToList()
                //.ForEach(x =>
                //{
                //    x.InputQty = item.InputQty;
                //    x.Status = item.Status;
                //    x.StatusValue = ((RFIReqStatus)item.Status).ToString();
                //});

                if (item.Status == (int)RFIReqStatus.Approve)
                {
                    updateItemObj.Approver = sessionUserName;
                    updateItemObj.ApprovedOn = currentDateTime;
                    isApproveAction = true;

                    if (item.InputQty > 0)
                    {
                        SAPRFIItem sAPRFIItem = new()
                        {
                            ITEM_NO = item.ItemNo,
                            PO_QUAN = item.POQty.ToString(),
                            MATERIAL_NO = item.MaterialNo,
                            APP_RFI_QTY = item.InputQty.ToString(),
                            APP_EMP_ID = _abpSession.UserId.ToString(),
                            APP_DATE_TIME = currentDateTime.ToString(),
                            APP_REMARK = input.UserActionRemark,
                            SERVICE_NO = item.ServiceNo,
                            SERVICE_DESC = item.ServiceDescription,
                            SERVICE_QUAN = item.ServiceQty.ToString(),
                            SERVICE_UOM = item.ServiceUOM
                        };
                        sAPRFIEdit.T_RFI_DATA.item.Add(sAPRFIItem);
                    }
                }
                else if (item.Status == (int)RFIReqStatus.Refer)
                {
                    updateItemObj.Referrer = sessionUserName;
                    updateItemObj.ReferedOn = currentDateTime;
                }
                else if (item.Status == (int)RFIReqStatus.Reject)
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
            isSAPUpdateSuccess = await UpdateSAPRFIDataAsync(sAPRFIEdit);
            if (!isSAPUpdateSuccess)
            {
                throw new UserFriendlyException("Action Failed.");
            }
        }
        //if (item.Status == (int)RFIReqStatus.OECApprove && UserRole == ClientRoles.oec.ToString())
        //{
        //    rfiObj.IsApprovedByOEC = true;
        //    rfiObj.OECActionRemark = input.OECActionRemark;
        //}

        if (UserRole == ClientRoles.vendor.ToString())
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
        input.OpenNCR = await GetOpenNCRCount(editObj.POMasterId);
        await _db.SaveChangesAsync();

        try
        {
            //check linq gives correct result or not
            var listEmails = await _db.SAPUserPOMap.Where(x => x.PONo == editObj.RFIData.FirstOrDefault().PONo).Select(p => p.User.EmailAddress).Distinct().ToListAsync();
            if (listEmails.Any())
            {
                string csvString = string.Join(",", listEmails);

                try
                {
                    await _sendEmail.ExecuteSMTPAsync(csvString, "Request Modified", "Request ID (" + input.Id + ")  has been updated by " + sessionUserName + ". Please review this on IPMS portal", CancellationToken.None);
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

    public async Task<RFIEditRequest> RejectAsync(RFIEditRequest input, ClaimsPrincipal usrObj, CancellationToken cancellationToken)
    {
        RFI editObj = await GetReqItemsAsync(input.Id);

        if (editObj == null)
        {
            throw new UserFriendlyException(HIPMSErrorConst.NotNull);
        }
        if (input.RFIItems == null || input.RFIItems.Count <= 0)
        {
            throw new UserFriendlyException("PO items not found");
        }
       
        DateTime currentDateTime = DateTime.Now;
        string sessionUserName = _userManager.GetUserName(usrObj);

       
        string UserRole = await _roleAppService.GetUserRoleName();
        if (UserRole == null)
        {
            throw new UserFriendlyException("User do not have sufficient permission.");
        }
        //var rfiObj = _db.RFI
        //   .Where(p => p.Id == input.Id).SingleOrDefault();

        if (UserRole == ClientRoles.oec.ToString())
        {
            editObj.OECActionRemark = input.OECActionRemark;
        }
        editObj.Status = input.Status;

        foreach (var item in input.RFIItems)
        {
            var updateItemObj = editObj.RFIData.Where(p => p.Id == item.Id && p.RFIId == input.Id).FirstOrDefault();
            
            if (UserRole == ClientRoles.vendor.ToString())
            {
                
            }
            else
            {
                updateItemObj.Status = item.Status;
                updateItemObj.StatusValue = ((RFIReqStatus)item.Status).ToString();
                //No need of loop correction needed
                // _db.RFIData
                //.Where(p => p.Id == item.Id && p.RFIId == input.Id)
                //.ToList()
                //.ForEach(x =>
                //{
                //    x.InputQty = item.InputQty;
                //    x.Status = item.Status;
                //    x.StatusValue = ((RFIReqStatus)item.Status).ToString();
                //});

                
                if (item.Status == (int)RFIReqStatus.Reject)
                {
                    updateItemObj.Rejecter = sessionUserName;
                    updateItemObj.RejectedOn = currentDateTime;
                }
                else
                {

                }
            }

        }

        if (UserRole == ClientRoles.vendor.ToString())
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
        return input;
    }


    public async Task<OpenNCROnPO> GetOpenNCRCount(long POMasterId)
    {
        OpenNCROnPO openNCROnPO = new OpenNCROnPO();

        DateTime enddate = DateTime.Today;
        DateTime startdate = DateTime.Today.AddDays(-7);
        openNCROnPO.Last7days = await _db.NCRMaster
                           .Where(x => x.POMasterId == POMasterId && (x.Status == (int)NCRStatus.OPEN || x.Status == (int)NCRStatus.UPDATE)
                            && (x.CreationTime.Date > startdate.Date && x.CreationTime.Date <= enddate.Date)).CountAsync();

        enddate = DateTime.Today.AddDays(-7);
        startdate = DateTime.Today.AddDays(-30);

        openNCROnPO.Last7To30days = await _db.NCRMaster
                          .Where(x => x.POMasterId == POMasterId && (x.Status == (int)NCRStatus.OPEN || x.Status == (int)NCRStatus.UPDATE)
                           && (x.CreationTime.Date > startdate.Date && x.CreationTime.Date < enddate.Date)).CountAsync();

        openNCROnPO.Befor30Days = await _db.NCRMaster
                           .Where(x => x.POMasterId == POMasterId && (x.Status == (int)NCRStatus.OPEN || x.Status == (int)NCRStatus.UPDATE)
                            && x.CreationTime.Date < startdate.Date).CountAsync();
        return openNCROnPO;

    }
    public async Task<bool> UpdateSAPRFIDataAsync(SAPRFIEdit input)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                //POSAPResponse sapPOResObj = new();

                httpClient.BaseAddress = new Uri(_sapSettings.SAPRFIUrl);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(_sapSettings.Username + ":" + _sapSettings.Password)));

                StringContent content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(_sapSettings.SAPRFIUrl, content).ConfigureAwait(false);

                string apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();
                RFIEditResponse objList = JsonConvert.DeserializeObject<RFIEditResponse>(apiResponse);

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

    public async Task<List<RFI>> GetAllReqItemsAsync(PagedRFIResultRequestDto input)
    {
        List<RFI> resObj = new();
        User user = _db.Users.Where(x => x.Id == _abpSession.UserId).SingleOrDefault();
        IList<string> userRoles = _userManager.GetRolesAsync(user).Result;
        if (userRoles != null && userRoles.Count > 0)
        {
            var listPO = _db.SAPUserPOMap.Where(x => x.UserId == _abpSession.UserId).Select(x => x.PONo).Distinct().ToList();

            if (userRoles.Any(str => str.ToLower().Contains("vendor")))
            {

                resObj = await _db.RFI.Include(y => y.RFIData)
                               .ThenInclude(p => p.POMaster)
                               .Where(x => x.Id > 0 && (x.CreatorUserId == _abpSession.UserId || x.RFIData.Any(f => f.PONo != null && listPO.Contains(f.PONo)))).ToListAsync();

            }
            else if (userRoles.Any(str => str.ToLower().Contains("admin")))
            {
                resObj = await _db.RFI.Include(y => y.RFIData)
                                 .ThenInclude(p => p.POMaster)
                                 .Where(x => x.Id > 0).ToListAsync();
            }
            else
            {
                //logic needs to be check
                //var listPO = _db.SAPUserPOMap.Where(x => x.UserId == _abpSession.UserId).Select(x => x.PONo).Distinct().ToList();
                resObj = await _db.RFI.Include(y => y.RFIData)
                                 .ThenInclude(p => p.POMaster)
                                 .Where(x => x.Id > 0 && x.RFIData.Any(f => f.PONo != null && listPO.Contains(f.PONo))).ToListAsync();

                // var result = lista.Where(a => listb.Any(b => string.Compare(a, b, true) == 0));

            }
        }
        return resObj;

    }
    public async Task<RFI> GetReqItemsAsync(long id)
    {

        RFI resObj = new();
        User user = _db.Users.Where(x => x.Id == _abpSession.UserId).SingleOrDefault();
        IList<string> userRoles = _userManager.GetRolesAsync(user).Result;
        if (userRoles != null && userRoles.Count > 0)
        {
            var listPO = _db.SAPUserPOMap.Where(x => x.UserId == _abpSession.UserId).Select(x => x.PONo).Distinct().ToList();

            if (userRoles.Any(str => str.ToLower().Contains("vendor")))
            {
                resObj = await _db.RFI.Include(y => y.RFIData)
                                 .ThenInclude(p => p.POMaster)
                                 .Include(y => y.RFIData)
                                 .ThenInclude(p => p.RFIDocuments)
                                 .Where(x => x.Id == id && (x.CreatorUserId == _abpSession.UserId || x.RFIData.Any(f => f.PONo != null && listPO.Contains(f.PONo)))).SingleOrDefaultAsync();
            }
            else if (userRoles.Any(str => str.ToLower().Contains("admin")))
            {
                resObj = await _db.RFI.Include(y => y.RFIData)
                               .ThenInclude(p => p.POMaster)
                               .Include(y => y.RFIData)
                               .ThenInclude(p => p.RFIDocuments)
                               .Where(x => x.Id == id).SingleOrDefaultAsync();
            }
            else
            {
                //logic needs to be check
                resObj = await _db.RFI.Include(y => y.RFIData)
                                 .ThenInclude(p => p.POMaster)
                                 .Include(y => y.RFIData)
                                 .ThenInclude(p => p.RFIDocuments)
                                 .Where(x => x.Id == id && x.RFIData.Any(f => f.PONo != null && listPO.Contains(f.PONo))).SingleOrDefaultAsync();

                // var result = lista.Where(a => listb.Any(b => string.Compare(a, b, true) == 0));

            }
        }
        return resObj;

    }
    public async Task<float> GetRFIPreviousQtyAsync(string poNo, string itemNo, string serviceNo, CancellationToken cancellationToken)
    {
        //var resObj = await _db.ICData.Where(y => y.PONo == poNo).Select(x => x.ICPreviousQty).ToListAsync();
        //need to check qty logic
        var resObj = await _db.RFIData.Where(y => y.PONo == poNo && y.ItemNo == itemNo && y.ServiceNo == serviceNo && y.Status != (int)RFIReqStatus.Reject).SumAsync(x => x.InputQty);
        return resObj;
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

    //public async Task<object> UpdateSAPICDataAsync(var input, CancellationToken cancellationToken)
    //{
    //    var obj = new
    //    {
    //        PO_NO = "8510001294",// Convert.ToString(input.PONo),
    //        AUTH_KEY = "MME5MI9OCDN5VERHD1C0MZYZRVH4" //ConfigurationManager.AppSettings["SAPAuthKey"]
    //    };

    //    try
    //    {
    //        using (var httpClient = new HttpClient())
    //        {
    //            POSAPResponse sapPOResObj = new();
    //            httpClient.BaseAddress = new Uri("http://10.250.2.5:50000//RESTAdapter/Icdata/");
    //            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("hopsdm" + ":" + "Blue@1993")));

    //            StringContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
    //            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("http://10.250.2.5:50000//RESTAdapter/Icdata/", content).ConfigureAwait(false);

    //            string apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();
    //            POSAPResponse objList = JsonConvert.DeserializeObject<POSAPResponse>(apiResponse);

    //            if (!string.IsNullOrEmpty(apiResponse))
    //            {
    //                //var OutputResponse = JsonConvert.DeserializeObject(Convert.ToString(result));
    //                //SqlParameter[] parameter = new SqlParameter[] { new SqlParameter("@RepsoneJson", result), new SqlParameter("@Created_By", ""), new SqlParameter("@Session_ID", ""), new SqlParameter("@SAMbody", InputData), new SqlParameter("@PO_Number",input.PONo), new SqlParameter("@API_Name", "MCData"), new SqlParameter("@ID", "") };

    //                var spParams = new DynamicParameters();
    //                spParams.Add("@RepsoneJson", apiResponse);
    //                spParams.Add("@Created_By", DateTime.Now.ToString());
    //                spParams.Add("@SAMBody", null);
    //                spParams.Add("@PO_Number", obj.PO_NO);
    //                spParams.Add("@Flag", 1);
    //                spParams.Add("@API_Name", "");
    //                spParams.Add("@ID", null);

    //                var SPSyncPOResponse = await _dapper.GetSingle<SPSyncPOResponse>("[dbo].[API_Sync_PO]"
    //                           , spParams, cancellationToken,
    //                            commandType: CommandType.StoredProcedure);
    //                return BaseResponse.SuccessResponse(SPSyncPOResponse, 200, "Success");

    //                //DataTable dt = DU.GetDtTblBySrdPrc("API_Get_PO", parameter);

    //                //if (dt != null && dt.Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["Data"])))
    //                //{
    //                //    var data = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(dt.Rows[0]["Data"]));
    //                //    return BaseResponse.SuccessResponse(data, 200, "Success");
    //                //}
    //                //else
    //                //{
    //                //    return BaseResponse.failedResponse(500, "Failed");
    //                //}
    //            }
    //            else
    //            {
    //                return BaseResponse.failedResponse(500, "Failed");
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //        return BaseResponse.failedResponse(500, "Unable to fetch/save PO data");
    //    }

    //    //var result = Helper.Caller(obj, "POData");
    //}
}