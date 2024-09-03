using Abp.MultiTenancy;
using HIPMS.Authorization.Users;

namespace HIPMS.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
