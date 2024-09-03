using Abp.AspNetCore.Mvc.Authorization;
using HIPMS.Controllers;
using HIPMS.Dashboard;
using HIPMS.Web.Models.Dashboard;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace HIPMS.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : HIPMSControllerBase
    {
        private readonly IDashboardService _iDashboardService;

        public HomeController(IDashboardService iDashboardService)
        {
            _iDashboardService = iDashboardService;
        }
        public ActionResult Index()
        {
            return DashboardView(new DashboardViewModel());
        }
        #region Dashboard

        private ActionResult DashboardView(DashboardViewModel model)
        {
            CancellationToken cancellationToken = new();
            var sPResP = _iDashboardService.GetAllAsync(cancellationToken).Result;
            if (sPResP != null)
            {
                model.ApprovedIC = sPResP.ApprovedIC;
                model.PendingIC = sPResP.PendingIC;
                model.RejectedIC = sPResP.RejectedIC;
                model.ReferredIC = sPResP.ReferredIC;
                //
                model.ApproveDC = sPResP.ApprovedDC;
                model.PendingDC = sPResP.PendingDC;
                model.RejectedDC = sPResP.RejectedDC;
                model.ReferredIC = sPResP.ReferredIC;
                //
                model.ApproveRFI = sPResP.ApprovedRFI;
                model.PendingRFI = sPResP.PendingRFI;
                model.RejectedRFI = sPResP.RejectedRFI;
                model.ReferredRFI = sPResP.ReferredRFI;
                //
                //TODO for ncr
                //
                model.OpenNCR = sPResP.OpenNCR;
                model.CloseNCR = sPResP.CloseNCR;
                model.PendingNCR = sPResP.PendingNCR;
                model.Other = sPResP.Other;

            }
            return View(model);
        }


        #endregion
    }
}
