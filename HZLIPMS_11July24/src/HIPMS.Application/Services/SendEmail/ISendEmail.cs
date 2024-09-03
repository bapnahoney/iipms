using HIPMS.Options;
using System.Threading;
using System.Threading.Tasks;

namespace HIPMS.Services.SendEmail;
public interface ISendEmail
{
    Task<bool> ExecuteAsync(SendEmailOptions options, CancellationToken cancellationToken);
    Task<bool> ExecuteSMTPAsync(EmailNotificationSettingsOptions options, CancellationToken cancellationToken);
    Task<bool> ExecuteSMTPAsync(string addressesList, string subject, string body, CancellationToken cancellationToken);
}