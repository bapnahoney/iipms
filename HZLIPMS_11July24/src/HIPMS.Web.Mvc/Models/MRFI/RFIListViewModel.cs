using System;
using System.Collections.Generic;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.Web.Models.MRFI;

public class RFIListViewModel
{
    public RFIListViewModel()
    {
        RFIItems = new List<RFIItemsData>();
    }
    public long Id { get; set; }
    public string PO { get; set; } = string.Empty;
    public string VendorNo { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string VendorRemark { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    //public string ManufacturerName { get; set; } = string.Empty;
    //public string ManufacturerPlantAddress { get; set; } = string.Empty;
    public string POType { get; set; } = string.Empty;

    public bool HasPrev { get; set; } = false;
    public bool HasNext { get; set; } = false;
    public int CurrentPage { get; set; } = 1;
    public int TotalPage { get; set; } = 1;
    public List<RFIItemsData> RFIItems { get; set; }

}
public class RFIItemsData
{

    public long RFIId { get; set; }
    public long POMasterId { get; set; }
    public string PONo { get; set; } = string.Empty;
    public string? ItemNo { get; set; }
    //public string? MaterialNo { get; set; }
    public string? MaterialDescription { get; set; }
    public POMaterialClass MaterialClassList { get; set; }
    public float POQty { get; set; }
    public float PreviousQty { get; set; }
    public float? BalanceQty { get; set; }
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
    public RFIReqStatus StatusList { get; set; }
}