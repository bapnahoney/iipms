using HIPMS.Models;
using System;
using System.Collections.Generic;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.Shared;

public class RFIEditRequest
{
    public RFIEditRequest()
    {
        RFIItems = new List<RFIItemEditRequest>();
    }
    public long Id { get; set; }
    public int Status { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string UserAction { get; set; } = string.Empty;
    public string VendorRemark { get; set; } = string.Empty;
    public string UserActionRemark { get; set; } = string.Empty;
    public string OECActionRemark { get; set; } = string.Empty;//oec
    public bool IsApprovedByOEC { get; set; }
    public OpenNCROnPO OpenNCR { get; set; }
    public List<RFIItemEditRequest> RFIItems { get; set; }
}
public class RFIItemEditRequest
{
    public RFIItemEditRequest()
    {

    }
    public long Id { get; set; }
    public long RFIId { get; set; }
    public long POMasterId { get; set; }
    public string PONo { get; set; } = string.Empty;
    public string? ItemNo { get; set; }
    public string? MaterialNo { get; set; }
    public string? MaterialDescription { get; set; }
    public POMaterialClass MaterialClassList { get; set; }
    public float POQty { get; set; }
    public float PreviousQty { get; set; }
    public float BalanceQty { get; set; }
    public float InputQty { get; set; }//this qty should be less than balance qty
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
    public DateTime? InspectionDoneOn { get; set; }//HZL user will select while approval
    public float? ServiceQty { get; set; }
    public string? ServiceUOM { get; set; }
    //public string Make { get; set; } = string.Empty;
    //public string Model { get; set; } = string.Empty;
    //public string PartNo { get; set; } = string.Empty;
    public RFIReqStatus StatusList { get; set; }
    public List<RFIDocument> RFIDocuments { get; set; } = new();
}
public class OpenNCROnPO
{
    public int Last7days { get; set; } = 0;
    public int Last7To30days { get; set; } = 0;
    public int Befor30Days { get; set; } = 0;
}
public class SAPRFIEdit
{
    public SAPRFIEdit()
    {
        T_RFI_DATA = new();
    }
    public string AUTH_KEY { get; set; }
    public string PO_NO { get; set; }
    public string VENDOR_NO { get; set; }
    public string NOTE { get; set; }
    public string RFI_DESC { get; set; }

    public string RFI_APP_DATE { get; set; }
    public long RFI_NO { get; set; }
    public string RFI_SUBMIT_DATE { get; set; }
    public RFI_DATA T_RFI_DATA { get; set; }
}
public class RFI_DATA
{
    public RFI_DATA()
    {
        item = new List<SAPRFIItem>();
    }
    public List<SAPRFIItem> item { get; set; }
}

public class SAPRFIItem
{
    public string ITEM_NO { get; set; }
    public string MATERIAL_NO { get; set; }
    //public float PO_QUAN { get; set; }
    public string PO_QUAN { get; set; }
    public string APP_RFI_QTY { get; set; }
    public string APP_EMP_ID { get; set; }
    public string APP_REMARK { get; set; }
    public string APP_DATE_TIME { get; set; }
    public string SERVICE_NO { get; set; }
    public string SERVICE_DESC { get; set; }
    public string? SERVICE_QUAN { get; set; }
    public string SERVICE_UOM { get; set; }
}

public class RFIExcelReport
{
    public string PO { get; set; } = string.Empty;
    public string POType { get; set; } = string.Empty;
    public string VendorNo { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string VendorRemark { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string? ItemNo { get; set; }
    public float POQty { get; set; }
    public float PreviousQty { get; set; }
    public float BalanceQty { get; set; }
    public float InputQty { get; set; }//this qty should be less than balance qty
    public string? InspectionBy { get; set; }//email
    public string? InspectionSummary { get; set; }
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
    public float? ServiceQty { get; set; }
    public string? ServiceUOM { get; set; }
    public string? StatusValue { get; set; }
    public DateTime? InspectionDoneOn { get; set; }//HZL user will select while approval

}