using HIPMS.Controllers;
using HIPMS.Services;
using HIPMS.Shared;
using Microsoft.AspNetCore.Mvc;

namespace HIPMS.Web.Controllers;

public class EmailTriggerController : HIPMSControllerBase
{
    private readonly IEmailTriggerService _emailTriggerService;

    public EmailTriggerController(IEmailTriggerService emailTriggerService)
    {
        _emailTriggerService = emailTriggerService;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ForgotPassword([FromForm] ForgotPasswordInput input)
    {
        _emailTriggerService.SendMailOnForgotPassword(input);
        //return View("Account/Login");
        //return View("Login");
        return RedirectToAction("Login", "Account");
    }
}
