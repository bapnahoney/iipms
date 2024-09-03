using HIPMS.Models;
using System;
using System.Collections.Generic;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.Shared;

public class ICEditRequest
{
    public ICEditRequest()
    {
        ICItems = new List<ICItemEditRequest>();
    }
    public long Id { get; set; }
    public int Status { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string UserAction { get; set; } = string.Empty;
    public string VendorRemark { get; set; } = string.Empty;
    public string UserActionRemark { get; set; } = string.Empty;
    public string OECActionRemark { get; set; } = string.Empty;
    public bool IsApprovedByOEC { get; set; }
    public List<ICItemEditRequest> ICItems { get; set; }
}
public class ICItemEditRequest
{
    public ICItemEditRequest()
    {
        ICDocuments = new List<ICDocument>();
    }
    public long Id { get; set; }

    public long InspectionClearanceId { get; set; }
    public long POMasterId { get; set; }
    public string PONo { get; set; } = string.Empty;
    public string? ItemNo { get; set; }
    public string? MaterialNo { get; set; }
    public string? MaterialDescription { get; set; }
    public POMaterialClass MaterialClassList { get; set; }
    public float POQty { get; set; }
    public float ICPreviousQty { get; set; }
    public float ICBalanceQty { get; set; }
    public float ICInputQty { get; set; }//this qty should be less than balance qty
    public long InspectionById { get; set; }
    public string? InspectionBy { get; set; }//email
    public string? InspectionSummary { get; set; }
    public int? Status { get; set; }
    public int? MaterialClass { get; set; }
    public string? ServiceNo { get; set; }
    public string? UOM { get; set; }
    public string? ServiceDescription { get; set; }
    public decimal? Quantity { get; set; }
    public string? Approver { get; set; }
    public DateTime? ApprovedOn { get; set; }
    public string? Rejecter { get; set; }
    public DateTime? RejectedOn { get; set; }
    public string? Referrer { get; set; }
    public DateTime? ReferedOn { get; set; }
    public string? MaterialClassValue { get; set; }
    public string? StatusValue { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string PartNo { get; set; } = string.Empty;
    public DateTime? InspectionDoneOn { get; set; }//HZL user will select while approval

    public ICReqStatus StatusList { get; set; }
    public List<ICDocument> ICDocuments { get; set; }
}

public class SAPICEdit
{
    public SAPICEdit()
    {
        IC_DATA = new();
    }
    public string AUTH_KEY { get; set; }
    public string PO_NO { get; set; }
    public string VENDOR_NO { get; set; }
    public string NOTE { get; set; }
    public string IC_APProval_DATE { get; set; }
    public long IC_NO { get; set; }
    public string IC_SUBMIT_DATE { get; set; }
    public IC_DATA IC_DATA { get; set; }
}
public class IC_DATA
{
    public IC_DATA()
    {
        item = new List<SAPICItem>();
    }
    public List<SAPICItem> item { get; set; }
}

public class SAPICItem
{
    public string ITEM_NO { get; set; }
    public string MATERIAL_NO { get; set; }
    public string PO_QUAN { get; set; }
    public string APP_IC_QTY { get; set; }
    public string APP_EMP_ID { get; set; }
    public string APP_REMARK { get; set; }
    public string APP_DATE_TIME { get; set; }
}