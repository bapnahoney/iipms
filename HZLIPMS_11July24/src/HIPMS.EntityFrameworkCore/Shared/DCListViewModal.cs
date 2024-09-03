using System;
using System.Collections.Generic;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.Shared;

public class DCListViewModal
{
    public DCListViewModal()
    {
        DCItems = new List<DCItemsData>();
    }
    public long Id { get; set; }
    public string PO { get; set; } = string.Empty;
    public string VendorNo { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string DispatchNo { get; set; } = string.Empty;
    public string DispatchMode { get; set; } = string.Empty;
    public string VendorRemark { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ManufacturerName { get; set; } = string.Empty;
    public string ManufacturerPlantAddress { get; set; } = string.Empty;
    public List<DCItemsData> DCItems { get; set; }
}
public class DCItemsData
{
    public long DispatchClearanceId { get; set; }
    public string PO { get; set; } = string.Empty;
    public long POMasterId { get; set; }
    public string? ItemNo { get; set; }
    public string? MaterialNo { get; set; }
    public string? MaterialDescription { get; set; }
    public float POQty { get; set; }
    public float DCPreviousQty { get; set; }
    public float DCBalanceQty { get; set; }
    public float DCInputQty { get; set; }//this qty should be less than balance qty
    public DCRequestStatus Status { get; set; }
    public POMaterialClass MaterialClass { get; set; }
    public string? ServiceNo { get; set; }
    public string? ServiceDescription { get; set; }
    public decimal? Quantity { get; set; }
    public long ClientInspectionById { get; set; }
    public string? ClientInspectionBy { get; set; }//email
    public long ContractorInspectionById { get; set; }
    public string? ContractorInspectionBy { get; set; }//email
    public long OtherInspectionById { get; set; }
    public string? OtherInspectionBy { get; set; }//email
    public string? InspectionSummary { get; set; }
    public DateTime? ClientInspectionDoneOn { get; set; }
    public DateTime? ContractorInspectionDoneOn { get; set; }
    public DateTime? OtherInspectionDoneOn { get; set; }
    public string? Approver { get; set; }
    public DateTime? ApprovedOn { get; set; }
    public string? Rejecter { get; set; }
    public DateTime? RejectedOn { get; set; }
    public string? Referrer { get; set; }
    public DateTime? ReferedOn { get; set; }
    public string? DispatchCompanyGST { get; set; }
}