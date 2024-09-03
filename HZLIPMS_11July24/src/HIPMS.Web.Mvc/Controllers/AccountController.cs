using Abp;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Json;
using Abp.MultiTenancy;
using Abp.Notifications;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.Timing;
using Abp.UI;
using Abp.Web.Models;
using Abp.Zero.Configuration;
using DocumentFormat.OpenXml.InkML;
using HIPMS.Authentication.JwtBearer;
using HIPMS.Authorization;
using HIPMS.Authorization.Users;
using HIPMS.Controllers;
using HIPMS.EntityFrameworkCore;
using HIPMS.Identity;
using HIPMS.Models;
using HIPMS.MultiTenancy;
using HIPMS.Services.SendEmail;
using HIPMS.Sessions;
using HIPMS.Shared;
using HIPMS.Users;
using HIPMS.Web.Models.Account;
using HIPMS.Web.Views.Shared.Components.TenantChange;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace HIPMS.Web.Controllers
{
    public class AccountController : HIPMSControllerBase
    {
        private readonly HIPMSDbContext _db;
        private readonly UserManager _userManager;
        private readonly TenantManager _tenantManager;
        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly LogInManager _logInManager;
        private readonly SignInManager _signInManager;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly TokenAuthConfiguration _configuration;
        private readonly ISessionAppService _sessionAppService;
        private readonly ITenantCache _tenantCache;
        private readonly INotificationPublisher _notificationPublisher;
        private readonly ISendEmail _sendEmail;
        private readonly IUserAppService _userAppService;
        private readonly IAbpSession _abpSession;

        public AccountController(HIPMSDbContext db, 
        UserManager userManager,
            IMultiTenancyConfig multiTenancyConfig,
            TenantManager tenantManager,
            IUnitOfWorkManager unitOfWorkManager,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            LogInManager logInManager,
            SignInManager signInManager,
            UserRegistrationManager userRegistrationManager,
            TokenAuthConfiguration configuration,
            ISessionAppService sessionAppService, IAbpSession abpSession,
            ITenantCache tenantCache,
            INotificationPublisher notificationPublisher, ISendEmail sendEmail, IUserAppService userAppService)
        {
            _db = db;

            _userManager = userManager;
            _multiTenancyConfig = multiTenancyConfig;
            _tenantManager = tenantManager;
            _unitOfWorkManager = unitOfWorkManager;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _logInManager = logInManager;
            _signInManager = signInManager;
            _userRegistrationManager = userRegistrationManager;
            _sessionAppService = sessionAppService;
            _tenantCache = tenantCache;
            _notificationPublisher = notificationPublisher;
            _sendEmail = sendEmail;
            _userAppService = userAppService;
            _abpSession= abpSession;
            _configuration = configuration;
            
        }

        #region Login / Logout

        public ActionResult Login(string userNameOrEmailAddress = "", string returnUrl = "", string successMessage = "")
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = GetAppHomeUrl();
            }

            return View(new LoginFormViewModel
            {
                ReturnUrl = returnUrl,
                IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled,
                IsSelfRegistrationAllowed = IsSelfRegistrationEnabled(),
                MultiTenancySide = AbpSession.MultiTenancySide
            });
        }

        [HttpPost]
        [UnitOfWork]
        public virtual async Task<JsonResult> Login(LoginViewModel loginModel, string returnUrl = "", string returnUrlHash = "")
        {
            returnUrl = NormalizeReturnUrl(returnUrl);
            if (!string.IsNullOrWhiteSpace(returnUrlHash))
            {
                returnUrl = returnUrl + returnUrlHash;
            }

            HttpContext.Session.SetString("SessionVal","Session_"+ Guid.NewGuid());

            var loginResult = await GetLoginResultAsync(loginModel.UsernameOrEmailAddress, loginModel.Password, GetTenancyNameOrNull());

            var now = DateTime.Now;
            
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,                
                notBefore: now,
                claims: CreateJwtClaims(loginResult.Identity),
                expires: now.Add(_configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );

           //string tee =  new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

           // UserAccessToken userAccessToken = new UserAccessToken();
           // userAccessToken.TenantId = 1;
           // userAccessToken.UserId = loginResult.User.Id;
           // userAccessToken.Name = "Authori";
           // userAccessToken.Value = tee;
           // loginResult.User.Tokens.Add(userAccessToken);

            string sessionId = HttpContext.Session.GetString("SessionVal");// HttpContext.Session.Id;
            UserSession userSession = await _db.UserSession.Where(x => x.UserId == loginResult.User.Id).FirstOrDefaultAsync();
            if (userSession == null)
            {
                UserSession obj = new()
                {
                    Name = loginResult.User.Name,
                    TenantId = loginResult.User.TenantId,
                    UserId = loginResult.User.Id,
                    IsAlreadyLoggedIn = true,
                    SessionId = HttpContext.Session.GetString("SessionVal")
                };
                await _db.UserSession.AddAsync(obj);
                await _db.SaveChangesAsync();
            }
            else if (userSession.IsAlreadyLoggedIn)
            {
                AuditLog userLog = await _db.AuditLogs.Where(x => x.UserId == loginResult.User.Id)
                                .OrderByDescending(x => x.Id)
                                .Take(1)
                                //.Select(x => x.ExecutionTime)
                                .FirstOrDefaultAsync();

                if (userLog != null)
                {
                    //session handle using this (get last activity time)
                    //var hours = (DateTime.Now - userLog.ExecutionTime).TotalHours;
                    var minutes = (DateTime.Now - userLog.ExecutionTime).TotalMinutes;
                    if (minutes > 30)
                    {
                        userSession.SessionId = sessionId;
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        throw new UserFriendlyException(L("UserAlreadyLoggedIn"));
                    }
                }
            }
            else if (!userSession.IsAlreadyLoggedIn)
            {
                userSession.IsAlreadyLoggedIn = true;
                userSession.SessionId = sessionId;
                await _db.SaveChangesAsync();
            }
            await _signInManager.SignInAsync(loginResult.Identity, loginModel.RememberMe);
            await UnitOfWorkManager.Current.SaveChangesAsync();

            return Json(new AjaxResponse { TargetUrl = returnUrl });
        }

        public async Task<ActionResult> Logout()
        {
            UserSession userSession = await _db.UserSession.Where(x => x.UserId == _abpSession.UserId).FirstOrDefaultAsync();
            if (userSession != null)
            {
                userSession.IsAlreadyLoggedIn = false;
                await _db.SaveChangesAsync();
            }

            await _signInManager.SignOutAsync();

            try
            {
                Response.Cookies.Delete("Set-Cookie");
                HttpContext?.Request.Headers.Remove("Set-Cookie");
                HttpContext.Session.Clear();
                Request.Headers.Clear();
            }
            catch (Exception ex)
            {
                
            }
            //todo honey place logic for session remove/clear/dispose 
            return RedirectToAction("Login");
        }
        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }
        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        #endregion

        #region Register

        public ActionResult Register()
        {
            return RegisterView(new RegisterViewModel());
        }

        private ActionResult RegisterView(RegisterViewModel model)
        {
            ViewBag.IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled;

            return View("Register", model);
        }

        private bool IsSelfRegistrationEnabled()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return false; // No registration enabled for host users!
            }

            return true;
        }

        [HttpPost]
        [UnitOfWork]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                ExternalLoginInfo externalLoginInfo = null;
                if (model.IsExternalLogin)
                {
                    externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
                    if (externalLoginInfo == null)
                    {
                        throw new Exception("Can not external login!");
                    }

                    model.UserName = model.EmailAddress;
                    model.Password = Authorization.Users.User.CreateRandomPassword();
                }
                else
                {
                    if (model.UserName.IsNullOrEmpty() || model.Password.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException(L("FormIsNotValidMessage"));
                    }
                }

                var user = await _userRegistrationManager.RegisterAsync(
                    model.Name,
                    model.Surname,
                    model.EmailAddress,
                    model.UserName,
                    model.Password,
                    true // Assumed email address is always confirmed. Change this if you want to implement email confirmation.
                );

                // Getting tenant-specific settings
                var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

                if (model.IsExternalLogin)
                {
                    Debug.Assert(externalLoginInfo != null);

                    if (string.Equals(externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email), model.EmailAddress, StringComparison.OrdinalIgnoreCase))
                    {
                        user.IsEmailConfirmed = true;
                    }

                    user.Logins = new List<UserLogin>
                    {
                        new UserLogin
                        {
                            LoginProvider = externalLoginInfo.LoginProvider,
                            ProviderKey = externalLoginInfo.ProviderKey,
                            TenantId = user.TenantId
                        }
                    };
                }

                await _unitOfWorkManager.Current.SaveChangesAsync();

                Debug.Assert(user.TenantId != null);

                var tenant = await _tenantManager.GetByIdAsync(user.TenantId.Value);

                // Directly login if possible
                if (user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin))
                {
                    AbpLoginResult<Tenant, User> loginResult;
                    if (externalLoginInfo != null)
                    {
                        loginResult = await _logInManager.LoginAsync(externalLoginInfo, tenant.TenancyName);
                    }
                    else
                    {
                        loginResult = await GetLoginResultAsync(user.UserName, model.Password, tenant.TenancyName);
                    }

                    if (loginResult.Result == AbpLoginResultType.Success)
                    {
                        await _signInManager.SignInAsync(loginResult.Identity, false);
                        return Redirect(GetAppHomeUrl());
                    }

                    Logger.Warn("New registered user could not be login. This should not be normally. login result: " + loginResult.Result);
                }

                //Email
                try
                {
                    await _sendEmail.ExecuteSMTPAsync(user.EmailAddress, "New Account Registration", "Your account has been registered successfully. UserName: " + model.UserName + "Password:" + model.Password, CancellationToken.None);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                return View("RegisterResult", new RegisterResultViewModel
                {
                    TenancyName = tenant.TenancyName,
                    NameAndSurname = user.Name + " " + user.Surname,
                    UserName = user.UserName,
                    EmailAddress = user.EmailAddress,
                    IsEmailConfirmed = user.IsEmailConfirmed,
                    IsActive = user.IsActive,
                    IsEmailConfirmationRequiredForLogin = isEmailConfirmationRequiredForLogin
                });
            }
            catch (UserFriendlyException ex)
            {
                ViewBag.ErrorMessage = ex.Message;

                return View("Register", model);
            }
        }

        #endregion

        #region External Login

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action(
                "ExternalLoginCallback",
                "Account",
                new
                {
                    ReturnUrl = returnUrl
                });

            return Challenge(
                // TODO: ...?
                // new Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties
                // {
                //     Items = { { "LoginProvider", provider } },
                //     RedirectUri = redirectUrl
                // },
                provider
            );
        }

        [UnitOfWork]
        public virtual async Task<ActionResult> ExternalLoginCallback(string returnUrl, string remoteError = null)
        {
            returnUrl = NormalizeReturnUrl(returnUrl);

            if (remoteError != null)
            {
                Logger.Error("Remote Error in ExternalLoginCallback: " + remoteError);
                throw new UserFriendlyException(L("CouldNotCompleteLoginOperation"));
            }

            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                Logger.Warn("Could not get information from external login.");
                return RedirectToAction(nameof(Login));
            }

            await _signInManager.SignOutAsync();

            var tenancyName = GetTenancyNameOrNull();

            var loginResult = await _logInManager.LoginAsync(externalLoginInfo, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    await _signInManager.SignInAsync(loginResult.Identity, false);
                    return Redirect(returnUrl);
                case AbpLoginResultType.UnknownExternalLogin:
                    return await RegisterForExternalLogin(externalLoginInfo);
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                        loginResult.Result,
                        externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) ?? externalLoginInfo.ProviderKey,
                        tenancyName
                    );
            }
        }

        private async Task<ActionResult> RegisterForExternalLogin(ExternalLoginInfo externalLoginInfo)
        {
            var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            var nameinfo = ExternalLoginInfoHelper.GetNameAndSurnameFromClaims(externalLoginInfo.Principal.Claims.ToList());

            var viewModel = new RegisterViewModel
            {
                EmailAddress = email,
                Name = nameinfo.name,
                Surname = nameinfo.surname,
                IsExternalLogin = true,
                ExternalLoginAuthSchema = externalLoginInfo.LoginProvider
            };

            if (nameinfo.name != null &&
                nameinfo.surname != null &&
                email != null)
            {
                return await Register(viewModel);
            }

            return RegisterView(viewModel);
        }

        [UnitOfWork]
        protected virtual async Task<List<Tenant>> FindPossibleTenantsOfUserAsync(UserLoginInfo login)
        {
            List<User> allUsers;
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                allUsers = await _userManager.FindAllAsync(login);
            }

            return allUsers
                .Where(u => u.TenantId != null)
                .Select(u => AsyncHelper.RunSync(() => _tenantManager.FindByIdAsync(u.TenantId.Value)))
                .ToList();
        }

        #endregion

        #region Helpers

        public ActionResult RedirectToAppHome()
        {
            return RedirectToAction("Index", "Home");
        }

        public string GetAppHomeUrl()
        {
            return Url.Action("Index", "About");
        }

        #endregion

        #region Change Tenant

        public async Task<ActionResult> TenantChangeModal()
        {
            var loginInfo = await _sessionAppService.GetCurrentLoginInformations();
            return View("/Views/Shared/Components/TenantChange/_ChangeModal.cshtml", new ChangeModalViewModel
            {
                TenancyName = loginInfo.Tenant?.TenancyName
            });
        }

        #endregion

        #region Common

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private string NormalizeReturnUrl(string returnUrl, Func<string> defaultValueBuilder = null)
        {
            if (defaultValueBuilder == null)
            {
                defaultValueBuilder = GetAppHomeUrl;
            }

            if (returnUrl.IsNullOrEmpty())
            {
                return defaultValueBuilder();
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                return returnUrl;
            }

            return defaultValueBuilder();
        }

        #endregion

        #region Etc

        /// <summary>
        /// This is a demo code to demonstrate sending notification to default tenant admin and host admin uers.
        /// Don't use this code in production !!!
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [AbpMvcAuthorize]
        public async Task<ActionResult> TestNotification(string message = "")
        {
            if (message.IsNullOrEmpty())
            {
                message = "This is a test notification, created at " + Clock.Now;
            }

            var defaultTenantAdmin = new UserIdentifier(1, 2);
            var hostAdmin = new UserIdentifier(null, 1);

            await _notificationPublisher.PublishAsync(
                    "App.SimpleMessage",
                    new MessageNotificationData(message),
                    severity: NotificationSeverity.Info,
                    userIds: new[] { defaultTenantAdmin, hostAdmin }
                 );

            return Content("Sent notification: " + message);
        }

        #endregion

        #region ForgotPassword
        public ActionResult ForgotPassword()
        {
            return ForgotPasswordView(new ForgotPasswordInput());
        }
        private ActionResult ForgotPasswordView(ForgotPasswordInput model)
        {
            ViewBag.IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled;
            _sendEmail.ExecuteAsync(null, CancellationToken.None);
            return View("ForgotPassword", model);
        }

        #endregion
    }
}
