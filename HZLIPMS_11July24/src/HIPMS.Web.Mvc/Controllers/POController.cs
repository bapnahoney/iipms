using Abp.AspNetCore.Mvc.Authorization;
using Abp.UI;
using HIPMS.Authorization;
using HIPMS.Authorization.PO;
using HIPMS.Authorization.PO.Dto;
using HIPMS.Controllers;
using HIPMS.IC.Dto;
using HIPMS.Models;
using HIPMS.Roles;
using HIPMS.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using static HIPMS.Shared.SharedEnum;


namespace HIPMS.Web.Controllers
{
    [AbpMvcAuthorize(PermissionNames.Pages_Admin)]
    public class POController : HIPMSControllerBase
    {
        private readonly IPOService _iPOService;
        private readonly IRoleAppService _roleAppService;

        public POController(IPOService iPOService, IRoleAppService roleAppService)
        {
            _iPOService = iPOService;
            _roleAppService = roleAppService;
        }
        // GET: POController
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //// GET: POController/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}


        //// GET: POController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: POController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: POController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: POController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: POController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: POController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //PO/PODetail/PONumber
        [HttpGet]
        [ValidateAntiForgeryToken]
        public object PODetail(string PONumber)
        {
            POResultRequestDto request = new()
            {
                PONumber = PONumber
            };

            try
            {
                object response = _iPOService.GetPODetailAsync(request).Result;
                return response;
                //return RedirectToAction(nameof(Index));
            }
            catch
            {
                return null;
            }
        }
        [HttpGet]
        public async Task<IActionResult> POUserDD()
        {
            try
            {
                var roleName = await _roleAppService.GetUserRoleName();
                //this section only mapped po can view by vendor
                if (roleName == ClientRoles.admin.ToString())
                {
                    var resObj = await _iPOService.GetUserPODDAsync();
                    //return View("Create", resObj);
                    return View("/Views/SAPPOUser/Create.cshtml", resObj);
                }
                else
                {
                    ErrorMessage err = new ErrorMessage();
                    err.Errormessage = "Invalid User.";
                    return PartialView("../Shared/ErrorModal", err);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage err = new ErrorMessage();
                err.Errormessage = ex.Message;
                return PartialView("../Shared/ErrorModal", err);
            }
        }

        [HttpGet]
        public async Task<IActionResult> View()
        {
            try
            {
                var roleName = await _roleAppService.GetUserRoleName();
                //this section only mapped po can view by vendor
                if (roleName == ClientRoles.admin.ToString())
                {
                    var resObj = await _iPOService.GetAllMappingAsync();
                    //return View("Create", resObj);
                    return View("/Views/SAPPOUser/Index.cshtml", resObj);
                }
                else
                {
                    ErrorMessage err = new ErrorMessage();
                    err.Errormessage = "Invalid User.";
                    return PartialView("../Shared/ErrorModal", err);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage err = new ErrorMessage();
                err.Errormessage = ex.Message;
                return PartialView("../Shared/ErrorModal", err);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePOMapRequest([FromForm] CrudUserPOMap input)
        {
            var roleName = _roleAppService.GetUserRoleName();
            //this section only mapped po can view by vendor
            if (roleName.Result == ClientRoles.admin.ToString())
            {
                if (input == null)
                {
                    ErrorMessage err = new ErrorMessage();
                    err.Errormessage = "Object cannot be null.";
                    return PartialView("../Shared/ErrorModal", err);
                }
                else if (input.UserId <= 0 || string.IsNullOrEmpty(input.PONo))
                {
                    ErrorMessage err = new ErrorMessage();
                    err.Errormessage = "Invalid request.";
                    return PartialView("../Shared/ErrorModal", err);
                }
                else
                {
                    try
                    {
                        var obj = _iPOService.UserPOMapUpdateAsync(input).Result;
                        return View("/Views/SAPPOUser/Index.cshtml", obj);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage err = new ErrorMessage();
                        err.Errormessage = "Duplicate Mapping.";
                        return PartialView("../Shared/ErrorModal", err);
                    }
                }
            }
            else
            {
                ErrorMessage err = new ErrorMessage();
                err.Errormessage = "Invalid User.";
                return PartialView("../Shared/ErrorModal", err);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<POMapResponse> POSearch(string POnumber)
        {
            if (string.IsNullOrEmpty(POnumber) || POnumber.ToLower().Contains("enter"))
            {
                throw new UserFriendlyException(L("Invalid PO number."));
            }
            CancellationToken cancellationToken = new CancellationToken();
            SearchPORequest searchPORequest = new SearchPORequest();
            searchPORequest.PONumber = POnumber;
            POMaster SearchPODetailRes = await _iPOService.SearchPODetailAsync(searchPORequest, cancellationToken);
            if (SearchPODetailRes != null && SearchPODetailRes.Id > 0)
            {
                POMapResponse res = new()
                {
                    MESSEGE_CODE = "200",
                    MESSEGE_DESC = "Success"
                };
                return res;
            }
            return null;
        }
    }
}
