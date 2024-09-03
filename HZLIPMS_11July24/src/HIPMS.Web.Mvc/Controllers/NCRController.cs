using Abp.AspNetCore.Mvc.Authorization;
using Abp.Runtime.Session;
using HIPMS.Authorization;
using HIPMS.Authorization.NCR;
using HIPMS.Controllers;
using HIPMS.Roles;
using HIPMS.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.Web.Controllers;
[AbpMvcAuthorize(PermissionNames.Pages_IC)]
public class NCRController : HIPMSControllerBase
{

    private readonly IAbpSession _abpSession;
    private readonly IRoleAppService _roleAppService;
    private readonly INCRService _iNCRService;
    public NCRController(IAbpSession abpSession, IRoleAppService roleAppService, INCRService nCRService)
    {
        _roleAppService = roleAppService;
        _abpSession = abpSession;
        _iNCRService = nCRService;
    }

    public async Task<IActionResult> Index()
    {
        var listNCR = await _iNCRService.GetAllAsync();
        if (listNCR is null || listNCR.Count <= 0)
        {
            ErrorMessage obj = new ErrorMessage();
            obj.Errormessage = "No data found.";
            return PartialView("../Shared/ErrorModal", obj);
        }
        return View(listNCR);
        //CrudNCRModel obj = new();
        //return View("Create",obj);
    }
    public async Task<ActionResult> Create(CrudNCRModel input)
    {
        string userRole = await _roleAppService.GetUserRoleName();
        if (userRole == ClientRoles.vendor.ToString())
        {
            ErrorMessage err = new ErrorMessage();
            err.Errormessage = "Vendor cannot create NCR request.";
            return PartialView("../Shared/ErrorModal", err);
        }
        if (input == null || string.IsNullOrEmpty(input.PONumber) || string.IsNullOrEmpty(input.NCRDescription))
        {
            CrudNCRModel obj = new();
            return View("Create", obj);
        }
        else
        {
            var obj = await _iNCRService.CreateAsync(input);
            return View("/Views/NCR/Index.cshtml", obj);
        }
    }
    public async Task<ActionResult> GetAllAsync()
    {
        var listNCR = await _iNCRService.GetAllAsync();
        if (listNCR is null || listNCR.Count <= 0)
        {
            ErrorMessage err = new ErrorMessage();
            err.Errormessage = "No data found.";
            return PartialView("../Shared/ErrorModal", err);
        }
        return View(listNCR);

    }
    public async Task<ActionResult> Edit(long id)
    {
        var objNCR = await _iNCRService.GetAsync(id);
        if (objNCR is null)
        {
            ErrorMessage err = new ErrorMessage();
            err.Errormessage = "No data found.";
            return PartialView("../Shared/ErrorModal", err);
        }
        return View("Edit", objNCR);
    }
    public async Task<ActionResult> EditRequest(CrudNCRModel input)
    {
        //string userRole = await _roleAppService.GetUserRoleName();
        //if (userRole == ClientRoles.vendor.ToString())
        //{
        //    ErrorMessage err = new ErrorMessage();
        //    err.Errormessage = "Vendor cannot create NCR request.";
        //    return PartialView("../Shared/ErrorModal", err);
        //}
        //if (input == null || string.IsNullOrEmpty(input.PONumber) || string.IsNullOrEmpty(input.NCRDescription))
        //{
        //    CrudNCRModel obj = new();
        //    return View("Create", obj);
        //}
        if (input == null)
        {
            ErrorMessage err = new ErrorMessage();
            err.Errormessage = "Object cannot be null.";
            return PartialView("../Shared/ErrorModal", err);
        }
        else
        {
            if (HttpContext.Request.Form["Update"].Count == 1)
            {
                input.Status = NCRStatus.UPDATE;
            }
            else if (HttpContext.Request.Form["Close"].Count == 1)
            {
                input.Status = NCRStatus.CLOSE;
            }
            else
            {
                input.Status = NCRStatus.OPEN;
            }

            var obj = await _iNCRService.UpdateAsync(input);
            return View("/Views/NCR/Index.cshtml", obj);
            //return View(obj);
        }
    }
}
