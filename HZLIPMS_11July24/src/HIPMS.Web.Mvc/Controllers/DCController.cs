using Abp.Runtime.Session;
using HIPMS.Controllers;
using HIPMS.DC;
using HIPMS.IC.Dto;
using HIPMS.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.Web.Controllers;

public class DCController : HIPMSControllerBase
{
    private readonly IDispatchClearanceService _dcAppService;
    private readonly IAbpSession _abpSession;
    public DCController(IDispatchClearanceService dcAppService, IAbpSession abpSession)
    {
        _dcAppService = dcAppService;
        _abpSession = abpSession;
    }

    public async Task<IActionResult> Index()
    {
        PagedICResultRequestDto inputObj = new();
        inputObj.MaxResultCount = 10;
        var resObj = await _dcAppService.GetAllAsync(inputObj);
        List<DCListViewModal> viewObj = new List<DCListViewModal>();
        foreach (var reqObj in resObj)
        {
            DCListViewModal viewResModel = new();
            viewResModel.Id = reqObj.Id;
            //viewResModel.PO = reqObj.DCData.FirstOrDefault().POMaster.PONo;
            viewResModel.VendorName = reqObj?.VendorName;
            viewResModel.VendorNo = reqObj?.VendorNo;
            viewResModel.VendorRemark = reqObj?.VendorRemark;
            viewResModel.ManufacturerPlantAddress = reqObj?.ManufacturerPlantAddress;
            viewResModel.ManufacturerName = reqObj?.ManufacturerName;

            foreach (var item in reqObj.DCData)
            {
                DCItemsData obj = new DCItemsData();
                obj.DispatchClearanceId = item.DispatchClearanceId;
                obj.PO = viewResModel.PO;
                obj.ItemNo = item.ItemNo;
                obj.DCPreviousQty = item.DCPreviousQty;
                obj.DCBalanceQty = item.DCBalanceQty;
                obj.DCInputQty = item.DCInputQty;
                obj.POQty = item.POQty;
                obj.MaterialNo = item.MaterialNo;
                obj.Status = (DCRequestStatus)item.Status;
                //obj.StatusValue = ((DCRequestStatus)item.Status).ToString();
                //obj.UOM = item.UOM;
                obj.MaterialDescription = item.MaterialDescription;
                //obj.MaterialClassValue = item.MaterialClassValue;
                obj.MaterialClass = (POMaterialClass)item.MaterialClass;
                //obj.MaterialClassList = (POMaterialClass)item.MaterialClass;
                viewResModel.DCItems.Add(obj);
            }
            viewObj.Add(viewResModel);
        }
        return View(viewObj);
    }

    [HttpGet]
    public List<DCListViewModal> getAllDC()
    {
        PagedICResultRequestDto inputObj = new();
        inputObj.MaxResultCount = 10;
        var resObj = _dcAppService.GetAllAsync(inputObj);
        List<DCListViewModal> viewObj = new List<DCListViewModal>();
        foreach (var reqObj in resObj.Result)
        {
            DCListViewModal viewResModel = new();
            viewResModel.Id = reqObj.Id;
            viewResModel.PO = "8510001294";// reqObj.DCData.FirstOrDefault().POMaster.PONo;
            viewResModel.VendorName = reqObj?.VendorName;
            viewResModel.VendorNo = reqObj?.VendorNo;
            viewResModel.VendorRemark = reqObj?.VendorRemark;
            viewResModel.ManufacturerPlantAddress = reqObj?.ManufacturerPlantAddress;
            viewResModel.ManufacturerName = reqObj?.ManufacturerName;

            foreach (var item in reqObj.DCData)
            {
                DCItemsData obj = new DCItemsData();
                obj.DispatchClearanceId = item.DispatchClearanceId;
                obj.PO = viewResModel.PO;
                obj.ItemNo = item.ItemNo;
                obj.DCPreviousQty = item.DCPreviousQty;
                obj.DCBalanceQty = item.DCBalanceQty;
                obj.DCInputQty = item.DCInputQty;
                obj.POQty = item.POQty;
                obj.MaterialNo = item.MaterialNo;
                obj.Status = (DCRequestStatus)item.Status;
                //obj.StatusValue = ((DCRequestStatus)item.Status).ToString();
                //obj.UOM = item.UOM;
                obj.MaterialDescription = item.MaterialDescription;
                //obj.MaterialClassValue = item.MaterialClassValue;
                obj.MaterialClass = (POMaterialClass)item.MaterialClass;
                //obj.MaterialClassList = (POMaterialClass)item.MaterialClass;
                viewResModel.DCItems.Add(obj);
            }
            viewObj.Add(viewResModel);
        }
        return viewObj;
    }
}
