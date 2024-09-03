using System;
using System.Collections.Generic;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.Web.Models.IC;

public class ICListViewModel
{
    public ICListViewModel()
    {
        ICItems = new List<ICItemsData>();
    }
    public long Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string PO { get; set; } = string.Empty;
    public string VendorNo { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string VendorRemark { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ManufacturerName { get; set; } = string.Empty;
    public string ManufacturerPlantAddress { get; set; } = string.Empty;
    public string POType { get; set; } = string.Empty;
    public bool HasPrev { get; set; } = false;
    public bool HasNext { get; set; } = false;
    public int CurrentPage { get; set; } = 0;
    public int TotalPage { get; set; } = 0;

    public List<ICItemsData> ICItems { get; set; }

}
public class ICItemsData
{

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
    public DateTime? InspectionDoneOn { get; set; }//HZL user will select while approval
    public ICReqStatus StatusList { get; set; }
}
public class ICErrorModel
{
    public string Errormessage { get; set; } = string.Empty;
}

public class ICExcelReport
{
    public string PO { get; set; } = string.Empty;
    public string POType { get; set; } = string.Empty;
    public string VendorNo { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string VendorRemark { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string? ItemNo { get; set; }
    public string? MaterialNo { get; set; }
    public string? MaterialDescription { get; set; }
    public float POQty { get; set; }
    public float ICPreviousQty { get; set; }
    public float ICBalanceQty { get; set; }
    public float ICInputQty { get; set; }//this qty should be less than balance qty
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
    public string? MaterialClassValue { get; set; }
    public string? StatusValue { get; set; }
    public DateTime? InspectionDoneOn { get; set; }//HZL user will select while approval
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string PartNo { get; set; } = string.Empty;
}