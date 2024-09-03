namespace HIPMS.Dashboard.Dto;

public class DashboardStatisticsSPResDto
{
    public int PendingIC { get; set; } = 0;
    public int ApprovedIC { get; set; } = 0;
    public int ReferredIC { get; set; } = 0;
    public int RejectedIC { get; set; } = 0;
    public int PendingDC { get; set; } = 0;
    public int ApprovedDC { get; set; } = 0;
    public int ReferredDC { get; set; } = 0;
    public int RejectedDC { get; set; } = 0;
    public int PendingRFI { get; set; } = 0;
    public int ApprovedRFI { get; set; } = 0;
    public int ReferredRFI { get; set; } = 0;
    public int RejectedRFI { get; set; } = 0;
    public int OpenNCR { get; set; } = 0;
    public int CloseNCR { get; set; } = 0;
    public int PendingNCR { get; set; } = 0;
    public int Other { get; set; } = 0;

}
