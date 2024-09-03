using System.ComponentModel.DataAnnotations;

namespace HIPMS.Web.Models.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string UsernameOrEmailAddress { get; set; }

    }
}
