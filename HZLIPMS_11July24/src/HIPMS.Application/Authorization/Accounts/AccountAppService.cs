using Abp.Configuration;
using Abp.Zero.Configuration;
using HIPMS.Authorization.Accounts.Dto;
using HIPMS.Authorization.Users;
using HIPMS.Services.SendEmail;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HIPMS.Authorization.Accounts
{
    public class AccountAppService : HIPMSAppServiceBase, IAccountAppService
    {
        // from: http://regexlib.com/REDetails.aspx?regexp_id=1923
        public const string PasswordRegex = "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\\s)[0-9a-zA-Z!@#$%^&*()]*$";

        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly ISendEmail _sendEmail;
        public AccountAppService(ISendEmail sendEmail,
        UserRegistrationManager userRegistrationManager)
        {
            _userRegistrationManager = userRegistrationManager;
            _sendEmail = sendEmail;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.Surname,
                input.EmailAddress,
                input.UserName,
                input.Password,
                true // Assumed email address is always confirmed. Change this if you want to implement email confirmation.
            );

            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);
            try
            {
                await _sendEmail.ExecuteSMTPAsync(user.EmailAddress, "New Account Registration", "Your account has been registered successfully. UserName: " + input.UserName + "Password:" + input.Password, CancellationToken.None);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }
    }
}
