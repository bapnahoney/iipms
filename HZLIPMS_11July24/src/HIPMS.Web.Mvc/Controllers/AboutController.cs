using Abp.AspNetCore.Mvc.Authorization;
using HIPMS.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace HIPMS.Web.Controllers
{
    [AbpMvcAuthorize]
    public class AboutController : HIPMSControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
