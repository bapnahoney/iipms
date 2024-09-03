using System.ComponentModel.DataAnnotations;

namespace HIPMS.Shared;

public class SharedEnum
{
    //public enum ICReqStatus
    //{
    //    Pending = 1,
    //    Approved = 2,
    //    Referred = 3,
    //    Rejected = 4
    //}
    public enum POType
    {
        ZPIS = 1,
        ZPDS = 2,
        ZPDM = 3,
        ZPIM = 4
    }
    public enum ICReqStatus
    {
        Pending = 1,
        Approve = 2,
        Refer = 3,
        Reject = 4,
        OECApprove = 5
    }
    public enum DCRequestStatus
    {
        Pending = 1,
        Approved = 2,
        Referred = 3,
        Rejected = 4
    }
    //public enum RFIReqStatus
    //{
    //    Pending = 1,
    //    Approved = 2,
    //    Referred = 3,
    //    Rejected = 4
    //}
    public enum RFIReqStatus
    {
        Pending = 1,
        Approve = 2,
        Refer = 3,
        Reject = 4,
        OECApprove = 5,
        Invalid =6 //case when decrease qty in s4h
    }
    public static string GetEnumDisplayValue(int? x)
    {
        if (x == null) x = 0;

        switch (x)
        {
            case 1:
                return "Pending";
            case 2:
                return "Approved";
            case 3:
                return "Referred";
            case 4:
                return "Rejected";
            case 5:
                return "OECApproved";
            default:
                return "Pending";
        }
    }
    public enum POMaterialClass
    {
        [Display(Name = "A")]
        A = 1,
        [Display(Name = "B")]
        B = 2,
        [Display(Name = "C")]
        C = 3
    }
    public enum DocumentModuleType
    {
        [Display(Name = "IC")]
        IC = 1,
        [Display(Name = "RFI")]
        RFI = 2,
        [Display(Name = "NCR")]
        NCR = 3,
        [Display(Name = "Other")]
        Other = 4
    }
    public enum DocumentType
    {
        [Display(Name = "Default")]
        Default = 1,

    }
    public enum Discipline
    {
        [Display(Name = "Civil")]
        Civil = 1,
        [Display(Name = "Mechanical")]
        RFI = 2,
        [Display(Name = "E&I")]
        EI = 3,
        [Display(Name = "Process")]
        Process = 4,
        [Display(Name = "Other")]
        Other = 5
    }
    public enum Criticality
    {
        [Display(Name = "Low")]
        Low = 1,
        [Display(Name = "Medium")]
        Medium = 2,
        [Display(Name = "High")]
        High = 3,
        [Display(Name = "Other")]
        Other = 4,
    }
    public enum ClientRoles
    {
        [Display(Name = "hzl")]
        hzl = 1,
        [Display(Name = "vendor")]
        vendor = 2,
        [Display(Name = "oec")]
        oec = 3,
        [Display(Name = "admin")]
        admin = 4,
    }
    public enum NCRStatus
    {
        [Display(Name = "OPEN")]
        OPEN = 1,
        [Display(Name = "CLOSE")]
        CLOSE = 2,
        [Display(Name = "UPDATE")]
        UPDATE = 3,
        [Display(Name = "Other")]
        Other = 4
    }
    public enum UserAction
    {
        [Display(Name = "Login")]
        Login = 1,
        [Display(Name = "Logout")]
        Logout = 2
    }
}
