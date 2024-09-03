

namespace HIPMS.Shared;

public class ForgotPasswordInput
{
    public long UserId { get; set; }
    public string Username { get; set; }
    public string UserEmail { get; set; }
    public string NewPassword { get; set; }
    public string Addresslist { get; set; } = string.Empty;
}
