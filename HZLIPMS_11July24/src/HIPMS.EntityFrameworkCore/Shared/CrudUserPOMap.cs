using System.Collections.Generic;
using System.ComponentModel;


namespace HIPMS.Shared;

public class CrudUserPOMap
{
    [DisplayName("User")]
    public long UserId { get; set; }
    public List<UserDD> UserDD { get; set; } = new();
    [DisplayName("PONumber")]
    public long POId { get; set; }
    public List<PODD> PODD { get; set; } = new();
    public bool IsVendor { get; set; } = false;
    public string? VendorNo { get; set; }
    public string? UserEmail { get; set; }
    public string? PONo { get; set; }
    public bool IsApprover { get; set; } = false;
}
public class ViewUserPOMap
{
    public string? PONo { get; set; }
    public string? UserEmail { get; set; }
}
//public class CrudUserPOMap
//{
//    public long UserId { get; set; }
//    public List<PODD> PODD { get; set; } 
//    public List<UserDD> UserDD { get; set; }
//}
public class UserDD
{
    public long Id { get; set; }
    public string UserEmail { get; set; } = string.Empty;
}
public class PODD
{
    public long Id { get; set; }
    public string PONumber { get; set; } = string.Empty;

}