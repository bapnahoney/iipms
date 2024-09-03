using HIPMS.Shared;
using System.Threading.Tasks;

namespace HIPMS.Services;
public interface IEmailTriggerService
{
    Task<bool> SendMailOnForgotPassword(ForgotPasswordInput req);
}
