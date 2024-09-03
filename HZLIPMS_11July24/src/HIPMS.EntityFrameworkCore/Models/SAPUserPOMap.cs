using Abp.Domain.Entities.Auditing;
using HIPMS.Authorization.Users;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


namespace HIPMS.Models;
[Index(nameof(UserId), nameof(POId), IsUnique = true)]
public partial class SAPUserPOMap : CreationAuditedEntity<long>
{
    //public SAPUserPOMap()
    //{
    //    User = new User();
    //    PO = new POMaster();
    //}
    [ForeignKey("UserId")]
    public long UserId { get; set; }

    [ForeignKey("POId")]
    public long POId { get; set; }
    public bool IsVendor { get; set; } = false;
    public string? VendorNo { get; set; }
    public string? PONo { get; set; }
    public bool IsApprover { get; set; } = false; //hzl user can approve this po request(IC/RFI) or not
    public virtual User User { get; set; }
    public virtual POMaster PO { get; set; }
}