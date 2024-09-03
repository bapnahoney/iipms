using Abp.Authorization;
using HIPMS.Authorization.Roles;
using HIPMS.Authorization.Users;

namespace HIPMS.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
