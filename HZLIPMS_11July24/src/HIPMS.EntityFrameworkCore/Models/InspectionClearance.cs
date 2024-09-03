using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HIPMS.Models;

public partial class InspectionClearance : CreationAuditedEntity<long>
{
    public InspectionClearance()
    {
        ICData = new HashSet<ICData>();
    }

    public long POMasterId { get; set; }
    [ForeignKey("POMasterId")]
    public string VendorNo { get; set; } = string.Empty;
    [MaxLength(50)]
    public string VendorName { get; set; } = string.Empty;
    [MaxLength(100)]
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
    public string UserActionRemark { get; set; } = string.Empty;
    public string OECActionRemark { get; set; } = string.Empty;
    public bool IsApprovedByOEC { get; set; } = false;
    public virtual ICollection<ICData> ICData { get; set; }
    public virtual POMaster POMaster { get; set; }

}
public partial class ICData : CreationAuditedEntity<long>
{
    public ICData()
    {
        ICDocuments = new HashSet<ICDocument>();
    }

    public long InspectionClearanceId { get; set; }
    [ForeignKey("InspectionClearanceId")]
    public long POMasterId { get; set; }
    [ForeignKey("POMasterId")]

    public string PONo { get; set; } = string.Empty;
    public string? ItemNo { get; set; }
    public string? MaterialNo { get; set; }
    public string? MaterialDescription { get; set; }
    public float POQty { get; set; }
    public float ICPreviousQty { get; set; }
    public float ICBalanceQty { get; set; }
    public float ICInputQty { get; set; }//this qty should be less than balance qty
    public long InspectionById { get; set; }
    public string? InspectionBy { get; set; }//email
    public string? InspectionSummary { get; set; }
    public int? Status { get; set; }
    // public string? StatusValue { get; set; }
    public int? MaterialClass { get; set; }
    // public string? MaterialClassValue { get; set; }
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
    public virtual InspectionClearance InspectionClearance { get; set; }
    public virtual POMaster POMaster { get; set; }
    public virtual ICollection<ICDocument> ICDocuments { get; set; }
}
public partial class ICDocument : CreationAuditedEntity<long>
{
    public long InspectionClearanceId { get; set; }
    [ForeignKey("InspectionClearanceId")]
    public long ICDataId { get; set; }
    [ForeignKey("ICDataId")]
    public int DocumentType { get; set; }
    public string? DocumentLocation { get; set; }
    public virtual InspectionClearance InspectionClearance { get; set; }
    public virtual ICData ICData { get; set; }
}