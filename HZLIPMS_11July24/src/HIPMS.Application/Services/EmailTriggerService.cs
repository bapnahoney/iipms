using Abp.Runtime.Session;
using HIPMS.Authorization.PO;
using HIPMS.Authorization.Users;
using HIPMS.EntityFrameworkCore;
using HIPMS.Options;
using HIPMS.Roles;
using HIPMS.Services.SendEmail;
using HIPMS.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HIPMS.Services;


public class EmailTriggerService : IEmailTriggerService
{
    private readonly HIPMSDbContext _db;
    private readonly IPOService _iPOService;
    private readonly IAbpSession _abpSession;
    private readonly UserManager _userManager;
    private readonly SAPSettingsOptions _sapSettings;
    private readonly IRoleAppService _roleAppService;
    private readonly ISendEmail _sendEmail;


    public EmailTriggerService(HIPMSDbContext db, UserManager userManager, IPOService iPOService,
                               IAbpSession abpSession, IRoleAppService roleAppService,
                               IOptions<SAPSettingsOptions> sapSettings, ISendEmail sendEmail)
    {
        _db = db;
        _abpSession = abpSession;
        _iPOService = iPOService;
        _userManager = userManager;
        _sapSettings = sapSettings.Value;
        _roleAppService = roleAppService;
        _sendEmail = sendEmail;
    }

    public async Task<bool> SendMailOnForgotPassword(ForgotPasswordInput req)
    {
        User user = await _db.Users.Where(x => x.EmailAddress == req.UserEmail && x.UserName == req.Username).SingleOrDefaultAsync();

        if (user == null)
        {
            throw new InvalidOperationException("Invalid email.");
        }
        try
        {
            //uncomment when final approval
            //await _userManager.ChangePasswordAsync(user, req.Username.FirstOrDefault() + "123qwe");
            //await _sendEmail.ExecuteSMTPAsync(user.EmailAddress, "ForgotPassword", "Your password has been reset successfully. New Password: " + req.Username.FirstOrDefault() + "123qwe", CancellationToken.None);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to send Mail.");
        }
        return true;
    }

}
