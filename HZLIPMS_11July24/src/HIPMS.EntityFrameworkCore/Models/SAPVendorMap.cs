using Abp.Domain.Entities.Auditing;
using HIPMS.Authorization.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace HIPMS.Models;


public partial class SAPVendorMap : CreationAuditedEntity<long>
{
    public SAPVendorMap()
    {
        User = new User();
    }
    public long UserId { get; set; }
    [ForeignKey("UserId")]
    public string VendorNo { get; set; } = string.Empty;
    public virtual User User { get; set; }
}