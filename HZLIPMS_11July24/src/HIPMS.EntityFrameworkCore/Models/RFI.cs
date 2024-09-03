using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HIPMS.Models;
//Request for Inspection
public partial class RFI : CreationAuditedEntity<long>
{
    public RFI()
    {
        RFIData = new HashSet<RFIData>();

    }

    public long POMasterId { get; set; }
    [ForeignKey("POMasterId")]
    [MaxLength(50)]
    public string VendorNo { get; set; } = string.Empty;
    [MaxLength(50)]
    public string VendorName { get; set; } = string.Empty;
    [MaxLength(75)]
    public string VendorRemark { get; set; } = string.Empty;
    [MaxLength(50)]
    public string ProjectName { get; set; } = string.Empty;
    [MaxLength(50)]
    public string ManufacturerName { get; set; } = string.Empty;
    [MaxLength(50)]
    public string ManufacturerPlantAddress { get; set; } = string.Empty;
    public int Status { get; set; }
    public int SessionID { get; set; }
    public int SyncStatus { get; set; }
    public DateTime SyncOn { get; set; }
    public int SyncCount { get; set; }
    public string UserActionRemark { get; set; } = string.Empty;//hzl&Admin
    public bool IsApprovedByOEC { get; set; } = false;
    public string OECActionRemark { get; set; } = string.Empty;//oec
    public virtual ICollection<RFIData> RFIData { get; set; }
    public virtual POMaster POMaster { get; set; }

}
public partial class RFIData : CreationAuditedEntity<long>
{
    public RFIData()
    {
        RFIDocuments = new HashSet<RFIDocument>();
    }
    public long RFIId { get; set; }
    [ForeignKey("RFIId")]
    public long POMasterId { get; set; }
    [ForeignKey("POMasterId")]
    public string PONo { get; set; } = string.Empty;
    public string? ItemNo { get; set; }
    public string? MaterialNo { get; set; }
    public string? MaterialDescription { get; set; }
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
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string PartNo { get; set; } = string.Empty;
    public virtual RFI RFI { get; set; }
    public virtual POMaster POMaster { get; set; }
    public virtual ICollection<RFIDocument> RFIDocuments { get; set; }
}
public partial class RFIDocument : CreationAuditedEntity<long>
{
    public long RFIId { get; set; }
    [ForeignKey("RFIId")]
    public long RFIDataId { get; set; }
    [ForeignKey("RFIDataId")]

    public int DocumentType { get; set; }
    public string? DocumentLocation { get; set; }
    public virtual RFI RFI { get; set; }
    public virtual RFIData RFIData { get; set; }
}