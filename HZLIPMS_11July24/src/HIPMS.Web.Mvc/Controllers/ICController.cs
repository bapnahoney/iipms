using Abp.AspNetCore.Mvc.Authorization;
using Abp.Runtime.Session;
using Abp.UI;
using ClosedXML.Excel;
using HIPMS.Authorization;
using HIPMS.Controllers;
using HIPMS.IC;
using HIPMS.IC.Dto;
using HIPMS.Models;
using HIPMS.Roles;
using HIPMS.Shared;
using HIPMS.Web.Models.IC;
using Microsoft.AspNetCore.Http;
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
    public class ICController : HIPMSControllerBase
    {
        private readonly IInspectionClearanceService _icAppService;
        private readonly IAbpSession _abpSession;
        private readonly IRoleAppService _roleAppService;
        public ICController(IInspectionClearanceService icAppService, IAbpSession abpSession, IRoleAppService roleAppService)
        {
            _icAppService = icAppService;
            _abpSession = abpSession;
            _roleAppService = roleAppService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {

            PagedICResultRequestDto reqObj = new();
            reqObj.MaxResultCount = 10;

            var reqItems = (await _icAppService.GetAllReqItemsAsync(reqObj));
            //var model = new ICListViewModel
            //{
            //    ICItems = reqItems
            //};

            List<ICListViewModel> resObj = new List<ICListViewModel>();
            foreach (var icReqObj in reqItems)
            {
                ICListViewModel viewResModel = new();
                viewResModel.Id = icReqObj.Id;
                viewResModel.PO = icReqObj.ICData.FirstOrDefault().POMaster.PONo;
                viewResModel.VendorName = icReqObj?.VendorName;
                viewResModel.VendorNo = icReqObj?.VendorNo;
                viewResModel.VendorRemark = icReqObj?.VendorRemark;
                viewResModel.ManufacturerPlantAddress = icReqObj?.ManufacturerPlantAddress;
                viewResModel.ManufacturerName = icReqObj?.ManufacturerName;

                foreach (var item in icReqObj.ICData)
                {
                    ICItemsData obj = new ICItemsData();
                    obj.InspectionClearanceId = item.InspectionClearanceId;
                    obj.PONo = item.PONo;
                    obj.ItemNo = item.ItemNo;
                    obj.ICPreviousQty = item.ICPreviousQty;
                    obj.ICBalanceQty = item.ICBalanceQty;
                    obj.ICInputQty = item.ICInputQty;
                    obj.POQty = item.POQty;
                    obj.MaterialNo = item.MaterialNo;
                    obj.Status = item.Status;
                    obj.StatusValue = item.StatusValue;
                    obj.UOM = item.UOM;
                    obj.MaterialDescription = item.MaterialDescription;
                    obj.MaterialClassValue = item.MaterialClassValue;
                    obj.MaterialClass = item.MaterialClass;
                    obj.MaterialClassList = (POMaterialClass)item.MaterialClass;
                    viewResModel.ICItems.Add(obj);
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

        public async Task<ActionResult> Create(CreateICRequestDto input)
        {
            //CancellationToken cancellationToken = CancellationToken.None;
            bool isValid = await _icAppService.IsValidUser();
            if (isValid)
            {
                return View("Create");
            }
            ErrorMessage obj = new ErrorMessage();
            obj.Errormessage = "User do not have sufficient permission.";
            return PartialView("../Shared/ErrorModal", obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ICListViewModel> POSearch(string POnumber)
        {
            if (string.IsNullOrEmpty(POnumber) || POnumber.ToLower().Contains("enter"))
            {
                throw new UserFriendlyException(L("Invalid PO number."));
            }
            CancellationToken cancellationToken = new CancellationToken();
            SearchPORequest searchPORequest = new SearchPORequest();
            searchPORequest.PONumber = POnumber;
            POMaster SearchPODetailRes = await _icAppService.SearchPODetailAsync(searchPORequest, cancellationToken);
            if (SearchPODetailRes != null && SearchPODetailRes.Id > 0)
            {

                ICListViewModel iCListViewModel = new()
                {
                    PO = SearchPODetailRes.PONo,
                    VendorName = SearchPODetailRes.VendorName,
                    VendorNo = SearchPODetailRes.VendorNo,
                    POType = SearchPODetailRes.POType,
                    //VendorRemark = SearchPODetailRes.VendorRemark,
                    //ManufacturerName=
                    ManufacturerPlantAddress = SearchPODetailRes.Plant,
                    ProjectName = SearchPODetailRes.ProjectName,
                };

                foreach (POItem itm in SearchPODetailRes.POItems)
                {
                    float ICPreviousQty = await _icAppService.GetICPreviousQtyAsync(SearchPODetailRes.PONo, itm.ItemNo, cancellationToken);

                    ICItemsData iCData = new ICItemsData();
                    iCData.PONo = itm.PONo;
                    iCData.POMasterId = itm.POMasterId;
                    iCData.ItemNo = itm.ItemNo;
                    iCData.MaterialDescription = itm.MaterialDescription;
                    iCData.MaterialNo = itm.MaterialNo;
                    iCData.MaterialClass = itm.MaterialClass;
                    iCData.MaterialClassValue = ((POMaterialClass)itm.MaterialClass).ToString();
                    iCData.POQty = Convert.ToInt32(itm.POQty);
                    iCData.ICPreviousQty = ICPreviousQty;
                    iCData.ICBalanceQty = iCData.POQty - ICPreviousQty;
                    iCData.Status = itm.Status;
                    iCData.UOM = itm.UOM;
                    //iCData.StatusValue = ICReqStatus.Pending.ToString();
                    //iCData.DrawingNo = itm.DrawingNo;
                    //iCData.CreatorUserId = itm.CreatorUserId;
                    //iCData.CreationTime = DateTime.Now;

                    iCListViewModel.ICItems.Add(iCData);
                };

                return iCListViewModel;
            }
            return null;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("application/json")]
        public async Task<string> ICRequest([FromBody] CreateICRequestDto request)
        {
            CancellationToken cancellationToken = CancellationToken.None;
            var editedObj = await _icAppService.CreateAsync(request, cancellationToken);

            //try
            //{
            //    await _icAppService.CreateAsync(request, cancellationToken);
            //}
            //catch (Exception ex)
            //{
            //        return ex.Message;
            //    //ErrorViewModel errorViewModel = new ErrorViewModel();
            //    //errorViewModel.ErrorInfo = new();
            //    //errorViewModel.ErrorInfo.Code = 500;
            //    //errorViewModel.ErrorInfo.Message = ex.Message;
            //    //return RedirectToAction("Error");
            //}
            return "Created succesfully";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("application/json")]
        public async Task<ActionResult> EditIC([FromBody] CreateICRequestDto request)
        {
            CancellationToken cancellationToken = CancellationToken.None;
            await _icAppService.CreateAsync(request, cancellationToken);

            return View("Create");
        }

        public async Task<ActionResult> Edit(long id)
        {
            if(HttpContext.Session.GetString("SessionVal") == null)
            {
                //await _signInManager.SignOutAsync();
                return RedirectToAction("Logout", "Account");
            }
            var reqItems = await _icAppService.GetReqItemsAsync(id);
            if (reqItems == null || reqItems.ICData.Count == 0)
            {
                throw new UserFriendlyException(L("Error"));
            }
            string role = await _roleAppService.GetUserRoleName();
            ICEditRequest editICModalViewModel = new ICEditRequest();
            editICModalViewModel.Id = id;
            editICModalViewModel.RoleName = role;

            List<ICItemEditRequest> icItems = new();
            foreach (var item in reqItems.ICData)
            {
                ICItemEditRequest obj = new ICItemEditRequest();
                obj.Id = item.Id;
                obj.POMasterId = item.POMasterId;
                obj.InspectionClearanceId = item.InspectionClearanceId;
                obj.PONo = item.PONo;
                obj.ItemNo = item.ItemNo;
                obj.ICPreviousQty = item.ICPreviousQty;
                obj.ICBalanceQty = item.ICBalanceQty;
                obj.ICInputQty = item.ICInputQty;
                obj.MaterialNo = item.MaterialNo;
                obj.POQty = item.POQty;
                obj.MaterialDescription = item.MaterialDescription;
                obj.MaterialClassValue = item.MaterialClassValue;
                obj.MaterialClass = item.MaterialClass;
                obj.MaterialClassList = (POMaterialClass)item.MaterialClass;
                obj.Status = item.Status;
                // obj.StatusValue = ((ICReqStatus)item.Status).ToString();
                obj.StatusValue = GetEnumDisplayValue(item.Status);
                //obj.StatusValue = (item.Status == 1 ? "Pending" : (item.Status == 2 ? "Approved" : (item.Status == 3 ? "Referred" : "Rejected")));
                obj.UOM = item.UOM;
                obj.Make = item.Make;
                obj.Model = item.Model;
                obj.PartNo = item.PartNo;
                obj.StatusList = (ICReqStatus)item.Status;
                obj.ICDocuments = item.ICDocuments.ToList();
                icItems.Add(obj);
            }
            editICModalViewModel.ICItems = icItems;
            editICModalViewModel.UserActionRemark = reqItems.UserActionRemark;
            editICModalViewModel.VendorRemark = reqItems.VendorRemark;
            editICModalViewModel.OECActionRemark = reqItems.OECActionRemark;
            editICModalViewModel.IsApprovedByOEC = reqItems.IsApprovedByOEC;
            // return PartialView("_EditModal", editICModalViewModel);
            return View("Edit", editICModalViewModel);
        }
        public async Task<ActionResult> AddRequest()
        {
            return View("Add");
        }


        //public async Task<ActionResult> EditICRequest([Bind] ICEditRequest obj)
        //{
        //    CancellationToken cancellationToken = CancellationToken.None;
        //    if (obj is null || obj.ICItems is null || obj.ICItems.Count == 0)
        //    {
        //        throw new UserFriendlyException("");
        //    }
        //    await _icAppService.UpdateAsync(obj, cancellationToken);
        //    //return RedirectToAction("Edit","IC",obj.Id,);
        //    return RedirectToAction("Index");
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditICRequest(ICEditRequest model)
        {
            if (model == null)
            {
                throw new UserFriendlyException(L("Error"));
            }
            if (HttpContext.Request.Form["refer"].Count == 1)
            {
                model.UserAction = ICReqStatus.Refer.ToString();
                model.Status = (int)ICReqStatus.Refer;
                model.ICItems
                   .ForEach(x =>
                   {
                       x.Status = (int)ICReqStatus.Refer;
                       x.StatusValue = ICReqStatus.Refer.ToString();
                   });
            }
            else if (HttpContext.Request.Form["reject"].Count == 1)
            {
                model.UserAction = ICReqStatus.Reject.ToString();
                model.Status = (int)ICReqStatus.Reject;
                model.ICItems
                 .ForEach(x =>
                 {
                     x.Status = (int)ICReqStatus.Reject;
                     x.StatusValue = ICReqStatus.Reject.ToString();
                 });
            }
            else if (HttpContext.Request.Form["update"].Count == 1)
            {
                model.UserAction = ICReqStatus.Pending.ToString();
                model.Status = (int)ICReqStatus.Pending;
                model.ICItems
                 .ForEach(x =>
                 {
                     x.Status = (int)ICReqStatus.Pending;
                     x.StatusValue = ICReqStatus.Pending.ToString();
                 });
            }
            else
            {
                model.UserAction = ICReqStatus.Approve.ToString();
                model.Status = (int)ICReqStatus.Approve;
                model.ICItems
               .ForEach(x =>
               {
                   x.Status = (int)ICReqStatus.Approve;
                   x.StatusValue = ICReqStatus.Approve.ToString();
               });
            }


            CancellationToken cancellationToken = CancellationToken.None;
            if (model is null || model.ICItems is null || model.ICItems.Count == 0)
            {
                throw new UserFriendlyException("");
            }
            ClaimsPrincipal currentUser = this.User;
            try
            {
                _icAppService.UpdateAsync(model, currentUser, cancellationToken).Wait();
            }
            catch(Exception ex) {

                throw new UserFriendlyException(ex.Message);
            }

           
            //return RedirectToAction("Edit","IC",obj.Id,);
            return RedirectToAction("Index");

        }

        //using closedxml  
        public IActionResult DownloadExcel()
        {
            PagedICResultRequestDto reqObj = new();

            var reqItems = _icAppService.GetAllReqItemsAsync(reqObj).Result;

            List<ICExcelReport> resObj = new List<ICExcelReport>();
            foreach (var icReqObj in reqItems)
            {
                foreach (var item in icReqObj.ICData)
                {
                    ICExcelReport viewResModel = new();

                    viewResModel.PO = icReqObj.ICData.FirstOrDefault().POMaster.PONo;
                    viewResModel.VendorName = icReqObj?.VendorName;
                    viewResModel.VendorNo = icReqObj?.VendorNo;
                    viewResModel.VendorRemark = icReqObj?.VendorRemark;

                    viewResModel.ItemNo = item.ItemNo;
                    viewResModel.ICPreviousQty = item.ICPreviousQty;
                    viewResModel.ICBalanceQty = item.ICBalanceQty;
                    viewResModel.ICInputQty = item.ICInputQty;
                    viewResModel.POQty = item.POQty;
                    viewResModel.MaterialNo = item.MaterialNo;
                    viewResModel.StatusValue = item.StatusValue;
                    viewResModel.UOM = item.UOM;
                    viewResModel.MaterialDescription = item.MaterialDescription;
                    viewResModel.MaterialClassValue = item.MaterialClassValue;
                    viewResModel.ApprovedOn = item.ApprovedOn;
                    viewResModel.Approver = item.Approver;
                    resObj.Add(viewResModel);
                }

            }
            if (resObj != null && resObj.Count > 0)
            {
                //required using ClosedXML.Excel;  
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "ICReport.xlsx";
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
                        worksheet.Cell(1, 6).Value = "ICPreviousQty";
                        worksheet.Cell(1, 7).Value = "ICBalanceQty";
                        worksheet.Cell(1, 8).Value = "ICInputQty";
                        worksheet.Cell(1, 9).Value = "POQty";
                        worksheet.Cell(1, 10).Value = "MaterialNo";
                        worksheet.Cell(1, 11).Value = "StatusValue";
                        worksheet.Cell(1, 12).Value = "UOM";
                        worksheet.Cell(1, 13).Value = "MaterialDescription";
                        worksheet.Cell(1, 14).Value = "MaterialClassValue";
                        worksheet.Cell(1, 15).Value = "ApprovedOn";
                        worksheet.Cell(1, 16).Value = "Approver";

                        for (int index = 1; index <= resObj.Count; index++)
                        {
                            worksheet.Cell(index + 1, 1).Value = resObj[index - 1].PO;
                            worksheet.Cell(index + 1, 2).Value = resObj[index - 1].VendorName;
                            worksheet.Cell(index + 1, 3).Value = resObj[index - 1].VendorNo;
                            worksheet.Cell(index + 1, 4).Value = resObj[index - 1].VendorRemark;
                            worksheet.Cell(index + 1, 5).Value = resObj[index - 1].ItemNo;
                            worksheet.Cell(index + 1, 6).Value = resObj[index - 1].ICPreviousQty;
                            worksheet.Cell(index + 1, 7).Value = resObj[index - 1].ICBalanceQty;
                            worksheet.Cell(index + 1, 8).Value = resObj[index - 1].ICInputQty;
                            worksheet.Cell(index + 1, 9).Value = resObj[index - 1].POQty;
                            worksheet.Cell(index + 1, 10).Value = resObj[index - 1].MaterialNo;
                            worksheet.Cell(index + 1, 11).Value = resObj[index - 1].StatusValue;
                            worksheet.Cell(index + 1, 12).Value = resObj[index - 1].UOM;
                            worksheet.Cell(index + 1, 13).Value = resObj[index - 1].MaterialDescription;
                            worksheet.Cell(index + 1, 14).Value = resObj[index - 1].MaterialClassValue;
                            worksheet.Cell(index + 1, 15).Value = resObj[index - 1].ApprovedOn;
                            worksheet.Cell(index + 1, 16).Value = resObj[index - 1].Approver;
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
                //ICErrorModel obj = new ICErrorModel();
                //obj.Errormessage = "Either no request available or PO not mapped with the user.";
                //return PartialView("ErrorModal", obj);
            }
        }
    }

}