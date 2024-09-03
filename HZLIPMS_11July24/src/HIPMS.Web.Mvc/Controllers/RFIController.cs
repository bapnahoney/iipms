using Abp.AspNetCore.Mvc.Authorization;
using Abp.Runtime.Session;
using Abp.UI;
using ClosedXML.Excel;
using HIPMS.Authorization;
using HIPMS.Controllers;
using HIPMS.IC.Dto;
using HIPMS.Models;
using HIPMS.Roles;
using HIPMS.Shared;
using HIPMS.SRFI;
using HIPMS.SRFI.Dto;
using HIPMS.Web.Models.MRFI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.Web.Controllers
{
    [AbpMvcAuthorize(PermissionNames.Pages_IC)]
    public class RFIController : HIPMSControllerBase
    {
        private readonly IRFIAppService _iRFIAppService;
        private readonly IAbpSession _abpSession;
        private readonly IRoleAppService _roleAppService;

        public RFIController(IRFIAppService iRFIAppService, IAbpSession abpSession
                            , IRoleAppService roleAppService)
        {
            _iRFIAppService = iRFIAppService;
            _abpSession = abpSession;
            _roleAppService = roleAppService;
        }

        
     
        public async Task<IActionResult> Index(int page = 1)
        {

            PagedRFIResultRequestDto reqObj = new();
            reqObj.MaxResultCount = 10;

            var reqItems = (await _iRFIAppService.GetAllReqItemsAsync(reqObj));

            List<RFIListViewModel> resObj = new List<RFIListViewModel>();
            foreach (var icReqObj in reqItems)
            {
                RFIListViewModel viewResModel = new();
                viewResModel.Id = icReqObj.Id;
                viewResModel.PO = icReqObj.RFIData.FirstOrDefault().POMaster.PONo;
                viewResModel.VendorName = icReqObj?.VendorName;
                viewResModel.VendorNo = icReqObj?.VendorNo;
                viewResModel.VendorRemark = icReqObj?.VendorRemark;
                //viewResModel.ManufacturerPlantAddress = icReqObj?.ManufacturerPlantAddress;
                //viewResModel.ManufacturerName = icReqObj?.ManufacturerName;

                foreach (var item in icReqObj.RFIData)
                {
                    RFIItemsData obj = new RFIItemsData();
                    obj.RFIId = item.RFIId;
                    obj.PONo = item.PONo;
                    obj.ItemNo = item.ItemNo;
                    obj.PreviousQty = item.PreviousQty;
                    obj.BalanceQty = item.BalanceQty;
                    obj.InputQty = item.InputQty;
                    obj.POQty = item.POQty;
                    //obj.MaterialNo = item.MaterialNo;
                    obj.Status = item.Status;
                    obj.StatusValue = item.StatusValue;
                    obj.UOM = item.UOM;
                    obj.MaterialDescription = item.MaterialDescription;
                    // obj.MaterialClassValue = item.MaterialClassValue;
                    //obj.MaterialClass = item.MaterialClass;
                    //obj.MaterialClassList = (POMaterialClass)item.MaterialClass;
                    obj.ServiceNo = item.ServiceNo;
                    obj.ServiceDescription = item.ServiceDescription;
                    obj.ServiceQty = item.ServiceQty;
                    obj.ServiceUOM = item.ServiceUOM;
                    viewResModel.RFIItems.Add(obj);
                }
                resObj.Add(viewResModel);
            }
            if (resObj != null && resObj.Count > 0)
            {
                var test = resObj.Skip(HIPMSConsts.PageSize * (page - 1))
                             .Take(HIPMSConsts.PageSize);

                test.FirstOrDefault().CurrentPage = page;
                test.FirstOrDefault().TotalPage = (resObj.Count / HIPMSConsts.PageSize) + 1;
                test.FirstOrDefault().HasPrev = page > 1 ? true : false;
                test.FirstOrDefault().HasNext = page == test.FirstOrDefault().TotalPage ? false : true;


                return View(test.ToList());
            }
            else
            {
                ErrorMessage obj = new ErrorMessage();
                obj.Errormessage = "Either no request available or PO not mapped with the user.";
                return PartialView("../Shared/ErrorModal", obj);
                //ICErrorModel obj = new ICErrorModel();
                //obj.Errormessage = "Either no request available or PO not mapped with the user.";
                //return PartialView("ErrorModal", obj);
            }
        }

        public async Task<ActionResult> Create(CreateRFIRequestDto input)
        {
            //CancellationToken cancellationToken = CancellationToken.None;
            //await _icAppService.CreateAsync(input, cancellationToken);
            bool isValid = await _iRFIAppService.IsValidUser();
            if (isValid)
            {
                return View("Create");
            }
            throw new UserFriendlyException(L("User do not have sufficient permission."));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<RFIListViewModel> POSearch(string POnumber)
        {
            if (String.IsNullOrEmpty(POnumber) || POnumber.ToLower().Contains("enter"))
            {
                throw new UserFriendlyException(L("Invalid PO number."));
            }
            CancellationToken cancellationToken = new CancellationToken();
            SearchPORequest searchPORequest = new SearchPORequest();
            searchPORequest.PONumber = POnumber;
            POMaster SearchPODetailRes = await _iRFIAppService.SearchPODetailAsync(searchPORequest, cancellationToken);
            if (SearchPODetailRes != null && SearchPODetailRes.Id > 0)
            {

                RFIListViewModel rfiListViewModel = new()
                {
                    PO = SearchPODetailRes.PONo,
                    VendorName = SearchPODetailRes.VendorName,
                    VendorNo = SearchPODetailRes.VendorNo,
                    POType = SearchPODetailRes.POType,
                    //VendorRemark = SearchPODetailRes.VendorRemark,
                    //ManufacturerName=
                    //ManufacturerPlantAddress = SearchPODetailRes.Plant,
                    ProjectName = SearchPODetailRes.ProjectName,
                };

                foreach (POItem itm in SearchPODetailRes.POItems)
                {
                    float PreviousQty = await _iRFIAppService.GetRFIPreviousQtyAsync(SearchPODetailRes.PONo, itm.ItemNo, itm.ServiceNo, cancellationToken);

                    RFIItemsData rfiData = new RFIItemsData();
                    rfiData.PONo = itm.PONo;
                    rfiData.POMasterId = itm.POMasterId;
                    rfiData.ItemNo = itm.ItemNo;
                    rfiData.MaterialDescription = itm.MaterialDescription;
                    //rfiData.MaterialNo = itm.MaterialNo;
                    rfiData.MaterialClass = itm.MaterialClass;
                    rfiData.MaterialClassValue = ((POMaterialClass)itm.MaterialClass).ToString();
                    rfiData.POQty = Convert.ToInt32(itm.POQty);
                    rfiData.ServiceQty = itm.ServiceQty;
                    rfiData.PreviousQty = PreviousQty;
                    rfiData.BalanceQty = rfiData.ServiceQty - PreviousQty;
                    rfiData.Status = itm.Status;
                    rfiData.UOM = itm.UOM;
                    rfiData.ServiceNo = itm.ServiceNo;
                    rfiData.ServiceDescription = itm.ServiceDescription;
                    rfiData.ServiceUOM = itm.ServiceUOM;

                    //iCData.StatusValue = ICReqStatus.Pending.ToString();
                    //iCData.DrawingNo = itm.DrawingNo;
                    //iCData.CreatorUserId = itm.CreatorUserId;
                    //iCData.CreationTime = DateTime.Now;

                    rfiListViewModel.RFIItems.Add(rfiData);
                };

                return rfiListViewModel;
            }
            return null;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("application/json")]
        public async Task<string> RFIRequest([FromBody] CreateRFIRequestDto request)
        {
            CancellationToken cancellationToken = CancellationToken.None;

            var editedObj = await _iRFIAppService.CreateAsync(request, cancellationToken);

            return "Created succesfully";
        }

        //[HttpPost]
        //[Consumes("application/json")]
        //public async Task<ActionResult> EditIC([FromBody] CreateRFIRequestDto request)
        //{
        //    CancellationToken cancellationToken = CancellationToken.None;
        //    await _iRFIAppService.CreateAsync(request, cancellationToken);

        //    return View("Create");
        //}

        public async Task<ActionResult> Edit(long id)
        {
            var reqItems = await _iRFIAppService.GetReqItemsAsync(id);
            if (reqItems == null || reqItems.RFIData.Count == 0)
            {
                throw new UserFriendlyException(L("Error"));
            }
            string role = await _roleAppService.GetUserRoleName();
            RFIEditRequest editICModalViewModel = new RFIEditRequest();
            editICModalViewModel.Id = id;
            editICModalViewModel.RoleName = role;
            editICModalViewModel.Status = reqItems.Status;
            List<RFIItemEditRequest> rfiItems = new();
            foreach (var item in reqItems.RFIData)
            {
                RFIItemEditRequest obj = new RFIItemEditRequest();
                obj.Id = item.Id;
                //added on 22-07-2024
                obj.POMasterId = item.POMasterId;
                obj.RFIId = item.RFIId;
                obj.PONo = item.PONo;
                obj.ItemNo = item.ItemNo;
                obj.PreviousQty = item.PreviousQty;
                obj.BalanceQty = item.BalanceQty;
                obj.InputQty = item.InputQty;
                //obj.MaterialNo = item.MaterialNo;
                obj.POQty = item.POQty;
                obj.MaterialDescription = item.MaterialDescription;
                obj.MaterialClassValue = item.MaterialClassValue;
                obj.MaterialClass = item.MaterialClass;
                obj.MaterialClassList = (POMaterialClass)item.MaterialClass;
                obj.Status = item.Status;
                //obj.StatusValue = ((RFIReqStatus)item.Status).ToString();
                obj.StatusValue = GetEnumDisplayValue(item.Status);
                obj.UOM = item.UOM;
                obj.StatusList = (RFIReqStatus)item.Status;
                obj.ServiceNo = item.ServiceNo;
                obj.ServiceDescription = item.ServiceDescription;
                obj.ServiceQty = item.ServiceQty;
                obj.ServiceUOM = item.ServiceUOM;
                //obj.Make = item.Make;
                //obj.Model = item.Model;
                //obj.PartNo = item.PartNo;
                obj.RFIDocuments = item.RFIDocuments.ToList();
                rfiItems.Add(obj);
            }
            editICModalViewModel.RFIItems = rfiItems;
            editICModalViewModel.UserActionRemark = reqItems.UserActionRemark;
            editICModalViewModel.OECActionRemark = reqItems.OECActionRemark;
            editICModalViewModel.VendorRemark = reqItems.VendorRemark;
            editICModalViewModel.IsApprovedByOEC = reqItems.IsApprovedByOEC;
            editICModalViewModel.OpenNCR = await _iRFIAppService.GetOpenNCRCount(reqItems.POMasterId);
            // return PartialView("_EditModal", editICModalViewModel);
            return View("Edit", editICModalViewModel);
        }
        public async Task<ActionResult> AddRequest()
        {
            return View("Add");
        }


        public async Task<ActionResult> EditRFIRequest([FromForm] RFIEditRequest model)
        {
            try
            {
                if (model == null)
                {
                    throw new UserFriendlyException(L("Error"));
                }
                if (HttpContext.Request.Form["refer"].Count == 1)
                {
                    model.UserAction = RFIReqStatus.Refer.ToString();
                    model.Status = (int)RFIReqStatus.Refer;
                    model.RFIItems
                       .ForEach(x =>
                       {
                           x.Status = (int)RFIReqStatus.Refer;
                           x.StatusValue = RFIReqStatus.Refer.ToString();
                       });
                }
                else if (HttpContext.Request.Form["reject"].Count == 1)
                {
                    model.UserAction = RFIReqStatus.Reject.ToString();
                    model.Status = (int)RFIReqStatus.Reject;
                    model.RFIItems
                     .ForEach(x =>
                     {
                         x.Status = (int)RFIReqStatus.Reject;
                         x.StatusValue = RFIReqStatus.Reject.ToString();
                     });
                }
                else if (HttpContext.Request.Form["update"].Count == 1)
                {
                    model.UserAction = RFIReqStatus.Pending.ToString();
                    model.Status = (int)RFIReqStatus.Pending;
                    model.RFIItems
                     .ForEach(x =>
                     {
                         x.Status = (int)RFIReqStatus.Pending;
                         x.StatusValue = RFIReqStatus.Pending.ToString();
                     });
                }
                else
                {
                    model.UserAction = RFIReqStatus.Approve.ToString();
                    model.Status = (int)RFIReqStatus.Approve;
                    model.RFIItems
                   .ForEach(x =>
                   {
                       x.Status = (int)RFIReqStatus.Approve;
                       x.StatusValue = RFIReqStatus.Approve.ToString();
                   });
                }

                CancellationToken cancellationToken = CancellationToken.None;
                if (model is null || model.RFIItems is null || model.RFIItems.Count == 0)
                {
                    throw new UserFriendlyException("");
                }
                ClaimsPrincipal currentUser = this.User;
                await _iRFIAppService.UpdateAsync(model, currentUser, cancellationToken);
                //return RedirectToAction("Edit","IC",obj.Id,);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ErrorMessage obj = new ErrorMessage();
                obj.Errormessage = ex.Message;
                return PartialView("../Shared/ErrorModal", obj);
            }
           
        }

        //using closedxml  
        public IActionResult DownloadExcel()
        {
            PagedRFIResultRequestDto reqObj = new();

            var reqItems = _iRFIAppService.GetAllReqItemsAsync(reqObj).Result;

            List<RFIExcelReport> resObj = new List<RFIExcelReport>();
            foreach (var rfiReqObj in reqItems)
            {
                foreach (var item in rfiReqObj.RFIData)
                {
                    RFIExcelReport viewResModel = new();

                    viewResModel.PO = rfiReqObj.RFIData.FirstOrDefault().POMaster.PONo;
                    viewResModel.VendorName = rfiReqObj?.VendorName;
                    viewResModel.VendorNo = rfiReqObj?.VendorNo;
                    viewResModel.VendorRemark = rfiReqObj?.VendorRemark;
                    viewResModel.ItemNo = item.ItemNo;
                    viewResModel.ServiceNo = item.ServiceNo;
                    viewResModel.ServiceDescription = item.ServiceDescription;
                    viewResModel.ServiceQty = item.ServiceQty;
                    viewResModel.ServiceUOM = item.ServiceUOM;
                    viewResModel.PreviousQty = item.PreviousQty;
                    viewResModel.BalanceQty = item.BalanceQty;
                    viewResModel.InputQty = item.InputQty;
                    viewResModel.POQty = item.POQty;
                    viewResModel.StatusValue = item.StatusValue;
                    viewResModel.UOM = item.UOM;
                    viewResModel.ApprovedOn = item.ApprovedOn;
                    viewResModel.Approver = item.Approver;
                    resObj.Add(viewResModel);
                }

            }
            if (resObj != null && resObj.Count > 0)
            {
                //required using ClosedXML.Excel;  
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "RFIReport.xlsx";
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        IXLWorksheet worksheet =
                        workbook.Worksheets.Add("IC");
                        worksheet.Cell(1, 1).Value = "PO";
                        worksheet.Cell(1, 2).Value = "VendorName";
                        worksheet.Cell(1, 3).Value = "VendorNo";
                        worksheet.Cell(1, 4).Value = "VendorRemark";
                        worksheet.Cell(1, 5).Value = "ItemNo";
                        worksheet.Cell(1, 6).Value = "PreviousQty";
                        worksheet.Cell(1, 7).Value = "BalanceQty";
                        worksheet.Cell(1, 8).Value = "InputQty";
                        worksheet.Cell(1, 9).Value = "POQty";
                        worksheet.Cell(1, 10).Value = "ServiceNo";
                        worksheet.Cell(1, 11).Value = "StatusValue";
                        worksheet.Cell(1, 12).Value = "UOM";
                        worksheet.Cell(1, 13).Value = "ServiceDescription";
                        worksheet.Cell(1, 14).Value = "ServiceUOM";
                        worksheet.Cell(1, 15).Value = "ServiceQty";
                        worksheet.Cell(1, 16).Value = "ApprovedOn";
                        worksheet.Cell(1, 17).Value = "Approver";

                        for (int index = 1; index <= resObj.Count; index++)
                        {
                            worksheet.Cell(index + 1, 1).Value = resObj[index - 1].PO;
                            worksheet.Cell(index + 1, 2).Value = resObj[index - 1].VendorName;
                            worksheet.Cell(index + 1, 3).Value = resObj[index - 1].VendorNo;
                            worksheet.Cell(index + 1, 4).Value = resObj[index - 1].VendorRemark;
                            worksheet.Cell(index + 1, 5).Value = resObj[index - 1].ItemNo;
                            worksheet.Cell(index + 1, 6).Value = resObj[index - 1].PreviousQty;
                            worksheet.Cell(index + 1, 7).Value = resObj[index - 1].BalanceQty;
                            worksheet.Cell(index + 1, 8).Value = resObj[index - 1].InputQty;
                            worksheet.Cell(index + 1, 9).Value = resObj[index - 1].POQty;
                            worksheet.Cell(index + 1, 10).Value = resObj[index - 1].ServiceNo;
                            worksheet.Cell(index + 1, 11).Value = resObj[index - 1].StatusValue;
                            worksheet.Cell(index + 1, 12).Value = resObj[index - 1].UOM;
                            worksheet.Cell(index + 1, 13).Value = resObj[index - 1].ServiceDescription;
                            worksheet.Cell(index + 1, 14).Value = resObj[index - 1].ServiceUOM;
                            worksheet.Cell(index + 1, 15).Value = resObj[index - 1].ServiceQty;
                            worksheet.Cell(index + 1, 16).Value = resObj[index - 1].ApprovedOn;
                            worksheet.Cell(index + 1, 17).Value = resObj[index - 1].Approver;
                        }
                        //required using System.IO;  
                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var content = stream.ToArray();
                            return File(content, contentType, fileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                ErrorMessage obj = new ErrorMessage();
                obj.Errormessage = "Either no request available or PO not mapped with the user.";
                return PartialView("../Shared/ErrorModal", obj);
            }
        }
    }
}