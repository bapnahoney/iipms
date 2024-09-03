using Abp.Domain.Entities.Auditing;


namespace HIPMS.Models;
public partial class UserSession : CreationAuditedEntity<long>
{
    public long UserId { get; set; }
    public long? TenantId { get; set; } = null;
    public string Name { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public bool IsAlreadyLoggedIn { get; set; } = false;
}

