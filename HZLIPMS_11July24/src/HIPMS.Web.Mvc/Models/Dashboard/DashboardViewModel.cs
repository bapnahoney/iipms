namespace HIPMS.Web.Models.Dashboard
{
    public class DashboardViewModel
    {
        public int PendingIC { get; set; }
        public int ApprovedIC { get; set; }
        public int ReferredIC { get; set; }
        public int RejectedIC { get; set; }

        public int PendingDC { get; set; }
        public int ApproveDC { get; set; }
        public int ReferredDC { get; set; }
        public int RejectedDC { get; set; }

        public int PendingRFI { get; set; }
        public int ApproveRFI { get; set; }
        public int ReferredRFI { get; set; }
        public int RejectedRFI { get; set; }

        public int PendingNCR { get; set; }
        public int OpenNCR { get; set; }
        public int CloseNCR { get; set; }
        public int Other { get; set; }

    }
}
