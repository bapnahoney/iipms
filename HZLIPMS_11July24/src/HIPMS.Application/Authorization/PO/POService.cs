using Abp;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Castle.Components.DictionaryAdapter;
using Dapper;
using HIPMS.Authorization.PO.Dto;
using HIPMS.Authorization.Users;
using HIPMS.Dapper;
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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.Authorization.PO;


[AbpAuthorize(PermissionNames.Pages_IC)]
public class POService : IPOService
{
    private readonly IDapperRepository _dapper;
    private readonly IAbpSession _abpSession;
    private readonly HIPMSDbContext _db;
    private readonly SAPSettingsOptions _sapSettings;
    private readonly ISendEmail _sendEmail;
    private readonly EmailNotificationSettingsOptions _mailSettings;
    private readonly IRoleAppService _roleAppService;


    public POService(HIPMSDbContext db, IAbpSession abpSession, IDapperRepository dapper,
                     IOptions<SAPSettingsOptions> sapSettings,
                     IOptions<EmailNotificationSettingsOptions> mailSettings, IRoleAppService roleAppService,
                     ISendEmail sendEmail)
    {
        _db = db;
        _abpSession = abpSession;
        _dapper = dapper;
        _sapSettings = sapSettings.Value;
        _sendEmail = sendEmail;
        _mailSettings = mailSettings.Value;
        _roleAppService = roleAppService;
    }

    //public async Task<CreateICRequestDto> CreateAsync(CreateICRequestDto input)
    //{
    //    var abpUserId = _abpSession.UserId;
    //    var abpTeanantId = _abpSession.TenantId;

    //    if (input == null)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    InspectionClearance dbObj = new()
    //    {
    //        VendorName = input.VendorName,
    //        VendorNo = input.VendorNo,
    //        VendorRemark = input.VendorRemark,
    //        ICData = input.ICData,
    //    };
    //    await _db.InspectionClearance.AddAsync(dbObj);
    //    _db.SaveChanges();
    //    return input;
    //}

    public async Task<List<POMaster>> GetAllAsync(PagedPOResultRequestDto input)
    {
        var resObj = await _db.POMaster.Where(x => x.PONo != null)
                          .ToListAsync();
        return resObj;

    }

    public async Task<POMaster> GetPODetailAsync(POResultRequestDto input)
    {
        var resObj = await _db.POMaster.Include(y => y.POItems)
                    .Where(x => x.PONo == input.PONumber).SingleOrDefaultAsync();
        return resObj;
    }

    public Task<List<POMaster>> GetAllPODetailAsync(POResultRequestDto input)
    {
        throw new NotImplementedException();
    }

    public async Task<object> GetPODetailFromSAPAsync(PagedPOResultRequestDto input, CancellationToken cancellationToken)
    {
        //    var obj1 = new EmailNotificationSettingsOptions
        //    {
        //        SenderEmail = _mailSettings.SenderEmail,//"8510001294",// Convert.ToString(input.PONo),
        //        SenderName = _mailSettings.SenderName,
        //        Server = _mailSettings.Server,
        //        Port = _mailSettings.Port,
        //        UserName = _mailSettings.UserName,
        //        Password = _mailSettings.Password
        //    };
        // _sendEmail.ExecuteSMTPAsync(obj1, CancellationToken.None);


        var obj = new
        {
            PO_NO = input.PONo,//"8510001294",// Convert.ToString(input.PONo),
            AUTH_KEY = _sapSettings.Authkey// "MME5MI9OCDN5VERHD1C0MZYZRVH4" //ConfigurationManager.AppSettings["SAPAuthKey"]
        };

        try
        {
            using (var httpClient = new HttpClient())
            {
                //POSAPResponse sapPOResObj = new();
                httpClient.BaseAddress = new Uri(_sapSettings.SAPPOUrl);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(_sapSettings.Username + ":" + _sapSettings.Password)));

                StringContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(_sapSettings.SAPPOUrl, content);//.ConfigureAwait(false);

                string apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();
                //POSAPResponse objList = JsonConvert.DeserializeObject<POSAPResponse>(apiResponse);

                if (!string.IsNullOrEmpty(apiResponse))
                {
                    //var OutputResponse = JsonConvert.DeserializeOb
                    //ject(Convert.ToString(result));
                    //SqlParameter[] parameter = new SqlParameter[] { new SqlParameter("@RepsoneJson", result), new SqlParameter("@Created_By", ""), new SqlParameter("@Session_ID", ""), new SqlParameter("@SAMbody", InputData), new SqlParameter("@PO_Number",input.PONo), new SqlParameter("@API_Name", "MCData"), new SqlParameter("@ID", "") };

                    var spParams = new DynamicParameters();
                    spParams.Add("@RepsoneJson", apiResponse);
                    //spParams.Add("@Created_By", DateTime.Now.ToString());
                    spParams.Add("@SAMBody", null);
                    spParams.Add("@PO_Number", obj.PO_NO);
                    spParams.Add("@Flag", 1);
                    spParams.Add("@API_Name", "");
                    spParams.Add("@ID", null);

                    var SPSyncPOResponse = await _dapper.GetSingle<SPSyncPOResponse>("[dbo].[API_Sync_PO]"
                               , spParams, cancellationToken,
                                commandType: CommandType.StoredProcedure);

                    if (SPSyncPOResponse == null)
                    {
                        try {
                            POSAPResponse objList = JsonConvert.DeserializeObject<POSAPResponse>(apiResponse);
                            if (objList == null)
                            {
                                throw new UserFriendlyException("No SAP bindings found for this channel.");
                            }
                            else
                            {
                                POMaster insertObj = new POMaster();
                                insertObj.POType = objList.PO_TYPE;
                                insertObj.PONo = obj.PO_NO;
                                insertObj.VendorNo = objList.VENDOR_NO;
                                insertObj.VendorName = objList.VENDOR_NAME;
                                insertObj.ProjectName = objList.PROJECT_NAME;
                                insertObj.Plant = string.Empty;
                                insertObj.CreationTime = DateTime.Now;
                                insertObj.CreatedBySAPUser = objList.CREATED_BY == null ? string.Empty : objList.CREATED_BY;

                                await _db.POMaster.AddAsync(insertObj);
                                await _db.SaveChangesAsync();
                                if (insertObj.Id <= 0)
                                {
                                    throw new UserFriendlyException("No SAP bindings found for this channel.");
                                }

                                List<POItem> insitems = new List<POItem>();

                                foreach (var insertItem in objList.PO_DETAILS.item)
                                {
                                    POItem insertItemObj = new POItem();
                                    insertItemObj.POMasterId = insertObj.Id;
                                    insertItemObj.ItemNo = insertItem.ITEM_NO;
                                    insertItemObj.MaterialNo = insertItem.MATERIAL_NO;
                                    insertItemObj.MaterialDescription = insertItem.MATERIAL_DESC;
                                    insertItemObj.POQty = insertItem.PO_QUAN;
                                    insertItemObj.Status = (int)RFIReqStatus.Pending;//no need
                                    insertItemObj.UOM = insertItem.UOM;
                                    insertItemObj.MaterialClass = insertItem.MATERIAL_CLASS == "A" ? 1 : (insertItem.MATERIAL_CLASS == "B" ? 2 : (insertItem.MATERIAL_CLASS == "C" ? 3 : 0));
                                    insertItemObj.ServiceNo = insertItem.SERVICE_NO;
                                    insertItemObj.ServiceDescription = insertItem.SERVICE_DESC;
                                    insertItemObj.ServiceQty = insertItem.SERVICE_QUAN;
                                    insertItemObj.ServiceUOM = insertItem.SERVICE_UOM;
                                    insertItemObj.CreationTime = DateTime.Now;
                                    insertItemObj.CreatorUserId = _abpSession.UserId;
                                    insertItemObj.PONo = obj.PO_NO;

                                    await _db.POItems.AddAsync(insertItemObj);

                                }
                                await _db.SaveChangesAsync();
                            }
                        }
                        catch(Exception ex)
                        {

                        }
                        
                    }
                    return BaseResponse.SuccessResponse(SPSyncPOResponse, 200, "Success");

                    //DataTable dt = DU.GetDtTblBySrdPrc("API_Get_PO", parameter);

                    //if (dt != null && dt.Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["Data"])))
                    //{
                    //    var data = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(dt.Rows[0]["Data"]));
                    //    return BaseResponse.SuccessResponse(data, 200, "Success");
                    //}
                    //else
                    //{
                    //    return BaseResponse.failedResponse(500, "Failed");
                    //}
                }
                else
                {
                    return BaseResponse.failedResponse(500, "Failed");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            if(ex.ToString().ToLower().Contains("connected host has failed to respond"))
            {
                return BaseResponse.failedResponse(500, "Network issue.");
            }
            return BaseResponse.failedResponse(500, "Unable to fetch/save PO data");
        }

        //var result = Helper.Caller(obj, "POData");
    }

  
    public async Task<CrudUserPOMap> GetUserPODDAsync()
    {
        CrudUserPOMap resObj = new CrudUserPOMap();

        resObj.PODD.Insert(0, new PODD { Id = 0, PONumber = "<--Select-->" });
        var poObj = await _db.POMaster.Select(et => new PODD()
        {
            Id = et.Id,
            PONumber = et.PONo
        }).ToListAsync();
        resObj.PODD.AddRange(poObj);
        resObj.UserDD.Insert(0, new UserDD { Id = 0, UserEmail = "<--Select-->" });
        var userObj = await _db.Users.Select(et => new UserDD()
        {
            Id = et.Id,
            UserEmail = et.EmailAddress
        }).ToListAsync();
        resObj.UserDD.AddRange(userObj);
        return resObj;
    }
    public async Task<List<ViewUserPOMap>> GetAllMappingAsync()
    {
        List<ViewUserPOMap> resObj = new();
        var listObj = await _db.SAPUserPOMap.Include(x => x.User)
                        .Where(x => x.PONo != null).ToListAsync();
        foreach (var obj in listObj)
        {
            ViewUserPOMap temp = new();
            temp.PONo = obj.PONo;
            temp.UserEmail = obj.User.EmailAddress;
            resObj.Add(temp);
        }
        return resObj;
    }

    public async Task<List<ViewUserPOMap>> UserPOMapUpdateAsync(CrudUserPOMap reqObj)
    {
        try
        {
            var poObj = await _db.POMaster.Where(x => x.PONo == reqObj.PONo).SingleOrDefaultAsync();
            if (poObj == null)
            {
                throw new UserFriendlyException("Invalid PO.");
            }

            SAPUserPOMap resObj = new SAPUserPOMap();
            resObj.UserId = reqObj.UserId;
            resObj.POId = poObj.Id;
            resObj.IsVendor = reqObj.IsVendor;
            resObj.IsApprover = reqObj.IsApprover;
            resObj.PONo = poObj.PONo;
            resObj.VendorNo = poObj.VendorNo;//no need remove later

            await _db.SAPUserPOMap.AddAsync(resObj);
            await _db.SaveChangesAsync();

            List<ViewUserPOMap> resViewObj = new();
            var listObj = _db.SAPUserPOMap.Include(x => x.User)
                            .Where(x => x.PONo != null).ToListAsync();
            foreach (var obj in listObj.Result)
            {
                ViewUserPOMap temp = new();
                temp.PONo = obj.PONo;
                temp.UserEmail = obj.User.EmailAddress;
                resViewObj.Add(temp);
            }

            return resViewObj;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException();
        }
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
        if (roleName != ClientRoles.admin.ToString())
        {
            throw new UserFriendlyException(403, "User do not have sufficient permission.");
        }

        POMaster pOResponse = await GetPODetailAsync(pOResultRequestDto);
        if (pOResponse == null || pOResponse.Id == 0)
        {
            pOResponse = new();
            PagedPOResultRequestDto inp = new();
            inp.PONo = input.PONumber;
            object obj = await GetPODetailFromSAPAsync(inp, cancellationToken);
            try
            {
                ResponseModel resObj = ((HIPMS.Services.ResponseDataModel)obj).Response;

                if (resObj == null)
                {
                    throw new UserFriendlyException(204, "PO detail not available in SAP.");
                }
                if (resObj.Code == 200)
                {
                    pOResponse = await GetPODetailAsync(pOResultRequestDto);
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

        }
        else
        {
            return pOResponse;
        }
    }
  
    //OnSearch
    public async Task<object> GetPODetailFromSAPV2Async(PagedPOResultRequestDto input, CancellationToken cancellationToken)
    {
        var obj = new
        {
            PO_NO = input.PONo,
            AUTH_KEY = _sapSettings.Authkey
        };

        try
        {
            using (var httpClient = new HttpClient())
            {
                //POSAPResponse sapPOResObj = new();
                httpClient.BaseAddress = new Uri(_sapSettings.SAPPOUrl);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(_sapSettings.Username + ":" + _sapSettings.Password)));

                StringContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(_sapSettings.SAPPOUrl, content);//.ConfigureAwait(false);

                string apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();
                //POSAPResponse objList = JsonConvert.DeserializeObject<POSAPResponse>(apiResponse);

                if (!string.IsNullOrEmpty(apiResponse))
                {
                    try
                    {
                        //Check all possible json type (POSAPResponse/)
                        POSAPResponse objList = JsonConvert.DeserializeObject<POSAPResponse>(apiResponse);
                        if (objList == null || objList.PO_DETAILS== null || objList.PO_DETAILS.item.Count <= 0)
                        {
                            throw new UserFriendlyException("No SAP bindings found for this PO.");
                        }

                        // Fetch POMasterid 
                        var POMasterDetail = await _db.POMaster.Include(y => y.POItems).Where(x => x.PONo == obj.PO_NO)
                                               .SingleOrDefaultAsync();
                        //.Select(x => x.Id)

                        var s4hitems = objList.PO_DETAILS.item;

                        if (POMasterDetail != null && POMasterDetail.Id > 0)
                        {
                            var POmasterId = POMasterDetail.Id;
                            //check service PO
                            if (objList.PO_TYPE == POType.ZPIS.ToString() || objList.PO_TYPE == POType.ZPDS.ToString())
                            {
                                //can filter && (x.Status == (int)RFIReqStatus.Pending || x.Status == (int)RFIReqStatus.Refer)
                                var resRFIObj = await _db.RFIData
                                           .Where(x => x.POMasterId == POmasterId)//todo later filter out with && x.InputQty > 0
                                           .ToListAsync();

                                if (resRFIObj == null || resRFIObj.Count == 0)
                                {
                                    //delete existing po line item
                                    var deletePOData = await _db.POItems.Where(x => x.POMasterId == POmasterId).ToListAsync();
                                    deletePOData.RemoveAll(x=>x.POMasterId ==POmasterId);
                                   
                                    _db.SaveChanges();

                                    //insert all new records
                                    foreach (var s4hitem in s4hitems)
                                    {
                                        POItem insertItemObj = new POItem();
                                        insertItemObj.POMasterId = POmasterId;
                                        insertItemObj.ItemNo = s4hitem.ITEM_NO;
                                        insertItemObj.MaterialNo = s4hitem.MATERIAL_NO;
                                        insertItemObj.MaterialDescription = s4hitem.MATERIAL_DESC;
                                        insertItemObj.POQty = s4hitem.PO_QUAN;
                                        insertItemObj.Status = (int)RFIReqStatus.Pending;
                                        insertItemObj.UOM = s4hitem.UOM;
                                        insertItemObj.MaterialClass = 0;
                                        insertItemObj.ServiceNo = s4hitem.SERVICE_NO;
                                        insertItemObj.ServiceDescription = s4hitem.SERVICE_DESC;
                                        insertItemObj.ServiceQty = s4hitem.SERVICE_QUAN;
                                        insertItemObj.ServiceUOM = s4hitem.SERVICE_UOM;
                                        insertItemObj.CreationTime = DateTime.Now;
                                        insertItemObj.CreatorUserId = _abpSession.UserId;
                                        insertItemObj.PONo = obj.PO_NO;

                                        await _db.POItems.AddAsync(insertItemObj);
                                    }
                                    _db.SaveChanges();
                                }
                                else
                                {
                                   // POItem netPOItem = new();
                                    List<POItem> insitems = new List<POItem>();

                                    foreach (var s4hitem in s4hitems)
                                    {
                                        //insert into history table, maintain each change in history 
                                        POItem netPOItem = POMasterDetail.POItems.Where(x => x.ItemNo == s4hitem.ITEM_NO && x.ServiceNo == s4hitem.SERVICE_NO).SingleOrDefault();

                                        if (string.IsNullOrEmpty(netPOItem.ItemNo) || string.IsNullOrEmpty(netPOItem.ServiceNo))
                                        {
                                            POItem insertItemObj = new POItem();
                                            insertItemObj.POMasterId = POmasterId;
                                            insertItemObj.ItemNo = s4hitem.ITEM_NO;
                                            insertItemObj.MaterialNo = s4hitem.MATERIAL_NO;
                                            insertItemObj.MaterialDescription = s4hitem.MATERIAL_DESC;
                                            insertItemObj.POQty = s4hitem.PO_QUAN;
                                            insertItemObj.Status = (int)RFIReqStatus.Pending;
                                            insertItemObj.UOM = s4hitem.UOM;
                                            insertItemObj.MaterialClass = 0;
                                            insertItemObj.ServiceNo = s4hitem.SERVICE_NO;
                                            insertItemObj.ServiceDescription = s4hitem.SERVICE_DESC;
                                            insertItemObj.ServiceQty = s4hitem.SERVICE_QUAN;
                                            insertItemObj.ServiceUOM = s4hitem.SERVICE_UOM;
                                            insertItemObj.CreationTime = DateTime.Now;
                                            insertItemObj.CreatorUserId = _abpSession.UserId;
                                            insertItemObj.PONo = obj.PO_NO;

                                            await _db.POItems.AddAsync(insertItemObj);


                                        }
                                        else if (s4hitem.SERVICE_QUAN > netPOItem.ServiceQty)
                                        {
                                            float diff = (s4hitem.SERVICE_QUAN - netPOItem.ServiceQty) ?? 0f;

                                            var itemTobeupdate = resRFIObj.Where(w => w.ItemNo == s4hitem.ITEM_NO && w.ServiceNo == s4hitem.SERVICE_NO).ToList();
                                            foreach (var up in itemTobeupdate)
                                            {
                                                up.ServiceQty = s4hitem.SERVICE_QUAN;
                                                up.BalanceQty = up.BalanceQty + diff;
                                            }

                                            //update PO data table
                                            //check this will update or not POdata 
                                            netPOItem.ServiceQty = s4hitem.SERVICE_QUAN;
                                        }
                                        else if (s4hitem.SERVICE_QUAN < netPOItem.ServiceQty)
                                        {
                                            float diff = (netPOItem.ServiceQty - s4hitem.SERVICE_QUAN) ?? 0f;

                                            var itemTobeupdate = resRFIObj.Where(w => w.ItemNo == s4hitem.ITEM_NO && w.ServiceNo == s4hitem.SERVICE_NO).ToList();
                                            foreach (var up in itemTobeupdate)
                                            {
                                                //logic place after testing
                                                if ((up.BalanceQty - diff) < 0)
                                                {
                                                    //should not throw exception only on approve this case will throw exception
                                                    //throw new UserFriendlyException("Quantity of the item( " + s4hitem.ITEM_NO + ") has been updated in S4H.");
                                                }
                                                else
                                                {
                                                    
                                                }
                                                up.ServiceQty = s4hitem.SERVICE_QUAN;
                                                up.BalanceQty = up.BalanceQty - diff;
                                            }

                                            //update PO data table
                                            //check this will update or not POdata 
                                            netPOItem.ServiceQty = s4hitem.SERVICE_QUAN;
                                        }
                                        else
                                        { }
                              
                                        await _db.SaveChangesAsync();
                                    }
                                }
                            }

                            //For Material PO
                            else if (objList.PO_TYPE == POType.ZPDM.ToString() || objList.PO_TYPE == POType.ZPIM.ToString())
                            {
                                //can filter && (x.Status == (int)RFIReqStatus.Pending || x.Status == (int)RFIReqStatus.Refer)
                                var resICObj = await _db.ICData
                                           .Where(x => x.POMasterId == POmasterId)//todo later filter out with && x.InputQty > 0
                                           .ToListAsync();

                                if (resICObj == null || resICObj.Count == 0)
                                {
                                    //delete existing po line item
                                    var deletePOData = await _db.POItems.Where(x => x.POMasterId == POmasterId).ToListAsync();
                                    deletePOData.RemoveAll(x => x.POMasterId == POmasterId);

                                    _db.SaveChanges();

                                    //insert all new records
                                    foreach (var s4hitem in s4hitems)
                                    {
                                        POItem insertItemObj = new POItem();
                                        insertItemObj.POMasterId = POmasterId;
                                        insertItemObj.ItemNo = s4hitem.ITEM_NO;
                                        insertItemObj.MaterialNo = s4hitem.MATERIAL_NO;
                                        insertItemObj.MaterialDescription = s4hitem.MATERIAL_DESC;
                                        insertItemObj.POQty = s4hitem.PO_QUAN;
                                        insertItemObj.Status = (int)ICReqStatus.Pending;
                                        insertItemObj.UOM = s4hitem.UOM;
                                        insertItemObj.MaterialClass = s4hitem.MATERIAL_CLASS == "A" ? 1 : (s4hitem.MATERIAL_CLASS == "B" ? 2 : (s4hitem.MATERIAL_CLASS == "C" ? 3 : 0));
                                        insertItemObj.ServiceNo = s4hitem.SERVICE_NO;
                                        insertItemObj.ServiceDescription = s4hitem.SERVICE_DESC;
                                        insertItemObj.ServiceQty = s4hitem.SERVICE_QUAN;
                                        insertItemObj.ServiceUOM = s4hitem.SERVICE_UOM;
                                        insertItemObj.CreationTime = DateTime.Now;
                                        insertItemObj.CreatorUserId = _abpSession.UserId;
                                        insertItemObj.PONo = obj.PO_NO;

                                        await _db.POItems.AddAsync(insertItemObj);
                                    }
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    //POItem netPOItem = new();
                                    List<POItem> insitems = new List<POItem>();

                                    foreach (var s4hitem in s4hitems)
                                    {
                                        //insert into history table, maintain each change in history 
                                        POItem netPOItem = POMasterDetail.POItems.Where(x => x.ItemNo == s4hitem.ITEM_NO).SingleOrDefault();

                                        if (netPOItem == null || string.IsNullOrEmpty(netPOItem.ItemNo))
                                        {
                                            POItem insertItemObj = new POItem();
                                            insertItemObj.POMasterId = POmasterId;
                                            insertItemObj.ItemNo = s4hitem.ITEM_NO;
                                            insertItemObj.MaterialNo = s4hitem.MATERIAL_NO;
                                            insertItemObj.MaterialDescription = s4hitem.MATERIAL_DESC;
                                            insertItemObj.POQty = s4hitem.PO_QUAN;
                                            insertItemObj.Status = (int)RFIReqStatus.Pending;
                                            insertItemObj.UOM = s4hitem.UOM;
                                            insertItemObj.MaterialClass = s4hitem.MATERIAL_CLASS == "A" ? 1 : (s4hitem.MATERIAL_CLASS == "B" ? 2 : (s4hitem.MATERIAL_CLASS == "C" ? 3 : 0));
                                            insertItemObj.ServiceNo = s4hitem.SERVICE_NO;
                                            insertItemObj.ServiceDescription = s4hitem.SERVICE_DESC;
                                            insertItemObj.ServiceQty = s4hitem.SERVICE_QUAN;
                                            insertItemObj.ServiceUOM = s4hitem.SERVICE_UOM;
                                            insertItemObj.CreationTime = DateTime.Now;
                                            insertItemObj.CreatorUserId = _abpSession.UserId;
                                            insertItemObj.PONo = obj.PO_NO;

                                            await _db.POItems.AddAsync(insertItemObj);


                                        }
                                        else if (s4hitem.PO_QUAN > netPOItem.POQty)
                                        {
                                            //float diff = (s4hitem.PO_QUAN - netPOItem.POQty) ?? 0f;
                                            decimal? diff = s4hitem.PO_QUAN - netPOItem.POQty ;
                                            var itemTobeupdate = resICObj.Where(w => w.ItemNo == s4hitem.ITEM_NO).ToList();
                                            foreach (var up in itemTobeupdate)
                                            {
                                                up.POQty = (float)s4hitem.PO_QUAN;
                                                up.ICBalanceQty = up.ICBalanceQty + ((float)diff);
                                            }

                                            //update PO data table
                                            //check this will update or not POdata 
                                            netPOItem.POQty = s4hitem.PO_QUAN;
                                        }
                                        else if (s4hitem.PO_QUAN < netPOItem.POQty)
                                        {
                                            decimal? diff = netPOItem.POQty - s4hitem.PO_QUAN;
                                            //float diff = (netPOItem.POQty - s4hitem.PO_QUAN) ?? 0f;

                                            var itemTobeupdate = resICObj.Where(w => w.ItemNo == s4hitem.ITEM_NO).ToList();
                                            foreach (var up in itemTobeupdate)
                                            {
                                                //logic place after testing
                                                if ((up.ICBalanceQty - ((float)diff)) < 0)
                                                {
                                                    //should not throw exception only on approve this case will throw exception
                                                    throw new UserFriendlyException("Quantity of the item( " + s4hitem.ITEM_NO + ") has been updated in S4H.");
                                                }
                                                else
                                                {
                                                    up.POQty = (float)s4hitem.PO_QUAN;
                                                    up.ICBalanceQty = up.ICBalanceQty - ((float)diff);
                                                }
                                            }

                                            //update PO data table
                                            //check this will update or not POdata 
                                            netPOItem.POQty = s4hitem.PO_QUAN;
                                        }
                                        else
                                        { }

                                        await _db.SaveChangesAsync();
                                    }
                                }
                            }

                        }
                        else
                        {
                            POMaster insertObj = new POMaster();
                            insertObj.POType = objList.PO_TYPE;
                            insertObj.PONo = obj.PO_NO;
                            insertObj.VendorNo = objList.VENDOR_NO;
                            insertObj.VendorName = objList.VENDOR_NAME;
                            insertObj.ProjectName = objList.PROJECT_NAME;
                            insertObj.Plant = string.Empty;
                            insertObj.CreationTime = DateTime.Now;
                            insertObj.CreatedBySAPUser = objList.CREATED_BY == null ? string.Empty : objList.CREATED_BY;

                            await _db.POMaster.AddAsync(insertObj);
                            await _db.SaveChangesAsync();
                            if (insertObj.Id <= 0)
                            {
                                throw new UserFriendlyException("No SAP bindings found for this channel.");
                            }

                            List<POItem> insitems = new List<POItem>();

                            foreach (var insertItem in objList.PO_DETAILS.item)
                            {
                                POItem insertItemObj = new POItem();
                                insertItemObj.POMasterId = insertObj.Id;
                                insertItemObj.ItemNo = insertItem.ITEM_NO;
                                insertItemObj.MaterialNo = insertItem.MATERIAL_NO;
                                insertItemObj.MaterialDescription = insertItem.MATERIAL_DESC;
                                insertItemObj.POQty = insertItem.PO_QUAN;
                                insertItemObj.Status = (int)RFIReqStatus.Pending;
                                insertItemObj.UOM = insertItem.UOM;
                                insertItemObj.MaterialClass = insertItem.MATERIAL_CLASS == "A" ? 1 : (insertItem.MATERIAL_CLASS == "B" ? 2 : (insertItem.MATERIAL_CLASS == "C" ? 3 : 0));
                                insertItemObj.ServiceNo = insertItem.SERVICE_NO;
                                insertItemObj.ServiceDescription = insertItem.SERVICE_DESC;
                                insertItemObj.ServiceQty = insertItem.SERVICE_QUAN;
                                insertItemObj.ServiceUOM = insertItem.SERVICE_UOM;
                                insertItemObj.CreationTime = DateTime.Now;
                                insertItemObj.CreatorUserId = _abpSession.UserId;
                                insertItemObj.PONo = obj.PO_NO;

                                await _db.POItems.AddAsync(insertItemObj);

                            }
                            await _db.SaveChangesAsync();
                        }
                        return BaseResponse.SuccessResponse(200, "Success");
                    }
                    catch (Exception ex)
                    {
                        var spParams = new DynamicParameters();
                        spParams.Add("@RepsoneJson", apiResponse);
                        // spParams.Add("@Created_By", DateTime.Now.ToString());
                        spParams.Add("@SAMBody", null);
                        spParams.Add("@PO_Number", obj.PO_NO);
                        spParams.Add("@Flag", 1);
                        spParams.Add("@API_Name", "");
                        spParams.Add("@ID", null);

                        var SPSyncPOResponse = await _dapper.GetSingle<SPSyncPOResponse>("[dbo].[API_Sync_PO]"
                                   , spParams, cancellationToken,
                                    commandType: CommandType.StoredProcedure);

                        if (SPSyncPOResponse == null)
                        {
                        }
                        return BaseResponse.SuccessResponse(200, "Success");
                    }
                }
                else
                {
                    return BaseResponse.failedResponse(500, "Failed");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            if (ex.ToString().ToLower().Contains("connected host has failed to respond"))
            {
                return BaseResponse.failedResponse(500, "Network issue.");
            }
            return BaseResponse.failedResponse(500, "Unable to fetch/save PO data");
        }
    }

    //OnEdit for view 
    public async Task<object> GetPODetailFromSAPV2OnApproveAsync(PagedPOResultRequestDto input, CancellationToken cancellationToken)
    {
        var obj = new
        {
            PO_NO = input.PONo,
            AUTH_KEY = _sapSettings.Authkey
        };

        //try
        //{
            using (var httpClient = new HttpClient())
            {
                //POSAPResponse sapPOResObj = new();
                httpClient.BaseAddress = new Uri(_sapSettings.SAPPOUrl);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(_sapSettings.Username + ":" + _sapSettings.Password)));

                StringContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(_sapSettings.SAPPOUrl, content);//.ConfigureAwait(false);

                string apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();

                //POSAPResponse objList = JsonConvert.DeserializeObject<POSAPResponse>(apiResponse);

                if (!string.IsNullOrEmpty(apiResponse))
                {
                    //try
                    //{
                        POSAPResponse objList;
                        //Check all possible json type (POSAPResponse/)
                        try
                        {
                            POSAPResponse responseItemlist = JsonConvert.DeserializeObject<POSAPResponse>(apiResponse);
                            objList = responseItemlist;
                        }
                        catch (Exception ex)
                        {
                            SingleItemPOSAPResponse responseSingleItem = JsonConvert.DeserializeObject<SingleItemPOSAPResponse>(apiResponse);
                            objList = new();
                            objList.CREATED_BY = responseSingleItem.CREATED_BY;
                            objList.PO_TYPE = responseSingleItem.PO_TYPE;
                            objList.PROJECT_NAME = responseSingleItem.PROJECT_NAME;
                            objList.VENDOR_NAME = responseSingleItem.VENDOR_NAME;
                            Item itm = new Item();
                            SItem sitem = new SItem();
                            sitem = responseSingleItem.PO_DETAILS.item;
                            itm.ITEM_NO = sitem.ITEM_NO;
                            itm.MATERIAL_NO = sitem.MATERIAL_NO;
                            itm.MATERIAL_DESC = sitem.MATERIAL_DESC;
                            itm.UOM = sitem.UOM;
                            itm.PO_QUAN = sitem.PO_QUAN;
                            itm.MATERIAL_CLASS = sitem.MATERIAL_CLASS;
                            itm.SERVICE_NO = sitem.SERVICE_NO;
                            itm.SERVICE_DESC = sitem.SERVICE_DESC;
                            itm.SERVICE_QUAN = sitem.SERVICE_QUAN;
                            itm.SERVICE_UOM = sitem.SERVICE_UOM;

                            objList.PO_DETAILS.item.Add(itm);
                        }

                        if (objList == null || objList.PO_DETAILS == null || objList.PO_DETAILS.item.Count <= 0)
                        {
                            throw new UserFriendlyException("No SAP bindings found for this PO.");
                        }

                        // Fetch POMasterid 
                        var POMasterDetail = await _db.POMaster.Include(y => y.POItems).Where(x => x.PONo == obj.PO_NO)
                                               .SingleOrDefaultAsync();
                        //.Select(x => x.Id)

                        var s4hitems = objList.PO_DETAILS.item;

                        if (POMasterDetail != null && POMasterDetail.Id > 0)
                        {
                            var POmasterId = POMasterDetail.Id;
                            //check service PO
                            if (objList.PO_TYPE == POType.ZPIS.ToString() || objList.PO_TYPE == POType.ZPDS.ToString())
                            {
                                //can filter && (x.Status == (int)RFIReqStatus.Pending || x.Status == (int)RFIReqStatus.Refer)
                                var resRFIObj = await _db.RFIData
                                           .Where(x => x.POMasterId == POmasterId)//todo later filter out with && x.InputQty > 0
                                           .ToListAsync();

                                if (resRFIObj == null || resRFIObj.Count == 0)
                                {
                                    //delete existing po line item
                                    var deletePOData = await _db.POItems.Where(x => x.POMasterId == POmasterId).ToListAsync();
                                    deletePOData.RemoveAll(x => x.POMasterId == POmasterId);

                                    _db.SaveChanges();

                                    //insert all new records
                                    foreach (var s4hitem in s4hitems)
                                    {
                                        POItem insertItemObj = new POItem();
                                        insertItemObj.POMasterId = POmasterId;
                                        insertItemObj.ItemNo = s4hitem.ITEM_NO;
                                        insertItemObj.MaterialNo = s4hitem.MATERIAL_NO;
                                        insertItemObj.MaterialDescription = s4hitem.MATERIAL_DESC;
                                        insertItemObj.POQty = s4hitem.PO_QUAN;
                                        insertItemObj.Status = (int)RFIReqStatus.Pending;
                                        insertItemObj.UOM = s4hitem.UOM;
                                        insertItemObj.MaterialClass = 0;
                                        insertItemObj.ServiceNo = s4hitem.SERVICE_NO;
                                        insertItemObj.ServiceDescription = s4hitem.SERVICE_DESC;
                                        insertItemObj.ServiceQty = s4hitem.SERVICE_QUAN;
                                        insertItemObj.ServiceUOM = s4hitem.SERVICE_UOM;
                                        insertItemObj.CreationTime = DateTime.Now;
                                        insertItemObj.CreatorUserId = _abpSession.UserId;
                                        insertItemObj.PONo = obj.PO_NO;

                                        await _db.POItems.AddAsync(insertItemObj);
                                    }
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    POItem netPOItem = new();
                                    List<POItem> insitems = new List<POItem>();

                                    foreach (var s4hitem in s4hitems)
                                    {
                                        //insert into history table, maintain each change in history 
                                        netPOItem = POMasterDetail.POItems.Where(x => x.ItemNo == s4hitem.ITEM_NO && x.ServiceNo == s4hitem.SERVICE_NO).SingleOrDefault();

                                        if (netPOItem == null || string.IsNullOrEmpty(netPOItem.ItemNo) || string.IsNullOrEmpty(netPOItem.ServiceNo))
                                        {
                                            POItem insertItemObj = new POItem();
                                            insertItemObj.POMasterId = POmasterId;
                                            insertItemObj.ItemNo = s4hitem.ITEM_NO;
                                            insertItemObj.MaterialNo = s4hitem.MATERIAL_NO;
                                            insertItemObj.MaterialDescription = s4hitem.MATERIAL_DESC;
                                            insertItemObj.POQty = s4hitem.PO_QUAN;
                                            insertItemObj.Status = (int)RFIReqStatus.Pending;
                                            insertItemObj.UOM = s4hitem.UOM;
                                            insertItemObj.MaterialClass = 0;
                                            insertItemObj.ServiceNo = s4hitem.SERVICE_NO;
                                            insertItemObj.ServiceDescription = s4hitem.SERVICE_DESC;
                                            insertItemObj.ServiceQty = s4hitem.SERVICE_QUAN;
                                            insertItemObj.ServiceUOM = s4hitem.SERVICE_UOM;
                                            insertItemObj.CreationTime = DateTime.Now;
                                            insertItemObj.CreatorUserId = _abpSession.UserId;
                                            insertItemObj.PONo = obj.PO_NO;

                                            await _db.POItems.AddAsync(insertItemObj);


                                        }
                                        else if (s4hitem.SERVICE_QUAN > netPOItem.ServiceQty)
                                        {
                                            float diff = (s4hitem.SERVICE_QUAN - netPOItem.ServiceQty) ?? 0f;

                                            var itemTobeupdate = resRFIObj.Where(w => w.ItemNo == s4hitem.ITEM_NO && w.ServiceNo == s4hitem.SERVICE_NO).ToList();
                                            foreach (var up in itemTobeupdate)
                                            {
                                                up.ServiceQty = s4hitem.SERVICE_QUAN;
                                                up.BalanceQty = up.BalanceQty + diff;
                                            }

                                            //update PO data table
                                            //check this will update or not POdata 
                                            netPOItem.ServiceQty = s4hitem.SERVICE_QUAN;
                                        }
                                        else if (s4hitem.SERVICE_QUAN < netPOItem.ServiceQty)
                                        {
                                            float diff = (netPOItem.ServiceQty - s4hitem.SERVICE_QUAN) ?? 0f;

                                            var itemTobeupdate = resRFIObj.Where(w => w.ItemNo == s4hitem.ITEM_NO && w.ServiceNo == s4hitem.SERVICE_NO).ToList();
                                            foreach (var up in itemTobeupdate)
                                            {
                                                //logic place after testing
                                                if ((up.BalanceQty - diff) < 0)
                                                {
                                                    //should not throw exception only on approve this case will throw exception
                                                    throw new UserFriendlyException("Quantity of the item( " + s4hitem.ITEM_NO + ") has been updated in S4H.");
                                                }
                                                else
                                                {
                                                    up.ServiceQty = s4hitem.SERVICE_QUAN;
                                                    up.BalanceQty = up.BalanceQty - diff;
                                                }
                                               
                                            }

                                            //update PO data table
                                            //check this will update or not POdata 
                                            netPOItem.ServiceQty = s4hitem.SERVICE_QUAN;
                                        }
                                        else
                                        { }

                                        await _db.SaveChangesAsync();
                                    }
                                }
                            }
                            //For Material PO
                            else if (objList.PO_TYPE == POType.ZPDM.ToString() || objList.PO_TYPE == POType.ZPIM.ToString())
                            {
                                //can filter && (x.Status == (int)RFIReqStatus.Pending || x.Status == (int)RFIReqStatus.Refer)
                                var resICObj = await _db.ICData
                                           .Where(x => x.POMasterId == POmasterId)//todo later filter out with && x.InputQty > 0
                                           .ToListAsync();

                                if (resICObj == null || resICObj.Count == 0)
                                {
                                    //delete existing po line item
                                    var deletePOData = await _db.POItems.Where(x => x.POMasterId == POmasterId).ToListAsync();
                                    deletePOData.RemoveAll(x => x.POMasterId == POmasterId);

                                    _db.SaveChanges();

                                    //insert all new records
                                    foreach (var s4hitem in s4hitems)
                                    {
                                        POItem insertItemObj = new POItem();
                                        insertItemObj.POMasterId = POmasterId;
                                        insertItemObj.ItemNo = s4hitem.ITEM_NO;
                                        insertItemObj.MaterialNo = s4hitem.MATERIAL_NO;
                                        insertItemObj.MaterialDescription = s4hitem.MATERIAL_DESC;
                                        insertItemObj.POQty = s4hitem.PO_QUAN;
                                        insertItemObj.Status = (int)ICReqStatus.Pending;
                                        insertItemObj.UOM = s4hitem.UOM;
                                        insertItemObj.MaterialClass = s4hitem.MATERIAL_CLASS == "A" ? 1 : (s4hitem.MATERIAL_CLASS == "B" ? 2 : (s4hitem.MATERIAL_CLASS == "C" ? 3 : 0));
                                        insertItemObj.ServiceNo = s4hitem.SERVICE_NO;
                                        insertItemObj.ServiceDescription = s4hitem.SERVICE_DESC;
                                        insertItemObj.ServiceQty = s4hitem.SERVICE_QUAN;
                                        insertItemObj.ServiceUOM = s4hitem.SERVICE_UOM;
                                        insertItemObj.CreationTime = DateTime.Now;
                                        insertItemObj.CreatorUserId = _abpSession.UserId;
                                        insertItemObj.PONo = obj.PO_NO;

                                        await _db.POItems.AddAsync(insertItemObj);
                                    }
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    POItem netPOItem = new();
                                    List<POItem> insitems = new List<POItem>();

                                    foreach (var s4hitem in s4hitems)
                                    {
                                        //insert into history table, maintain each change in history 
                                        netPOItem = POMasterDetail.POItems.Where(x => x.ItemNo == s4hitem.ITEM_NO).SingleOrDefault();

                                        if (netPOItem == null || string.IsNullOrEmpty(netPOItem.ItemNo))
                                        {
                                            POItem insertItemObj = new POItem();
                                            insertItemObj.POMasterId = POmasterId;
                                            insertItemObj.ItemNo = s4hitem.ITEM_NO;
                                            insertItemObj.MaterialNo = s4hitem.MATERIAL_NO;
                                            insertItemObj.MaterialDescription = s4hitem.MATERIAL_DESC;
                                            insertItemObj.POQty = s4hitem.PO_QUAN;
                                            insertItemObj.Status = (int)RFIReqStatus.Pending;
                                            insertItemObj.UOM = s4hitem.UOM;
                                            insertItemObj.MaterialClass = s4hitem.MATERIAL_CLASS == "A" ? 1 : (s4hitem.MATERIAL_CLASS == "B" ? 2 : (s4hitem.MATERIAL_CLASS == "C" ? 3 : 0));
                                            insertItemObj.ServiceNo = s4hitem.SERVICE_NO;
                                            insertItemObj.ServiceDescription = s4hitem.SERVICE_DESC;
                                            insertItemObj.ServiceQty = s4hitem.SERVICE_QUAN;
                                            insertItemObj.ServiceUOM = s4hitem.SERVICE_UOM;
                                            insertItemObj.CreationTime = DateTime.Now;
                                            insertItemObj.CreatorUserId = _abpSession.UserId;
                                            insertItemObj.PONo = obj.PO_NO;

                                            await _db.POItems.AddAsync(insertItemObj);


                                        }
                                        else if (s4hitem.PO_QUAN > netPOItem.POQty)
                                        {
                                            //float diff = (s4hitem.SERVICE_QUAN - netPOItem.ServiceQty) ?? 0f;
                                            decimal? diff = s4hitem.PO_QUAN - netPOItem.POQty;
                                            var itemTobeupdate = resICObj.Where(w => w.ItemNo == s4hitem.ITEM_NO).ToList();
                                            foreach (var up in itemTobeupdate)
                                            {
                                                up.POQty = ((float)s4hitem.PO_QUAN);
                                                up.ICBalanceQty = up.ICBalanceQty + ((float)diff);
                                            }

                                            //update PO data table
                                            //check this will update or not POdata 
                                            netPOItem.POQty = s4hitem.PO_QUAN;
                                        }
                                        else if (s4hitem.PO_QUAN < netPOItem.POQty)
                                        {
                                            decimal? diff = netPOItem.POQty - s4hitem.PO_QUAN;
                                            //float diff = (netPOItem.POQty - s4hitem.PO_QUAN) ?? 0f;
                                            
                                            var itemTobeupdate = resICObj.Where(w => w.ItemNo == s4hitem.ITEM_NO).ToList();
                                            foreach (var up in itemTobeupdate)
                                            {
                                                //logic place after testing
                                                if ((up.ICBalanceQty - ((float)diff)) < 0)
                                                {
                                                    //should not throw exception only on approve this case will throw exception
                                                    throw new UserFriendlyException("Quantity of the item( " + s4hitem.ITEM_NO + ") has been updated in S4H.");
                                                }
                                                else
                                                {
                                                    up.POQty = (float)s4hitem.PO_QUAN;
                                                    up.ICBalanceQty = up.ICBalanceQty - ((float)diff);
                                                }
                                            }

                                            //update PO data table
                                            //check this will update or not POdata 
                                            netPOItem.POQty = s4hitem.PO_QUAN;
                                        }
                                        else
                                        { }

                                        await _db.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                        else
                        {
                            POMaster insertObj = new POMaster();
                            insertObj.POType = objList.PO_TYPE;
                            insertObj.PONo = obj.PO_NO;
                            insertObj.VendorNo = objList.VENDOR_NO;
                            insertObj.VendorName = objList.VENDOR_NAME;
                            insertObj.ProjectName = objList.PROJECT_NAME;
                            insertObj.Plant = string.Empty;
                            insertObj.CreationTime = DateTime.Now;
                            insertObj.CreatedBySAPUser = objList.CREATED_BY == null ? string.Empty : objList.CREATED_BY;

                            await _db.POMaster.AddAsync(insertObj);
                            await _db.SaveChangesAsync();
                            if (insertObj.Id <= 0)
                            {
                                throw new UserFriendlyException("No SAP bindings found for this channel.");
                            }

                            List<POItem> insitems = new List<POItem>();

                            foreach (var insertItem in objList.PO_DETAILS.item)
                            {
                                POItem insertItemObj = new POItem();
                                insertItemObj.POMasterId = insertObj.Id;
                                insertItemObj.ItemNo = insertItem.ITEM_NO;
                                insertItemObj.MaterialNo = insertItem.MATERIAL_NO;
                                insertItemObj.MaterialDescription = insertItem.MATERIAL_DESC;
                                insertItemObj.POQty = insertItem.PO_QUAN;
                                insertItemObj.Status = (int)RFIReqStatus.Pending;
                                insertItemObj.UOM = insertItem.UOM;
                                insertItemObj.MaterialClass = insertItem.MATERIAL_CLASS == "A" ? 1 : (insertItem.MATERIAL_CLASS == "B" ? 2 : (insertItem.MATERIAL_CLASS == "C" ? 3 : 0));
                                insertItemObj.ServiceNo = insertItem.SERVICE_NO;
                                insertItemObj.ServiceDescription = insertItem.SERVICE_DESC;
                                insertItemObj.ServiceQty = insertItem.SERVICE_QUAN;
                                insertItemObj.ServiceUOM = insertItem.SERVICE_UOM;
                                insertItemObj.CreationTime = DateTime.Now;
                                insertItemObj.CreatorUserId = _abpSession.UserId;
                                insertItemObj.PONo = obj.PO_NO;

                                await _db.POItems.AddAsync(insertItemObj);

                            }
                            await _db.SaveChangesAsync();
                        }
                        return BaseResponse.SuccessResponse(200, "Success");
                    //}
                    //catch (Exception ex)
                    //{
                    //    //This catch only works when single object comes from SAP and insert if doesnt exist in .net
                    //    var spParams = new DynamicParameters();
                    //    spParams.Add("@RepsoneJson", apiResponse);
                    //    // spParams.Add("@Created_By", DateTime.Now.ToString());
                    //    spParams.Add("@SAMBody", null);
                    //    spParams.Add("@PO_Number", obj.PO_NO);
                    //    spParams.Add("@Flag", 1);
                    //    spParams.Add("@API_Name", "");
                    //    spParams.Add("@ID", null);

                    //    var SPSyncPOResponse = await _dapper.GetSingle<SPSyncPOResponse>("[dbo].[API_Sync_PO]"
                    //               , spParams, cancellationToken,
                    //                commandType: CommandType.StoredProcedure);

                    //    if (SPSyncPOResponse == null)
                    //    {
                    //        return BaseResponse.failedResponse(500, "Failed");
                    //    }
                    //    return BaseResponse.SuccessResponse(200, "Success");
                    //}
                }
                else
                {
                    return BaseResponse.failedResponse(500, "Failed");
                }
            }
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex.Message);
        //    if (ex.ToString().ToLower().Contains("connected host has failed to respond"))
        //    {
        //        return BaseResponse.failedResponse(500, "Network issue.");
        //    }
        //    return BaseResponse.failedResponse(500, "Unable to fetch/save PO data");
        //}
    }



}

